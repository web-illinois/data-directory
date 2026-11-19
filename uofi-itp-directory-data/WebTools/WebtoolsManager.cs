using System.Text.Json.Nodes;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.WebTools {
    public class WebtoolsManager(DirectoryRepository? directoryRepository, JsonReader? jsonReader) {
        private readonly JsonReader jsonReader = jsonReader ?? throw new ArgumentNullException("jsonReader");
        private readonly DirectoryRepository _directoryRepository = directoryRepository ?? throw new ArgumentNullException("directoryRepository");

        private Dictionary<ToolkitTypeEnum, string> nodeName = new() {
            { ToolkitTypeEnum.Person, "user_id" },
            { ToolkitTypeEnum.Department, "home_department_code" },
            { ToolkitTypeEnum.Service, "id" }
        };

        public async Task<string> UpdateToolkitData(ToolkitTypeEnum toolkitTypeEnum) {
            var toolkitLog = await _directoryRepository.ReadAsync(x => x.ToolkitLogs.OrderByDescending(x => x.LastUpdated).FirstOrDefault(t => t.ToolkitType == toolkitTypeEnum && t.IsActive));

            var toolkitLogNew = toolkitLog != null ? toolkitLog.Advance() : ToolkitLog.Start(toolkitTypeEnum);
            if (toolkitLogNew.Page == 0) {
                var schema = await jsonReader.GetSchema(toolkitTypeEnum);
                if (toolkitLogNew.Schema == "") {
                    toolkitLogNew.Schema = schema;
                } else if (toolkitLogNew.Schema != schema) {
                    return "schema mismatch";
                }
            }
            var toolkitData = await jsonReader.GetJson(toolkitTypeEnum, toolkitLogNew.Page, toolkitLogNew.Timestamp);
            var keys = new List<string>();
            foreach (var item in toolkitData.Item2) {
                var key = JsonNode.Parse(item)?[nodeName[toolkitTypeEnum]]?.ToString() ?? "";
                if (key == "" || keys.Contains(key)) {
                    continue;
                }
                keys.Add(key);
                switch (toolkitTypeEnum) {
                    case ToolkitTypeEnum.Person:
                        var existingToolkitPerson = await _directoryRepository.ReadAsync<ToolkitPerson>(x => x.ToolkitPeople.FirstOrDefault(t => t.Key == key));
                        if (existingToolkitPerson != null) {
                            existingToolkitPerson.Json = item;
                            existingToolkitPerson.IsActive = true;
                            existingToolkitPerson.LastUpdated = DateTime.Now;
                            _ = await _directoryRepository.UpdateAsync(existingToolkitPerson);
                        } else {
                            var toolkitPerson = new ToolkitPerson {
                                Key = key,
                                Json = item,
                                IsActive = true
                            };
                            _ = await _directoryRepository.CreateAsync(toolkitPerson);
                        }
                        toolkitLogNew.Log += $";{key}";
                        break;
                    case ToolkitTypeEnum.Department:
                        var existingToolkitDepartment = await _directoryRepository.ReadAsync<ToolkitDepartment>(x => x.ToolkitDepartments.FirstOrDefault(t => t.Key == key));
                        if (existingToolkitDepartment != null) {
                            existingToolkitDepartment.Json = item;
                            existingToolkitDepartment.IsActive = true;
                            existingToolkitDepartment.LastUpdated = DateTime.Now;
                            _ = await _directoryRepository.UpdateAsync(existingToolkitDepartment);
                        } else {
                            var toolkitDepartment = new ToolkitDepartment {
                                Key = key,
                                Json = item,
                                IsActive = true
                            };
                            _ = await _directoryRepository.CreateAsync(toolkitDepartment);
                        }
                        toolkitLogNew.Log += $";{key}";
                        break;
                    case ToolkitTypeEnum.Service:
                        var existingToolkitService = await _directoryRepository.ReadAsync<ToolkitService>(x => x.ToolkitServices.FirstOrDefault(t => t.Key == key));
                        if (existingToolkitService != null) {
                            existingToolkitService.Json = item;
                            existingToolkitService.IsActive = true;
                            existingToolkitService.LastUpdated = DateTime.Now;
                            _ = await _directoryRepository.UpdateAsync(existingToolkitService);
                        } else {
                            var toolkitService = new ToolkitService {
                                Key = key,
                                Json = item,
                                IsActive = true
                            };
                            _ = await _directoryRepository.CreateAsync(toolkitService);
                        }
                        break;
                }
            }
            toolkitLogNew.Log = $"{keys.Count} items: {toolkitLogNew.Log.TrimStart(';')}";
            toolkitLogNew.Success = true;
            toolkitLogNew.IsLast = !toolkitData.Item1;
            var result = await _directoryRepository.CreateAsync(toolkitLogNew);
            toolkitLogNew.Success = result == 1;
            return $"{keys.Count} items: {toolkitLogNew.ToString()}";
        }
    }
}
