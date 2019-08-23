using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIRMSBot2
{
    class BotConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string SourceOffice { get; set; }
        public string OriginOffice { get; set; }
        public string SecurityClassification { get; set; }
        public string WrittenBy { get; set; }


        public void Save()
        {
            var configLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BotConfig.json");

            File.WriteAllText(configLocation, JsonConvert.SerializeObject(this));
        }

        public static BotConfig Get()
        {

            var configLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BotConfig.json");
            if (!File.Exists(configLocation))
                return new BotConfig();

            return JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(configLocation));


        }
    }
}
