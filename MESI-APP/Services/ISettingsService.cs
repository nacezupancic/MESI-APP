using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MESI_APP.Services
{
    public interface ISettingsService
    {
        Task<Dictionary<string, JsonElement>> GetSettings(bool init = false);
        Task SaveSettings(Dictionary<string, object> settings);
    }
}
