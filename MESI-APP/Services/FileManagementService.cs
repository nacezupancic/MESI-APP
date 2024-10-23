using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Services
{
    public class FileManagementService : IFileManagementService
    {
        private readonly ILoggerService _logger;
        public FileManagementService(ILoggerService loggerService) {
            _logger = loggerService;
        }

        public bool FileExists(string filepath) => File.Exists(filepath);

        public async Task<string> GetFileContent(string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new ArgumentException("Filepath is null or empty");
            }

            if (!FileExists(filePath)) {
                throw new ArgumentException("File does not exist.", filePath);
            }
            try
            {

                return await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> SaveToFile(string content, string filePath) {

            try
            {
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch (Exception e) {
                _logger.Error($"Error saving to file: {e.Message}");
                return false;
            }
        }
    }
}
