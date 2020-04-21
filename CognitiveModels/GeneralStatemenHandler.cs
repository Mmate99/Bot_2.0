// <auto-generated>
// Code generated by LUISGen GeneralStatementLUIS.json -cs Luis.GeneralStatemenHandler -o 
// Tool github: https://github.com/microsoft/botbuilder-tools
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;

namespace CoreBot
{
    public partial class GeneralStatemenHandler: IRecognizerConvert
    {
        [JsonProperty("text")]
        public string Text;

        [JsonProperty("alteredText")]
        public string AlteredText;

        public enum Intent {
            None, 
            Statement
        };
        [JsonProperty("intents")]
        public Dictionary<Intent, IntentScore> Intents;

        public class _Entities
        {
            // Simple entities
            public string[] SearchKey;

            public string[] bigger;

            public string[] negation;

            public string[] smaller;

            public string[] value;

            // Built-in entities
            public DateTimeSpec[] datetime;

            public string[] personName;

            // Composites
            public class _InstanceValue
            {
                public InstanceData[] datetime;
                public InstanceData[] personName;
                public InstanceData[] value;
                public InstanceData[] negation;
                public InstanceData[] bigger;
                public InstanceData[] smaller;
            }
            public class ValueClass
            {
                public DateTimeSpec[] datetime;
                public string[] personName;
                public string[] value;
                public string[] negation;
                public string[] bigger;
                public string[] smaller;
                [JsonProperty("$instance")]
                public _InstanceValue _instance;
            }
            public ValueClass[] Value;

            // Instance
            public class _Instance
            {
                public InstanceData[] SearchKey;
                public InstanceData[] Value;
                public InstanceData[] bigger;
                public InstanceData[] datetime;
                public InstanceData[] negation;
                public InstanceData[] personName;
                public InstanceData[] smaller;
                public InstanceData[] value;
            }
            [JsonProperty("$instance")]
            public _Instance _instance;
        }
        [JsonProperty("entities")]
        public _Entities Entities;

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> Properties {get; set; }

        public void Convert(dynamic result)
        {
            var app = JsonConvert.DeserializeObject<GeneralStatemenHandler>(JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) TopIntent()
        {
            Intent maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }
            return (maxIntent, max);
        }
    }
}
