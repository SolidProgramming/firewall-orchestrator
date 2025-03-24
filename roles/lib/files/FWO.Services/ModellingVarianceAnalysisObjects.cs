﻿using FWO.Data;
using FWO.Data.Modelling;
using FWO.Data.Workflow;
using FWO.Logging;

namespace FWO.Services
{
    /// <summary>
	/// Part of Variance Analysis Class analysing the network and service objects
	/// </summary>
    public partial class ModellingVarianceAnalysis
    {
        private ModellingAppRole? existingAppRole;
        private List<ModellingAppServerWrapper> newAppServers = [];
        private List<ModellingAppServerWrapper> deletedAppServers = [];
        private List<ModellingAppServerWrapper> unchangedAppServers = [];
        private List<WfReqElement> newGroupMembers = [];
        private List<WfReqElement> newCreatedGroupMembers = [];
        private List<WfReqElement> deletedGroupMembers = [];
        private List<WfReqElement> unchangedGroupMembersDuringCreate = [];
        private List<WfReqElement> unchangedGroupMembers = [];


        private void AnalyseNetworkAreas(ModellingConnection conn)
        {
            foreach(var area in ModellingNetworkAreaWrapper.Resolve(conn.SourceAreas))
            {
                elements.Add(new()
                {
                    RequestAction = RequestAction.create.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    GroupName = area.IdString
                });
            }
            foreach(var area in ModellingNetworkAreaWrapper.Resolve(conn.DestinationAreas))
            {
                elements.Add(new()
                {
                    RequestAction = RequestAction.create.ToString(),
                    Field = ElemFieldType.destination.ToString(),
                    GroupName = area.IdString
                });
            }
        }

        private void AnalyseAppRoles(ModellingConnection conn, Management mgt)
        {
            foreach (ModellingAppRole srcAppRole in ModellingAppRoleWrapper.Resolve(conn.SourceAppRoles))
            {
                AnalyseAppRole(srcAppRole, mgt, true);
            }
            foreach (ModellingAppRole dstAppRole in ModellingAppRoleWrapper.Resolve(conn.DestinationAppRoles))
            {
                AnalyseAppRole(dstAppRole, mgt);
            }
        }

        private void AnalyseAppRole(ModellingAppRole appRole, Management mgt, bool isSource = false)
        {
            if (!ResolveExistingNwGroup(appRole, mgt))
            {
                if (TaskList.FirstOrDefault(x => x.Title == userConfig.GetText("new_app_role") + appRole.IdString && x.OnManagement?.Id == mgt.Id) == null)
                {
                    RequestNewNwGroup(appRole, mgt);
                }
            }
            else if (NwGroupChanged(appRole) &&
                TaskList.FirstOrDefault(x => x.Title == userConfig.GetText("update_app_role") + appRole.IdString + userConfig.GetText("add_members") && x.OnManagement?.Id == mgt.Id) == null &&
                DeleteTasksList.FirstOrDefault(x => x.Title == userConfig.GetText("update_app_role") + appRole.IdString + userConfig.GetText("remove_members") && x.OnManagement?.Id == mgt.Id) == null)
            {
                RequestUpdateNwGroup(appRole, mgt);
            }

            elements.Add(new()
            {
                RequestAction = RequestAction.create.ToString(),
                Field = isSource ? ElemFieldType.source.ToString() : ElemFieldType.destination.ToString(),
                GroupName = appRole.IdString
            });
        }

        private async Task AnalyseAppZone(Management mgt)
        {
            if (!userConfig.CreateAppZones)
            {
                return;
            }

            ModellingAppZone? existingAppZone = await AppZoneHandler.GetExistingAppZone();

            if (existingAppZone is not null)
            {               
                if (!ResolveExistingNwGroup(existingAppZone, mgt))
                {
                    RequestNewNwGroup(existingAppZone, mgt);
                }
                else if (NwGroupChanged(existingAppZone) )
                {
                    RequestUpdateNwGroup(existingAppZone, mgt);
                }
            }
        }

        private bool ResolveExistingNwGroup(ModellingNwGroup nwGroup, Management mgt)
        {
            string nwGroupType = nwGroup.GetType() == typeof(ModellingAppRole) ? "AppRole" : "AppZone"; 
            Log.WriteDebug($"Search {nwGroupType}", $"Name: {nwGroup.Name}, IdString: {nwGroup.IdString}, Management: {mgt.Name}");

            bool shortened = false;
            string sanitizedARName = Sanitizer.SanitizeJsonFieldMand(nwGroup.IdString, ref shortened);
            if (allExistingAppRoles.ContainsKey(mgt.Id))
            {
                existingAppRole = allExistingAppRoles[mgt.Id].FirstOrDefault(a => a.Name == nwGroup.IdString || a.Name == sanitizedARName);
            }
            if (existingAppRole != null)
            {
                Log.WriteDebug($"Search {nwGroupType}", $"Found!!");
            }

            return existingAppRole != null;
        }

        private (long?, bool) ResolveAppServerId(ModellingAppServer appServer, Management mgt)
        {
            Log.WriteDebug("Search AppServer", $"Name: {appServer.Name}, Ip: {appServer.Ip}, Management: {mgt.Name}");

            ModellingAppServer? existingAppServer = allExistingAppServers[mgt.Id].FirstOrDefault(a => appServerComparer.Equals(a, appServer));
            if (existingAppServer != null)
            {
                Log.WriteDebug("Search AppServer", $"Found!!");
                return (existingAppServer?.Id, true);
            }
            else if (alreadyCreatedAppServers[mgt.Id].FirstOrDefault(a => appServerComparer.Equals(a, appServer)) != null)
            {
                return (null, true);
            }
            else
            {
                alreadyCreatedAppServers[mgt.Id].Add(appServer);
                return (null, false);
            }
        }

        private bool NwGroupChanged(ModellingNwGroup nwGroup)
        {
            newAppServers = [];
            deletedAppServers = [];
            unchangedAppServers = [];

            if (existingAppRole is null)
            {
                return false;
            }

            foreach (ModellingAppServerWrapper appserver in ( (ModellingAppRole)nwGroup ).AppServers)
            {
                if (existingAppRole.AppServers.FirstOrDefault(a => appServerComparer.Equals(a.Content, appserver.Content)) != null)
                {
                    unchangedAppServers.Add(appserver);
                }
                else
                {
                    newAppServers.Add(appserver);
                }
            }
            foreach (ModellingAppServerWrapper exAppserver in existingAppRole.AppServers)
            {
                if (( (ModellingAppRole)nwGroup ).AppServers.FirstOrDefault(a => appServerComparer.Equals(exAppserver.Content, a.Content)) == null)
                {
                    deletedAppServers.Add(exAppserver);
                }
            }
            return newAppServers.Count > 0 || deletedAppServers.Count > 0;
        }

        private void RequestNewNwGroup(ModellingNwGroup nwGroup, Management mgt)
        {
            string title = "";

            if (nwGroup.GetType() == typeof(ModellingAppRole))
            {
                title = userConfig.GetText("new_app_role");
            }
            else if (nwGroup.GetType() == typeof(ModellingAppZone))
            {
                title = userConfig.GetText("new_app_zone");
            }

            List<WfReqElement> groupMembers = [];
            foreach (ModellingAppServer appServer in ModellingAppServerWrapper.Resolve(( (ModellingAppRole)nwGroup ).AppServers))
            {
                (long? networkId, bool alreadyRequested) = ResolveAppServerId(appServer, mgt);
                groupMembers.Add(new()
                {
                    RequestAction = alreadyRequested ? RequestAction.addAfterCreation.ToString() : RequestAction.create.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    Name = AppServerHelper.ConstructAppServerName(appServer, namingConvention),
                    IpString = appServer.Ip,
                    IpEnd = appServer.IpEnd,
                    GroupName = nwGroup.IdString,
                    NetworkId = networkId
                });
            }
            Dictionary<string, string>? addInfo = new() { { AdditionalInfoKeys.GrpName, nwGroup.IdString }, { AdditionalInfoKeys.AppRoleId, nwGroup.Id.ToString() } };
            TaskList.Add(new()
            {
                Title = title + nwGroup.IdString,
                TaskType = WfTaskType.group_create.ToString(),
                RequestAction = RequestAction.create.ToString(),
                ManagementId = mgt.Id,
                OnManagement = mgt,
                Elements = groupMembers,
                AdditionalInfo = System.Text.Json.JsonSerializer.Serialize(addInfo)
            });
        }

        private void RequestUpdateNwGroup(ModellingNwGroup nwGroup, Management mgt)
        {
            string title = "";

            if (nwGroup.GetType() == typeof(ModellingAppRole))
            {
                title = userConfig.GetText("update_app_role");
            }
            else if (nwGroup.GetType() == typeof(ModellingAppZone))
            {
                title = userConfig.GetText("update_app_zone");
            }

            FillGroupMembers(nwGroup.IdString, mgt);
            Dictionary<string, string>? addInfo = new() { { AdditionalInfoKeys.GrpName, nwGroup.IdString }, { AdditionalInfoKeys.AppRoleId, nwGroup.Id.ToString() } };
            if (newGroupMembers.Count > 0)
            {
                newGroupMembers.AddRange(unchangedGroupMembers);
                newGroupMembers.AddRange(unchangedGroupMembersDuringCreate); // will be deleted later
                TaskList.Add(new()
                {
                    Title = title + nwGroup.IdString + userConfig.GetText("add_members"),
                    TaskType = WfTaskType.group_modify.ToString(),
                    RequestAction = RequestAction.modify.ToString(),
                    ManagementId = mgt.Id,
                    OnManagement = mgt,
                    Elements = newGroupMembers,
                    AdditionalInfo = System.Text.Json.JsonSerializer.Serialize(addInfo)
                });
            }
            if (deletedGroupMembers.Count > 0)
            {
                deletedGroupMembers.AddRange(unchangedGroupMembers);
                deletedGroupMembers.AddRange(newCreatedGroupMembers);
                DeleteTasksList.Add(new()
                {
                    Title = title + nwGroup.IdString + userConfig.GetText("remove_members"),
                    TaskType = WfTaskType.group_modify.ToString(),
                    RequestAction = RequestAction.modify.ToString(),
                    ManagementId = mgt.Id,
                    OnManagement = mgt,
                    Elements = deletedGroupMembers,
                    AdditionalInfo = System.Text.Json.JsonSerializer.Serialize(addInfo)
                });
            }
        }

        private void FillGroupMembers(string idString, Management mgt)
        {
            newGroupMembers = [];
            newCreatedGroupMembers = [];
            deletedGroupMembers = [];
            unchangedGroupMembers = [];
            unchangedGroupMembersDuringCreate = [];
            foreach (ModellingAppServerWrapper appServer in newAppServers)
            {
                (long? networkId, bool alreadyRequested) = ResolveAppServerId(appServer.Content, mgt);
                newGroupMembers.Add(new()
                {
                    RequestAction = alreadyRequested ? RequestAction.addAfterCreation.ToString() : RequestAction.create.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    Name = AppServerHelper.ConstructAppServerName(appServer.Content, namingConvention),
                    IpString = appServer.Content.Ip,
                    IpEnd = appServer.Content.IpEnd,
                    GroupName = idString,
                    NetworkId = networkId
                });
                newCreatedGroupMembers.Add(new()
                {
                    RequestAction = RequestAction.unchanged.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    Name = appServer.Content.Name,
                    IpString = appServer.Content.Ip,
                    IpEnd = appServer.Content.IpEnd,
                    GroupName = idString,
                    NetworkId = networkId
                });
            }
            foreach (ModellingAppServerWrapper appServer in unchangedAppServers)
            {
                unchangedGroupMembers.Add(new()
                {
                    RequestAction = RequestAction.unchanged.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    Name = appServer.Content.Name,
                    IpString = appServer.Content.Ip,
                    IpEnd = appServer.Content.IpEnd,
                    GroupName = idString
                });
            }
            foreach (ModellingAppServerWrapper appServer in deletedAppServers)
            {
                unchangedGroupMembersDuringCreate.Add(new()
                {
                    RequestAction = RequestAction.unchanged.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    Name = appServer.Content.Name,
                    IpString = appServer.Content.Ip,
                    IpEnd = appServer.Content.IpEnd,
                    GroupName = idString
                });
                deletedGroupMembers.Add(new()
                {
                    RequestAction = RequestAction.delete.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    Name = appServer.Content.Name,
                    IpString = appServer.Content.Ip,
                    IpEnd = appServer.Content.IpEnd,
                    GroupName = idString
                });
            }
        }

        private void AnalyseAppServers(ModellingConnection conn)
        {
            foreach (ModellingAppServerWrapper srcAppServer in conn.SourceAppServers)
            {
                elements.Add(new()
                {
                    RequestAction = RequestAction.create.ToString(),
                    Field = ElemFieldType.source.ToString(),
                    Name = srcAppServer.Content.Name,
                    IpString = srcAppServer.Content.Ip,
                    IpEnd = srcAppServer.Content.IpEnd
                });
            }
            foreach (ModellingAppServerWrapper dstAppServer in conn.DestinationAppServers)
            {
                elements.Add(new()
                {
                    RequestAction = RequestAction.create.ToString(),
                    Field = ElemFieldType.destination.ToString(),
                    Name = dstAppServer.Content.Name,
                    IpString = dstAppServer.Content.Ip,
                    IpEnd = dstAppServer.Content.IpEnd
                });
            }
        }

        private void AnalyseServiceGroups(ModellingConnection conn, Management mgt)
        {
            foreach (ModellingServiceGroup svcGrp in ModellingServiceGroupWrapper.Resolve(conn.ServiceGroups))
            {
                if (userConfig.ModRolloutResolveServiceGroups)
                {
                    foreach (ModellingService svc in ModellingServiceWrapper.Resolve(svcGrp.Services))
                    {
                        elements.Add(new()
                        {
                            RequestAction = RequestAction.create.ToString(),
                            Field = ElemFieldType.service.ToString(),
                            Name = svc.Name,
                            Port = svc.Port,
                            PortEnd = svc.PortEnd,
                            ProtoId = svc.ProtoId
                        });
                    }
                }
                else
                {
                    if (TaskList.FirstOrDefault(x => x.Title == userConfig.GetText("new_svc_grp") + svcGrp.Name && x.OnManagement?.Id == mgt.Id) == null)
                    {
                        RequestNewServiceGroup(svcGrp, mgt);
                    }
                    elements.Add(new()
                    {
                        RequestAction = RequestAction.create.ToString(),
                        Field = ElemFieldType.service.ToString(),
                        GroupName = svcGrp.Name
                    });
                }
            }
        }

        private void RequestNewServiceGroup(ModellingServiceGroup svcGrp, Management mgt)
        {
            List<WfReqElement> groupMembers = [];
            foreach (ModellingService svc in ModellingServiceWrapper.Resolve(svcGrp.Services))
            {
                groupMembers.Add(new()
                {
                    RequestAction = RequestAction.create.ToString(),
                    Field = ElemFieldType.service.ToString(),
                    Name = svc.Name,
                    Port = svc.Port,
                    PortEnd = svc.PortEnd,
                    ProtoId = svc.ProtoId,
                    GroupName = svcGrp.Name
                });
            }
            Dictionary<string, string>? addInfo = new() { { AdditionalInfoKeys.GrpName, svcGrp.Name }, { AdditionalInfoKeys.SvcGrpId, svcGrp.Id.ToString() } };
            TaskList.Add(new()
            {
                Title = userConfig.GetText("new_svc_grp") + svcGrp.Name,
                TaskType = WfTaskType.group_create.ToString(),
                ManagementId = mgt.Id,
                OnManagement = mgt,
                Elements = groupMembers,
                AdditionalInfo = System.Text.Json.JsonSerializer.Serialize(addInfo)
            });
        }

        private void AnalyseServices(ModellingConnection conn)
        {
            foreach (ModellingService svc in ModellingServiceWrapper.Resolve(conn.Services))
            {
                elements.Add(new()
                {
                    RequestAction = RequestAction.create.ToString(),
                    Field = ElemFieldType.service.ToString(),
                    Name = svc.Name,
                    Port = svc.Port,
                    PortEnd = svc.PortEnd,
                    ProtoId = svc.ProtoId
                });
            }
        }
    }
}
