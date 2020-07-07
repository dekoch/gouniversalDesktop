using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.IO;

namespace gouniversalDesktop
{
    class config
    {
        public class ConfigFile
        {
            public string Binary { get; set; }
            public string StartPath { get; set; }
        }

        public void Write(string path, ConfigFile cf)
        {
            byte[] b = JsonSerializer.SerializeToUtf8Bytes(cf);

            File.WriteAllBytes(path, b);
        }

        public ConfigFile Read(string path)
        {
            byte[] b = File.ReadAllBytes(path);

            var utf8Reader = new Utf8JsonReader(b);
            return JsonSerializer.Deserialize<ConfigFile>(ref utf8Reader);
        }
    }
}
