using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SnekBot.Data
{
    public class DataService
    {
        private const string ConfigPath = "Data/Config.json";
        private const string DataPath = "Data/Data.json";


        private static readonly Dictionary<string, string> Configuration;
        public static DataObject Data;


        static DataService()
        {
            var jsonConfig = File.ReadAllText(ConfigPath);
            var configs = JsonConvert.DeserializeObject<dynamic>(jsonConfig);

            var jsonData = File.ReadAllText(DataPath);
            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);

            Configuration = configs.ToObject<Dictionary<string,string>>();
            try
            {
                Data = data.ToObject<DataObject>();
            }
            catch(Exception e)
            {
                Data = new DataObject();
            }
        }

        public static string GetConfig(string key)
        {
            return Configuration.ContainsKey(key) ? Configuration[key] : "";
        }

        public static async void OnShutdown()
        {
            File.Delete(DataPath);
            File.Create(DataPath);

            var json = JsonConvert.SerializeObject(Data);
            await File.WriteAllTextAsync(json, DataPath).ConfigureAwait(false);
        }

        public static List<ProtectionFlags> GetChannelProtectionFlags(ulong channelId)
        {
            try
            {
                return Data.ProtectedChannels.First(c => c.ChannelId == channelId).ProtectionFlags;
            }
            catch
            {
                return null;
            }
        }

        public static Role GetMutedRole()
        {
            return Data.Roles.First(r => r.RoleType==RoleType.Mute)?? null;
        }

    }
}
