using FWO.Logging;
using FWO.Api.Client;
using FWO.Api.Client.Queries;
using FWO.GlobalConstants;
using FWO.Api.Data;
using FWO.Config.Api;
using System.Text.Json;


namespace FWO.Middleware.Server
{
    /// <summary>
    /// Class handling the Area Subnet Data Import
    /// </summary>
    public class AreaSubnetDataImport : DataImportBase
    {
        private List<ModellingImportAreaData> importedAreas = [];
        private List<ModellingNetworkArea> existingAreas = [];


        /// <summary>
        /// Constructor for Area Subnet Data Import
        /// </summary>
        public AreaSubnetDataImport(ApiConnection apiConnection, GlobalConfig globalConfig) : base (apiConnection, globalConfig)
        {}

        /// <summary>
        /// Run the Area Subnet Data Import
        /// </summary>
        public async Task<bool> Run()
        {
            if(!RunImportScript(globalConfig.ImportSubnetDataPath + ".py"))
            {
                Log.WriteInfo("Import Area Subnet Data", $"Script {globalConfig.ImportSubnetDataPath}.py failed but trying to import from existing file.");
            }

            int successCounter = 0;
            int failCounter = 0;
            int deleteCounter = 0;
            int deleteFailCounter = 0;

            List<string> importfilePathAndNames = JsonSerializer.Deserialize<List<string>>(globalConfig.ImportSubnetDataPath) ?? throw new Exception("Config Data could not be deserialized.");

            foreach (string file in importfilePathAndNames)
            {
                try
                {
                    importFile = "{\r\n   \"areas\": [\r\n      {\r\n         \"name\": \"MGT\",\r\n         \"id_string\": \"NA91\",\r\n         \"subnets\": [\r\n            {\r\n               \"ip\": \"1.2.3.0/24\",\r\n               \"name\": \"Netz1\"\r\n            },\r\n            {\r\n               \"ip\": \"2.2.2.2\",\r\n               \"name\": \"Net2\"\r\n            },\r\n            {\r\n               \"ip\": \"3.3.3.3-3.3.3.4\",\r\n               \"name\": \"Netz3\"\r\n            }\r\n         ]\r\n      },\r\n      {\r\n         \"name\": \"DC\",\r\n         \"id_string\": \"NA50\",\r\n         \"subnets\": [\r\n            {\r\n               \"ip\": \"10.121.254.128/27\",\r\n               \"name\": \"Netz5\"\r\n            },\r\n            {\r\n               \"ip\": \"10.121.254.192/27\",\r\n               \"name\": \"Netz1\"\r\n            },\r\n            {\r\n               \"ip\": \"10.129.254.224/27\",\r\n               \"name\": \"Netz1\"\r\n            },\r\n            {\r\n               \"ip\": \"10.122.28.1/23\",\r\n               \"name\": \"Console\"\r\n            }\r\n         ]\r\n      },\r\n      {\r\n         \"name\": \"Area172\",\r\n         \"id_string\": \"NAxx\",\r\n         \"subnets\": [\r\n            {\r\n               \"ip\": \"172.0.0.0/7\",\r\n               \"name\": \"all172\"\r\n            }\r\n         ]\r\n      },\r\n      {\r\n         \"name\": \"Area10\",\r\n         \"id_string\": \"NA10\",\r\n         \"subnets\": [\r\n            {\r\n               \"ip\": \"10.0.0.0/7\",\r\n               \"name\": \"all10\"\r\n            }\r\n         ]\r\n      }\r\n   ]\r\n}\r\n";
                    
                    //ReadFile(globalConfig.ImportSubnetDataPath + ".json");

                    ModellingImportNwData? importedNwData = JsonSerializer.Deserialize<ModellingImportNwData>(importFile) ?? throw new Exception("File could not be parsed.");

                    if (importedNwData != null && importedNwData.Areas != null)
                    {
                        importedAreas = importedNwData.Areas;
                        existingAreas = await apiConnection.SendQueryAsync<List<ModellingNetworkArea>>(ModellingQueries.getAreas);
                        foreach (var incomingArea in importedAreas)
                        {
                            if (await SaveArea(incomingArea))
                            {
                                ++successCounter;
                            }
                            else
                            {
                                ++failCounter;
                            }
                        }
                        foreach (var existingArea in existingAreas)
                        {
                            if (importedAreas.FirstOrDefault(x => x.Name == existingArea.Name) == null)
                            {
                                if (await DeleteArea(existingArea))
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
                    else
                    {
                        Log.WriteInfo("Import Area Subnet Data", $"No Area Data found in {importFile} No changes done. ");
                    }
                }
                catch (Exception exc)
                {
                    Log.WriteError("Import Area Subnet Data", $"File could not be processed.", exc);
                }
            }

            Log.WriteInfo("Import Area Subnet Data", $"Imported {successCounter} areas, {failCounter} failed. Deleted {deleteCounter} areas, {deleteFailCounter} failed.");
            return true;
        }

        private async Task<bool> SaveArea(ModellingImportAreaData incomingArea)
        {
            try
            {
                ModellingNetworkArea? existingArea = existingAreas.FirstOrDefault(x => x.Name == incomingArea.Name);
                if(existingArea == null)
                {
                    await NewArea(incomingArea);
                }
                else
                {
                    await UpdateArea(incomingArea, existingArea);
                }
            }
            catch (Exception exc)
            {
                Log.WriteError("Import Area Subnet Data", $"Area {incomingArea.Name}({incomingArea.IdString}) could not be processed.", exc);
                return false;
            }
            return true;
        }

        private async Task NewArea(ModellingImportAreaData incomingArea)
        {
            var AreaVar = new
            { 
                name = incomingArea.Name,
                idString = incomingArea.IdString,
                creator = GlobalConst.kImportAreaSubnetData
            };
            ReturnId[]? areaIds = (await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.newArea, AreaVar)).ReturnIds;
            if (areaIds != null)
            {
                foreach(var subnet in incomingArea.Subnets)
                {
                    subnet.Ip = GlobalFunc.IpOperations.SanitizeIp(subnet.Ip);

                    (string Start, string End) ip = GlobalFunc.IpOperations.CidrToRangeString(subnet.Ip);
                    subnet.IpEnd = GlobalFunc.IpOperations.SanitizeIp(ip.End);

                    var SubnetVar = new
                    {
                        name = subnet.Name,
                        ip = subnet.Ip,
                        ipEnd = subnet.IpEnd,
                        importSource = GlobalConst.kImportAreaSubnetData
                    };
                    ReturnId[]? subnetIds= (await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.newAreaSubnet, SubnetVar)).ReturnIds;
                    if (subnetIds != null)
                    {
                        var Vars = new
                        {
                            nwObjectId = subnetIds[0].NewId,
                            nwGroupId = areaIds[0].NewId
                        };
                        await apiConnection.SendQueryAsync<ReturnId>(ModellingQueries.addNwObjectToNwGroup, Vars);
                    }
                }
            }
        }

        private async Task UpdateArea(ModellingImportAreaData incomingArea, ModellingNetworkArea existingArea)
        {
            if(existingArea.IsDeleted)
            {
                await ReactivateArea(existingArea);
            }
            List<ModellingImportAreaSubnets> subnetsToAdd = new (incomingArea.Subnets);
            List<NetworkSubnetWrapper> subnetsToDelete = new (existingArea.Subnets);
            foreach(var existingSubnet in existingArea.Subnets)
            {
                foreach(var incomingSubnet in incomingArea.Subnets)
                {
                    if (incomingSubnet.Name == existingSubnet.Content.Name && incomingSubnet.Ip == existingSubnet.Content.Ip && 
                        (incomingSubnet.IpEnd == existingSubnet.Content.IpEnd) || (incomingSubnet.IpEnd == "" && existingSubnet.Content.Ip == existingSubnet.Content.IpEnd))
                    {
                        subnetsToAdd.Remove(incomingSubnet);
                        subnetsToDelete.Remove(existingSubnet);
                    }
                }
            }
            foreach(var subnet in subnetsToDelete)
            {
                await apiConnection.SendQueryAsync<NewReturning>(OwnerQueries.deleteAreaSubnet, new { id = subnet.Content.Id });
            }
            foreach(var subnet in subnetsToAdd)
            {
                subnet.Ip = GlobalFunc.IpOperations.SanitizeIp(subnet.Ip);

                (string Start, string End) ip = GlobalFunc.IpOperations.CidrToRangeString(subnet.Ip);
                subnet.IpEnd = GlobalFunc.IpOperations.SanitizeIp(ip.End);

                var SubnetVar = new
                {
                    name = subnet.Name,
                    ip = subnet.Ip,
                    ipEnd = subnet.IpEnd,
                    importSource = GlobalConst.kImportAreaSubnetData
                };
                ReturnId[]? subnetIds= (await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.newAreaSubnet, SubnetVar)).ReturnIds;
                if (subnetIds != null)
                {
                    var Vars = new
                    {
                        nwObjectId = subnetIds[0].NewId,
                        nwGroupId = existingArea.Id,
                    };
                    await apiConnection.SendQueryAsync<ReturnId>(ModellingQueries.addNwObjectToNwGroup, Vars);
                }
            }
        }

        private async Task<bool> DeleteArea(ModellingNetworkArea area)
        {
            try
            {
                // if(await CheckAreaInUse(area))
                // {
                await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.setAreaDeletedState, new { id = area.Id , deleted = true});
                await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.removeSelectedNwGroupObjectFromAllApps, new { nwGroupId = area.Id }); 
                // }
                // else
                // {
                //     await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.deleteNwGroup, new { id = area.Id });
                // }
            }
            catch (Exception exc)
            {
                Log.WriteError("Import Area Subnet Data", $"Outdated Area {area.Name} could not be deleted.", exc);
                return false;
            }
            return true;
        }

        private async Task ReactivateArea(ModellingNetworkArea area)
        {
            try
            {
                await apiConnection.SendQueryAsync<NewReturning>(ModellingQueries.setAreaDeletedState, new { id = area.Id , deleted = false});
            }
            catch (Exception exc)
            {
                Log.WriteError("Reactivate Area", $"Area {area.Name}({area.IdString}) could not be reactivated.", exc);
            }
        }

        // private async Task<bool> CheckAreaInUse(ModellingNetworkArea area)
        // {
        //     try
        //     {
        //         // List<ModellingConnection> foundConnections = await apiConnection.SendQueryAsync<List<ModellingConnection>>(ModellingQueries.getConnectionIdsForNwGroup, new { id = area.Id });
        //         // if (foundConnections.Count == 0)
        //         //  {
        //         //     // Todo: further checks: appServer in area ? in any selection list ??
        //         //     if ()
        //         //     {
        //         //         return false;
        //         //     }
        //         // }
        //         return true;
        //     }
        //     catch (Exception)
        //     {
        //         return true;
        //     }
        // }
    }
}
