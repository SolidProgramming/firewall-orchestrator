﻿using FWO.Logging;
using NetTools;
using FWO.Api.Client;
using FWO.Basics;
using FWO.Api.Data;
using FWO.Config.Api;
using System.Text.Json;
using FWO.Middleware.RequestParameters;
using FWO.Api.Client.Queries;
using Novell.Directory.Ldap;
using System.Data;

namespace FWO.Middleware.Server
{
	/// <summary>
	/// Class handling the App Data Import
	/// </summary>
	public class AppDataImport : DataImportBase
	{
		private List<ModellingImportAppData> importedApps = [];
		private List<FwoOwner> existingApps = [];
		private List<ModellingAppServer> existingAppServers = [];

		private Ldap internalLdap = new();
		private Ldap ownerGroupLdap = new();

		private List<Ldap> connectedLdaps = [];
		private string modellerRoleDn = "";
		private string requesterRoleDn = "";
		private string implementerRoleDn = "";
		private string reviewerRoleDn = "";
		List<GroupGetReturnParameters> allGroups = [];
		List<GroupGetReturnParameters> allInternalGroups = [];
	

		/// <summary>
		/// Constructor for App Data Import
		/// </summary>
		public AppDataImport(ApiConnection apiConnection, GlobalConfig globalConfig) : base(apiConnection, globalConfig)
		{ }

		/// <summary>
		/// Run the App Data Import
		/// </summary>
		public async Task<bool> Run()
		{
			try
			{
				List<string> importfilePathAndNames = JsonSerializer.Deserialize<List<string>>(globalConfig.ImportAppDataPath) ?? throw new Exception("Config Data could not be deserialized.");
				await InitLdap();
				foreach (var importfilePathAndName in importfilePathAndNames)
				{
					if (!RunImportScript(importfilePathAndName + ".py"))
					{
						Log.WriteInfo("Import App Data", $"Script {importfilePathAndName}.py failed but trying to import from existing file.");
					}
					await ImportSingleSource(importfilePathAndName + ".json");
				}
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Data", $"Import could not be processed.", exc);
				return false;
			}
			return true;
		}

		private async Task InitLdap()
		{
			connectedLdaps = await apiConnection.SendQueryAsync<List<Ldap>>(AuthQueries.getLdapConnections);
			internalLdap = connectedLdaps.FirstOrDefault(x => x.IsInternal() && x.HasGroupHandling()) ?? throw new Exception("No internal Ldap with group handling found.");
			ownerGroupLdap = connectedLdaps.FirstOrDefault(x => x.Id == globalConfig.OwnerLdapId) ?? throw new Exception("Ldap with group handling not found.");
			modellerRoleDn = $"cn=modeller,{internalLdap.RoleSearchPath}";
			requesterRoleDn = $"cn=requester,{internalLdap.RoleSearchPath}";
			implementerRoleDn = $"cn=implementer,{internalLdap.RoleSearchPath}";
			reviewerRoleDn = $"cn=reviewer,{internalLdap.RoleSearchPath}";
			allInternalGroups = internalLdap.GetAllInternalGroups();
			if (globalConfig.OwnerLdapId == GlobalConst.kLdapInternalId)
			{
				allGroups = allInternalGroups;	// TODO: check if ref is ok here
			}
			else
			{
				allGroups = ownerGroupLdap.GetAllGroupObjects(GetLdapSearchPattern(globalConfig.OwnerLdapGroupNames));
			}
		}

		private static string GetLdapSearchPattern(string ownerGroupNamePattern)
		{
			string searchPattern = ownerGroupNamePattern;
			// assuming that we remove everything after the kAppIdPlaceholder
			int index = ownerGroupNamePattern.IndexOf(GlobalConst.kAppIdPlaceholder);
			if (index != -1)
			{
				// Keep text up to the substring
				searchPattern = ownerGroupNamePattern.Substring(0, index + GlobalConst.kAppIdPlaceholder.Length);
			}
			// now remove CN= from pattern
			index = searchPattern.IndexOf('=');
			if (index != -1)
			{
				searchPattern = searchPattern.Substring(index + 1);
			}
			return searchPattern.Replace(GlobalConst.kAppIdPlaceholder, "*");
		}

		private async Task<bool> ImportSingleSource(string importfileName)
		{
			try
			{
				ReadFile(importfileName);
				ModellingImportOwnerData? importedOwnerData = JsonSerializer.Deserialize<ModellingImportOwnerData>(importFile) ?? throw new Exception("File could not be parsed.");
				if (importedOwnerData != null && importedOwnerData.Owners != null)
				{
					importedApps = importedOwnerData.Owners;
					await ImportApps(importfileName);
				}
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Data", $"File {importfileName} could not be processed.", exc);
				return false;
			}
			return true;
		}

		private async Task ImportApps(string importfileName)
		{
			int successCounter = 0;
			int failCounter = 0;
			int deleteCounter = 0;
			int deleteFailCounter = 0;

			if (!globalConfig.OwnerLdapGroupNames.Contains(GlobalConst.kAppIdPlaceholder))
			{
                Log.WriteWarning("Import App Data", $"Owner group pattern does not contain placeholder {GlobalConst.kAppIdPlaceholder}");
                Log.WriteAlert($"source: \"{GlobalConst.kImportAppData}\"",
                    $"userId: \"0\", title: \"Error encountered while trying to import App Data\", description: \"Owner group name does not contain placeholder {GlobalConst.kAppIdPlaceholder}\", alertCode: \"{AlertCode.ImportAppData}\"");
			}
			else
			{
				existingApps = await apiConnection.SendQueryAsync<List<FwoOwner>>(OwnerQueries.getOwners);
				foreach (var incomingApp in importedApps)
				{
					if (await SaveApp(incomingApp))
					{
						++successCounter;
					}
					else
					{
						++failCounter;
					}
				}
				string? importSource = importedApps.FirstOrDefault()?.ImportSource;
				if(importSource != null)
				{
					foreach (var existingApp in existingApps.Where(x => x.ImportSource == importSource && x.Active))
					{
						if (importedApps.FirstOrDefault(x => x.ExtAppId == existingApp.ExtAppId) == null)
						{
							if (await DeactivateApp(existingApp))
							{
								++deleteCounter;
							}
							else
							{
								++deleteFailCounter;
							}
						}
					}
				}
				Log.WriteInfo("Import App Data", $"Imported from {importfileName}: {successCounter} apps, {failCounter} failed. Deactivated {deleteCounter} apps, {deleteFailCounter} failed.");
			}
		}

		private async Task<bool> SaveApp(ModellingImportAppData incomingApp)
		{
			try
			{
				string userGroupDn;
				FwoOwner? existingApp = existingApps.FirstOrDefault(x => x.ExtAppId == incomingApp.ExtAppId);

				if (existingApp == null)
				{
					userGroupDn = await NewApp(incomingApp);
				}
				else
				{
					userGroupDn = await UpdateApp(incomingApp, existingApp);
				}
				// in order to store email addresses of users in the group in UiUser for email notification:
				await AddAllGroupMembersToUiUser(userGroupDn);
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Data", $"App {incomingApp.Name} could not be processed.", exc);
				return false;
			}
			return true;
		}

		private async Task<string> NewApp(ModellingImportAppData incomingApp)
		{
			string userGroupDn;
			if (globalConfig.ManageOwnerLdapGroups)
			{
				userGroupDn = CreateUserGroup(incomingApp);
			}
			else
			{
				userGroupDn = GetGroupName(incomingApp.ExtAppId);
			}

			var variables = new
			{
				name = incomingApp.Name,
				dn = incomingApp.MainUser ?? "",
				groupDn = userGroupDn,
				appIdExternal = incomingApp.ExtAppId,
				criticality = incomingApp.Criticality,
				importSource = incomingApp.ImportSource,
				commSvcPossible = false
			};
			ReturnId[]? returnIds = (await apiConnection.SendQueryAsync<NewReturning>(OwnerQueries.newOwner, variables)).ReturnIds;
			if (returnIds != null)
			{
				if(incomingApp.MainUser != null && incomingApp.MainUser != "")
				{
					UpdateRoles(incomingApp.MainUser);
				}
				int appId = returnIds[0].NewId;
				foreach (var appServer in incomingApp.AppServers)
				{
					await NewAppServer(appServer, appId, incomingApp.ImportSource);
				}
			}
			return userGroupDn;
		}

		private async Task<string> UpdateApp(ModellingImportAppData incomingApp, FwoOwner existingApp)
		{
			string userGroupDn = GetGroupName(incomingApp.ExtAppId);

			if (existingApp.GroupDn == null || existingApp.GroupDn == "")
			{
				GroupGetReturnParameters? groupWithSameName = allGroups.FirstOrDefault(x => new DistName(x.GroupDn).Group == GetGroupName(incomingApp.ExtAppId));
				if (groupWithSameName != null)
				{
					if (userGroupDn == "")
					{
						userGroupDn = groupWithSameName.GroupDn;
					}
					if (globalConfig.ManageOwnerLdapGroups)
					{
						UpdateUserGroup(incomingApp, groupWithSameName.GroupDn);
					}
				}
				else
				{
					if (globalConfig.ManageOwnerLdapGroups)
					{
						userGroupDn = CreateUserGroup(incomingApp);
					}
				}
			}
			else
			{
				if (globalConfig.ManageOwnerLdapGroups)
				{
					UpdateUserGroup(incomingApp, userGroupDn);
				}
			}

			var Variables = new
			{
				id = existingApp.Id,
				name = incomingApp.Name,
				dn = incomingApp.MainUser ?? "",
				groupDn = userGroupDn,
				appIdExternal = incomingApp.ExtAppId,
				criticality = incomingApp.Criticality,
				commSvcPossible = existingApp.CommSvcPossible
			};
			await apiConnection.SendQueryAsync<NewReturning>(OwnerQueries.updateOwner, Variables);
			if(incomingApp.MainUser != null && incomingApp.MainUser != "")
			{
				UpdateRoles(incomingApp.MainUser);
			}
			await ImportAppServers(incomingApp, existingApp.Id);
			return userGroupDn;
		}

		private async Task<bool> DeactivateApp(FwoOwner app)
		{
			try
			{
				await apiConnection.SendQueryAsync<NewReturning>(OwnerQueries.deactivateOwner, new { id = app.Id });
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Data", $"Outdated App {app.Name} could not be deactivated.", exc);
				return false;
			}
			return true;
		}

		private string GetGroupName(string extAppIdString)
		{
			return globalConfig.OwnerLdapGroupNames.Replace(GlobalConst.kAppIdPlaceholder, extAppIdString);
		}

		/// <summary>
		/// for each user of a remote ldap group create a user in uiuser
		/// this is necessary in order to get details like email address for users
		/// which have never logged in but who need to be notified via email
		/// </summary>
		private async Task AddAllGroupMembersToUiUser(string userGroupDn)
		{
			foreach (Ldap ldap in connectedLdaps)
			{
				foreach (string memberDn in ldap.GetGroupMembers(userGroupDn))
				{
					UiUser? uiUser = await ConvertLdapToUiUser(apiConnection, memberDn);
					if(uiUser != null)
					{
						await UiUserHandler.UpsertUiUser(apiConnection, uiUser, false);
					}
				}
			}
		}

		private async Task<UiUser?> ConvertLdapToUiUser(ApiConnection apiConnection, string userDn)
		{
			// add the modelling user to local uiuser table for later ref to email address
			// find the user in all connected ldaps
			foreach (Ldap ldap in connectedLdaps)
			{
				if (!string.IsNullOrEmpty(ldap.UserSearchPath) && userDn.ToLower().Contains(ldap.UserSearchPath!.ToLower()))
				{
					LdapEntry? ldapUser = ldap.GetUserDetailsFromLdap(userDn);
					
					if (ldapUser != null)
					{
						// add data from ldap entry to uiUser
						return new()
						{
							LdapConnection = new UiLdapConnection(){ Id = ldap.Id },
							Dn = ldapUser.Dn,
							Name = ldap.GetName(ldapUser),
							Firstname = ldap.GetFirstName(ldapUser),
							Lastname = ldap.GetLastName(ldapUser),
							Email = ldap.GetEmail(ldapUser),
							Tenant = await DeriveTenantFromLdap(ldap, ldapUser)							
						};
					}
				}
			}
			return null;
		}

		private async Task<Tenant> DeriveTenantFromLdap(Ldap ldap, LdapEntry ldapUser)
		{
			// try to derive the the user's tenant from the ldap settings
			Tenant tenant = new()
			{
				Id = GlobalConst.kTenant0Id  // default: tenant0 (id=1)
			};

			string tenantName = "";

			// can we derive the users tenant purely from its ldap?
			if (!string.IsNullOrEmpty(ldap.GlobalTenantName) || ldap.TenantLevel > 0)
			{
				if (ldap.TenantLevel > 0)
				{
					// getting tenant via tenant level setting from distinguished name
					tenantName = ldap.GetTenantName(ldapUser);
				}
				else
				{
					if (!string.IsNullOrEmpty(ldap.GlobalTenantName))
					{
						tenantName = ldap.GlobalTenantName ?? "";
					}
				}

				var variables = new { tenant_name = tenantName };
				Tenant[] tenants = await apiConnection.SendQueryAsync<Tenant[]>(AuthQueries.getTenantId, variables, "getTenantId");
				if (tenants.Length == 1)
				{
					tenant.Id = tenants[0].Id;
				}
			}
			return tenant;
		}

		private string CreateUserGroup(ModellingImportAppData incomingApp)
		{
			string groupDn = "";
			if (incomingApp.Modellers != null && incomingApp.Modellers.Count > 0
				|| incomingApp.ModellerGroups != null && incomingApp.ModellerGroups.Count > 0)
			{
				string groupName = GetGroupName(incomingApp.ExtAppId);
				groupDn = internalLdap.AddGroup(groupName, true);
				if (incomingApp.Modellers != null)
				{
					foreach (var modeller in incomingApp.Modellers)
					{
						// add user to internal group:
						internalLdap.AddUserToEntry(modeller, groupDn);
					}
				}
				if (incomingApp.ModellerGroups != null)
				{
					foreach (var modellerGrp in incomingApp.ModellerGroups)
					{
						internalLdap.AddUserToEntry(modellerGrp, groupDn);
					}
				}
				internalLdap.AddUserToEntry(groupDn, modellerRoleDn);
				internalLdap.AddUserToEntry(groupDn, requesterRoleDn);
				internalLdap.AddUserToEntry(groupDn, implementerRoleDn);
				internalLdap.AddUserToEntry(groupDn, reviewerRoleDn);
			}
			return groupDn;
		}

		private string UpdateUserGroup(ModellingImportAppData incomingApp, string groupDn)
		{
			List<string> existingMembers = (allGroups.FirstOrDefault(x => x.GroupDn == groupDn) ?? throw new Exception("Group could not be found.")).Members;
			if (incomingApp.Modellers != null)
			{
				foreach (var modeller in incomingApp.Modellers)
				{
					if (existingMembers.FirstOrDefault(x => x.Equals(modeller, StringComparison.CurrentCultureIgnoreCase)) == null)
					{
						internalLdap.AddUserToEntry(modeller, groupDn);
					}
				}
			}
			if (incomingApp.ModellerGroups != null)
			{
				foreach (var modellerGrp in incomingApp.ModellerGroups)
				{
					if (existingMembers.FirstOrDefault(x => x.Equals(modellerGrp, StringComparison.CurrentCultureIgnoreCase)) == null)
					{
						internalLdap.AddUserToEntry(modellerGrp, groupDn);
					}
				}
			}
			foreach (var member in existingMembers)
			{
				if ((incomingApp.Modellers == null || incomingApp.Modellers.FirstOrDefault(x => x.Equals(member, StringComparison.CurrentCultureIgnoreCase)) == null)
					&& (incomingApp.ModellerGroups == null || incomingApp.ModellerGroups.FirstOrDefault(x => x.Equals(member, StringComparison.CurrentCultureIgnoreCase)) == null))
				{
					internalLdap.RemoveUserFromEntry(member, groupDn);
				}
			}
			UpdateRoles(groupDn);
			return groupDn;
		}

		private void UpdateRoles(string dn)
		{
			List<string> roles = internalLdap.GetRoles([dn]);
			if(!roles.Contains(Roles.Modeller))
			{
				internalLdap.AddUserToEntry(dn, modellerRoleDn);
			}
			if(!roles.Contains(Roles.Requester))
			{
				internalLdap.AddUserToEntry(dn, requesterRoleDn);
			}
			if(!roles.Contains(Roles.Implementer))
			{
				internalLdap.AddUserToEntry(dn, implementerRoleDn);
			}
			if(!roles.Contains(Roles.Reviewer))
			{
				internalLdap.AddUserToEntry(dn, reviewerRoleDn);
			}
		}

		private async Task ImportAppServers(ModellingImportAppData incomingApp, int applId)
		{
			int successCounter = 0;
			int failCounter = 0;
			int deleteCounter = 0;
			int deleteFailCounter = 0;

			var Variables = new
			{
				importSource = incomingApp.ImportSource,
				appId = applId
			};
			existingAppServers = await apiConnection.SendQueryAsync<List<ModellingAppServer>>(ModellingQueries.getImportedAppServers, Variables);
			foreach (var incomingAppServer in incomingApp.AppServers)
			{
				if (await SaveAppServer(incomingAppServer, applId, incomingApp.ImportSource))
				{
					++successCounter;
				}
				else
				{
					++failCounter;
				}
			}
			foreach (var existingAppServer in existingAppServers)
			{
				if (incomingApp.AppServers.FirstOrDefault(x => IpAsCidr(x.Ip) == IpAsCidr(existingAppServer.Ip)) == null)
				{
					if (await MarkDeletedAppServer(existingAppServer))
					{
						++deleteCounter;
					}
					else
					{
						++deleteFailCounter;
					}
				}
			}
			Log.WriteDebug($"Import App Server Data for App {incomingApp.Name}", $"Imported {successCounter} app servers, {failCounter} failed. {deleteCounter} app servers marked as deleted, {deleteFailCounter} failed.");
		}

		private async Task<bool> SaveAppServer(ModellingImportAppServer incomingAppServer, int appID, string impSource)
		{
			try
			{
				ModellingAppServer? existingAppServer = existingAppServers.FirstOrDefault(x => IpAsCidr(x.Ip) == IpAsCidr(incomingAppServer.Ip));
				if (existingAppServer == null)
				{
					return await NewAppServer(incomingAppServer, appID, impSource);
				}
				else
				{
					if (existingAppServer.IsDeleted)
					{
						if (!await ReactivateAppServer(existingAppServer))
						{	
							return false;
						}
					}
					if (!existingAppServer.Name.Equals(incomingAppServer.Name))
					{
						if (!await UpdateAppServerName(existingAppServer, BuildAppServerName(incomingAppServer)))
						{	
							return false;
						}
					}
					if (existingAppServer.CustomType == null)
					{
						if (!await UpdateAppServerType(existingAppServer))
						{	
							return false;
						}
					}
				}
				return true;
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Server Data", $"App Server {incomingAppServer.Name} could not be processed.", exc);
				return false;
			}
		}

		private string BuildAppServerName(ModellingImportAppServer appServer)
		{
			bool changed = false;
			try
			{
				if (string.IsNullOrEmpty(appServer.Name))
				{
					Log.WriteWarning("Import App Server Data", $"Found empty (unresolvable) IP {appServer.Ip}");
					ModellingNamingConvention NamingConvention = JsonSerializer.Deserialize<ModellingNamingConvention>(globalConfig.ModNamingConvention) ?? new();
					return Sanitizer.SanitizeJsonFieldMand(NamingConvention.AppServerPrefix + DisplayBase.DisplayIp(appServer.Ip, appServer.IpEnd), ref changed);
				}
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Server Data", $"App Server name {appServer.Name} could not be set according to naming conventions.", exc);
			}
			return appServer.Name;
		}

		private async Task<bool> NewAppServer(ModellingImportAppServer incomingAppServer, int appID, string impSource)
		{
			try
			{
				var Variables = new
				{
					name = BuildAppServerName(incomingAppServer),
					appId = appID,
					ip = IpAsCidr(incomingAppServer.Ip),
					ipEnd = incomingAppServer.IpEnd != "" ? IpAsCidr(incomingAppServer.IpEnd) : IpAsCidr(incomingAppServer.Ip),
					importSource = impSource,
					customType = 0
				};
				await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.newAppServer, Variables);
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Server Data", $"App Server {incomingAppServer.Name} could not be processed.", exc);
				return false;
			}
			return true;
		}

		private async Task<bool> ReactivateAppServer(ModellingAppServer appServer)
		{
			try
			{
				var Variables = new
				{
					id = appServer.Id,
					deleted = false
				};
				await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.setAppServerDeletedState, Variables);
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Server Data", $"App Server {appServer.Name} could not be reactivated.", exc);
				return false;
			}
			return true;
		}

		private async Task<bool> UpdateAppServerType(ModellingAppServer appServer)
		{
			try
			{
				var Variables = new
				{
					id = appServer.Id,
					customType = 0
				};
				await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.setAppServerType, Variables);
			}
			catch (Exception exc)
			{
				Log.WriteError("Import App Server Data", $"Type of App Server {appServer.Name} could not be set.", exc);
				return false;
			}
			return true;
		}

		private async Task<bool> UpdateAppServerName(ModellingAppServer appServer, string newName)
		{
			if (appServer.Name != newName)
			{
				try
				{
					var Variables = new
					{
						newName,
						id = appServer.Id,
					};
					await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.setAppServerName, Variables);
					Log.WriteWarning("Import App Server Data", $"Name of App Server changed from {appServer.Name} changed to {newName}");
					
				}
				catch (Exception exc)
				{
					Log.WriteError("Import App Server Data", $"Name of App Server {appServer.Name} could not be set to {newName}.", exc);
					return false;
				}
			}
			return true;
		}

		private async Task<bool> MarkDeletedAppServer(ModellingAppServer appServer)
		{
			try
			{
				var Variables = new
				{
					id = appServer.Id,
					deleted = true
				};
				await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.setAppServerDeletedState, Variables);
			}
			catch (Exception exc)
			{
				Log.WriteError("Import AppServer Data", $"Outdated AppServer {appServer.Name} could not be marked as deleted.", exc);
				return false;
			}
			return true;
		}

		private static string IpAsCidr(string ip)
		{
			return IPAddressRange.Parse(ip).ToCidrString();
		}
	}
}
