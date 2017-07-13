using System;
using System.IO;
using Newtonsoft.Json;

namespace Nero {
    public class Configuration {
        [JsonIgnore]
        public static string FileName { get; set; } = "config/configuration.json";
        public ulong[] Owners { get; set; }
        public string Prefix { get; set; } = "!n ";
        public string Token { get; set; } = "";
        
        public string FFLogsKey { get; set; } = "";
    

        public static void EnsureExists() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file)) {
                string path = Path.GetDirectoryName(file);
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var config = new Configuration();

                Console.WriteLine("Please enter your discord token: ");
                string token = Console.ReadLine();
                config.Token = token;

                Console.WriteLine("Please enter your fflogs api key: ");
                string key = Console.ReadLine();
                config.FFLogsKey = key;

                config.SaveJson();
            }
        }   

        public void SaveJson() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, ToJson());
        }

        public static Configuration Load() {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }

        public string ToJson() 
            => JsonConvert.SerializeObject(this, Formatting.Indented);
        

    }

}