using MESI_APP.Helpers;
using MESI_APP.Models.SaveableCanvasModels;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace MESI_APP.Services
{
    public class SettingsService
    {
        private readonly FileManagementService _fileManagementService;
        public SettingsService(FileManagementService fileManagementService)
        {
            _fileManagementService = fileManagementService;
        }
        public async Task<Dictionary<string, JsonElement>> GetSettings(bool init = false) {
            if (!init)
            {
                bool fileExists = _fileManagementService.FileExists(Constants.LatestConfig);
                if (fileExists) { 
                    var settings = await GetSettingsFromFilepath(Constants.LatestConfig);
                    if (settings != null && settings.Count>0) {
                        return settings;
                    }
                }
            }
            return await GetSettingsFromFilepath(Constants.InitialConfigFile);
        }

        private async Task<Dictionary<string, JsonElement>> GetSettingsFromFilepath(string settingsPath) {
            var fileContent = await _fileManagementService.GetFileContent(settingsPath);
            var deserializedDict = await SerializationHelper.DeserializeJsonString<Dictionary<string, JsonElement>>(fileContent);
            return deserializedDict;
        }

        public async Task SaveSettings(Dictionary<string, object> settings) {
            string jsonString = await SerializationHelper.PrepareJsonString(settings);
            await _fileManagementService.SaveToFile(jsonString, Constants.LatestConfig);
        }                
    }
}
