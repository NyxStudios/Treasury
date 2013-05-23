using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TShockAPI;

namespace Treasury
{
    public class Config
    {
        [Description("Valid types are \"sqlite\" and \"mysql\"")] public string StorageType = "sqlite";
        [Description("The MySQL hostname and port to direct connections to")] public string MySqlHost = "localhost:3306";
        [Description("Database name to connect to")] public string MySqlDbName = "";
        [Description("Database username to connect with")] public string MySqlUsername = "";
        [Description("Database password to connect with")] public string MySqlPassword = "";

        /// <summary>
        /// Reads a configuration file from a given path
        /// </summary>
        /// <param name="path">string path</param>
        /// <returns>ConfigFile object</returns>
        public static ConfigFile Read(string path)
        {
            if (!File.Exists(path))
                return new ConfigFile();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        /// <summary>
        /// Reads the configuration file from a stream
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>ConfigFile object</returns>
        public static ConfigFile Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<ConfigFile>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// Writes the configuration to a given path
        /// </summary>
        /// <param name="path">string path - Location to put the config file</param>
        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }

        /// <summary>
        /// Writes the configuration to a stream
        /// </summary>
        /// <param name="stream">stream</param>
            public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }
    }
}
