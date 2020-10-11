using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GPTChatBot
{
    /*Hacky little config class to let me update config on the fly with commands*/
    public static class ConfigMan
    {
        public static JObject Config;
        public static string Path;
        public static async void Build(string path) 
        {
            Path = path + "\\appsettings.json";
            string json = File.ReadAllText(Path);
            Config = (JObject)JsonConvert.DeserializeObject(json);
        }
        public static async void Update(string key, string value, bool write = true) 
        {
            Config[key] = value;
            if (write)
            {
                /*This will not maintain in debug since the executing dir is different, but will work when deployed*/
                string json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(Path, json);
            }
        }
        public static string Get(string key) 
        {
            foreach (KeyValuePair<string, JToken> kvp in Config)
            {
                if (key == kvp.Key)
                {
                    return kvp.Value.ToString();
                }
            }
            return string.Empty;
        }
    }
}
