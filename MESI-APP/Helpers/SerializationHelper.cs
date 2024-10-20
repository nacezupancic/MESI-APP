using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MESI_APP.Helpers
{
    public static class SerializationHelper
    {
        public static async Task<string> PrepareJsonString(object serializationObject)
        {
            string jsonString;
            using (var ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(ms, serializationObject);
                ms.Position = 0;
                using var reader = new StreamReader(ms);
                jsonString = await reader.ReadToEndAsync();
            }
            return jsonString;
        }

        public static async Task<T> DeserializeJsonString<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T returnValue = await JsonSerializer.DeserializeAsync<T>(ms);
                return returnValue;
            }
        }
    }
}
