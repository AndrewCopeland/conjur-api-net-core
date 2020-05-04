using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ConjurClient.Resources
{
    public class Resource
    {
        public JObject RawJson { get; set; }
        public String Account { get; set; }
        public ResourceKind Kind { get; set; }
        public String Id {get; set;}
        public String FullId { get; set; }
        public Dictionary<string, string> Annotations { get; set; }

        public Resource(JObject json)
        {
            Annotations = new Dictionary<string, string>();
            RawJson = json;
            FullId = json.SelectToken("id").ToString();

            // Parse FullID into Account, Kind and Id 
            string[] parts = FullId.Split(":", 3);
            Account = parts[0];
            // Convert kind string into enum
            Kind = (ResourceKind)Enum.Parse(typeof(ResourceKind), parts[1]);
            Id = parts[2];

            // Parse Annotations into Dictionary
            JArray annotations = RawJson.SelectToken("annotations").ToObject<JArray>();
            foreach (JObject a in annotations)
            {
                IList<string> keys = a.Properties().Select(p => p.Name).ToList();
                foreach (string key in keys)
                {
                    string value = a.GetValue(key).ToString();
                    Annotations.Add(key, value);
                }
            }
        }
    }
}
