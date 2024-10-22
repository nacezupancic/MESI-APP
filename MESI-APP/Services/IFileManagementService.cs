using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Services
{
    public interface IFileManagementService
    {
        bool FileExists(string filepath);
        Task<string> GetFileContent(string filePath);
        Task<bool> SaveToFile(string content, string filePath);
    }
}
