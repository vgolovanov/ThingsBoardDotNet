using Json.NetMF;
using System;
using System.Collections;
using System.Text;

namespace ThingsBoardDotNet
{
    public class TBAttributes
    {
        private Hashtable attributesData = new Hashtable();
        private string JsonString;

        public void Add(string TelemetryKey, object TelemetryValue)
        {
            attributesData.Add(TelemetryKey, TelemetryValue);
        }

        public void Add(string JsonTelemetryString)
        {
            JsonString = JsonTelemetryString;
        }

        public string ToJson()
        {            
            if (JsonString != null)
            {
                return JsonString;
            }
            else if (attributesData.Count > 0 )
            {                
                return JsonSerializer.SerializeObject(attributesData);
            }     
            return null;
        }
    }
}
