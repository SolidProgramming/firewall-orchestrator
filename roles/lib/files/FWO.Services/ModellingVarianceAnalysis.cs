﻿using FWO.Api.Client;
using FWO.Config.Api;
using FWO.Data;
using FWO.Data.Modelling;
using FWO.Data.Report;
using FWO.Data.Workflow;
using System.Text.Json; 


namespace FWO.Services
{
    /// <summary>
	/// Variance Analysis Class
	/// </summary>
    public partial class ModellingVarianceAnalysis(ApiConnection apiConnection, ExtStateHandler extStateHandler,
            UserConfig userConfig, FwoOwner owner, Action<Exception?, string, string, bool> displayMessageInUi)
    {
        private readonly ModellingNamingConvention namingConvention = JsonSerializer.Deserialize<ModellingNamingConvention>(userConfig.ModNamingConvention) ?? new();
        private readonly RuleRecognitionOption ruleRecognitionOption = string.IsNullOrEmpty(userConfig.RuleRecognitionOption) ? new() : 
            JsonSerializer.Deserialize<RuleRecognitionOption>(userConfig.RuleRecognitionOption) ?? new();
        private readonly ModellingAppZoneHandler AppZoneHandler = new(apiConnection, userConfig, owner, displayMessageInUi);
        private AppServerComparer appServerComparer = new(new());
        private List<Management> RelevantManagements = [];

        private List<WfReqTask> TaskList = [];
        private List<WfReqTask> AccessTaskList = [];
        private List<WfReqTask> DeleteTasksList = [];
        private int taskNumber = 0;
        private List<WfReqElement> elements = [];

        private ModellingVarianceResult varianceResult = new();
        
        private Dictionary<int, List<Rule>> allModelledRules = [];
        private List<ModellingAppRole> allModelledAppRoles = [];

        private readonly Dictionary<int, List<ModellingAppRole>> allProdAppRoles = [];
        private readonly Dictionary<int, List<ModellingAppServer>> allExistingAppServers = [];
        private readonly Dictionary<int, List<ModellingAppServer>> alreadyCreatedAppServers = [];

        public ModellingAppZone? PlannedAppZone = default;

        public async Task<ModellingVarianceResult> AnalyseRulesVsModelledConnections(List<ModellingConnection> connections, ModellingFilter modellingFilter)
        {
            await InitManagements();
            varianceResult = new() { Managements = RelevantManagements };
            if(ruleRecognitionOption.NwSeparateGroupAnalysis)
            {
                await GetNwObjectsProductionState();
                PreAnalyseAllAppRoles(connections);
            }
            if(await GetModelledRulesProductionState(modellingFilter))
            {
                foreach(var conn in connections.Where(c => !c.IsInterface).OrderBy(c => c.Id))
                {
                    AnalyseRules(conn);
                }
            }
            return varianceResult;
        }

        private void PreAnalyseAllAppRoles(List<ModellingConnection> connections)
        {
            CollectModelledAppRoles(connections);
            foreach (Management mgt in RelevantManagements)
            {
                varianceResult.MissingAppRoles.Add(mgt.Id, []);
                varianceResult.DifferingAppRoles.Add(mgt.Id, []);
                foreach (var modelledAppRole in allModelledAppRoles)
                {
                    AnalyseAppRole(modelledAppRole, mgt);
                }
            }
            varianceResult.AppRoleStats.ModelledAppRolesCount = allModelledAppRoles.Count;
            varianceResult.AppRoleStats.AppRolesOk = allModelledAppRoles.Where(a => !a.IsMissing && !a.HasDifference).Count();
            varianceResult.AppRoleStats.AppRolesMissingCount = allModelledAppRoles.Where(a => a.IsMissing).Count();
            varianceResult.AppRoleStats.AppRolesDifferenceCount = allModelledAppRoles.Where(a => a.HasDifference).Count();
        }

        private void CollectModelledAppRoles(List<ModellingConnection> connections)
        {
            AppRoleComparer appRoleComparer = new ();
            foreach(var conn in connections.Where(c => !c.IsInterface).OrderBy(c => c.Id))
            {
                foreach(var modelledAppRole in ModellingAppRoleWrapper.Resolve(conn.SourceAppRoles))
                {
                    allModelledAppRoles.Add(modelledAppRole);
                }
                foreach(var modelledAppRole in ModellingAppRoleWrapper.Resolve(conn.DestinationAppRoles))
                {
                    allModelledAppRoles.Add(modelledAppRole);
                }
            }
            allModelledAppRoles = allModelledAppRoles.Distinct(appRoleComparer).ToList();
        }

        private void AnalyseAppRole(ModellingAppRole modelledAppRole, Management mgt)
        {
            if (ResolveProdAppRole(modelledAppRole, mgt) == null)
            {
                modelledAppRole.IsMissing = true;
                varianceResult.MissingAppRoles[mgt.Id].Add(new(modelledAppRole){ ManagementName = mgt.Name });
            }
            else if (AppRoleChanged(modelledAppRole))
            {
                modelledAppRole.HasDifference = true;
                ModellingAppRole changedAppRole = new(modelledAppRole){ ManagementName = mgt.Name, SurplusAppServers = deletedAppServers};
                foreach(var appServer in changedAppRole.AppServers)
                {
                    appServer.Content.NotImplemented = newAppServers.FirstOrDefault(a => a.Content.Id == appServer.Content.Id) != null;
                }
                varianceResult.DifferingAppRoles[mgt.Id].Add(changedAppRole);
            }
        }

        public async Task<List<WfReqTask>> AnalyseModelledConnectionsForRequest(List<ModellingConnection> connections)
        {
            // later: get rules + compare, bundle requests
            appServerComparer = new (namingConvention);
            await InitManagements();
            await GetNwObjectsProductionState();

            TaskList = [];
            AccessTaskList = [];
            DeleteTasksList = [];
            foreach (Management mgt in RelevantManagements)
            {
                await AnalyseAppZone(mgt);
                foreach (var conn in connections.Where(c => !c.IsRequested).OrderBy(c => c.Id))
                {
                    elements = [];
                    AnalyseNetworkAreasForRequest(conn);
                    AnalyseAppRolesForRequest(conn, mgt);
                    AnalyseAppServersForRequest(conn);
                    AnalyseServiceGroupsForRequest(conn, mgt);
                    AnalyseServicesForRequest(conn);
                    if (elements.Count > 0)
                    {
                        Dictionary<string, string>? addInfo = new() { { AdditionalInfoKeys.ConnId, conn.Id.ToString() } };
                        AccessTaskList.Add(new()
                        {
                            Title = ( conn.IsCommonService ? userConfig.GetText("new_common_service") : userConfig.GetText("new_connection") ) + ": " + conn.Name ?? "",
                            TaskType = WfTaskType.access.ToString(),
                            ManagementId = mgt.Id,
                            OnManagement = mgt,
                            Elements = elements,
                            RuleAction = 1,  // Todo ??
                            Tracking = 1,  // Todo ??
                            AdditionalInfo = JsonSerializer.Serialize(addInfo),
                            Comments = [new() { Comment = new() { CommentText = ConstructComment(conn) } }]
                        });
                    }
                }
            }
            TaskList.AddRange(AccessTaskList);
            TaskList.AddRange(DeleteTasksList);
            taskNumber = 1;
            foreach (WfReqTask task in TaskList)
            {
                task.TaskNumber = taskNumber++;
                task.Owners = [new() { Owner = owner }];
                task.StateId = extStateHandler.GetInternalStateId(ExtStates.ExtReqInitialized) ?? 0;
            }
            return TaskList;
        }

        private string ConstructComment(ModellingConnection conn)
        {
            string comment = userConfig.ModModelledMarker + conn.Id.ToString();
            if(conn.IsCommonService)
            {
                comment += ", ComSvc";
            }
            if (conn.ExtraConfigs.Count > 0 || conn.ExtraConfigsFromInterface.Count > 0)
            {
                comment += ", " + userConfig.GetText("impl_instructions") + ": " +
                    string.Join(", ", conn.ExtraConfigs.ConvertAll(x => x.Display()).Concat(conn.ExtraConfigsFromInterface.ConvertAll(x => x.Display())));
            }
            return comment;
        }
    }
}
