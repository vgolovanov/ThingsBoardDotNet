using Json.NetMF;
using System;
using System.Collections;
using System.Text;

#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.ThingsBoard
{
#else
namespace dotNETCore.ThingsBoard
{
#endif  
    public class TBTelemetry
    {       
        [JsonPropertyName("ts")]
        public long TimeStamp;
        [JsonPropertyName("values")]
        public object TelemetryValues;

        private Hashtable telemetryData = new Hashtable();
        private string JsonString;

        public TBTelemetry()
        {
        }

        public void Add(object TelemetryKey, object TelemetryValue)
        {
            telemetryData.Add(TelemetryKey, TelemetryValue);
        }

        public void Add(string JsonTelemetryString)
        {
            JsonString = JsonTelemetryString;
        }

        public string ToJson(Boolean TimeStamp = false)
        {
            GetTimeStamp();

            if (JsonString != null)
            {
                return JsonString;
            }         
            else if (telemetryData.Count > 0 && TimeStamp==true)
            {
                this.TelemetryValues = telemetryData;
                return JsonSerializer.SerializeObject(this);      
            }
            else if (telemetryData.Count > 0 && TimeStamp==false)
            { 
                return JsonSerializer.SerializeObject(telemetryData);
            }
            return null;
        }

        private void GetTimeStamp()
        {
#if NETCORE
            ////https://www.tutorialspoint.com/how-to-get-the-unix-timestamp-in-chash
           // TimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(); //GMT

            //TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
            //TimeSpan unixTicks = new TimeSpan(DateTime.Now.Ticks) - epochTicks;
            //Int32 unixTimestamp = (Int32)unixTicks.TotalSeconds;    //Current Time Zone

            TimeStamp = (long)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds; //Current Time Zone          
#else
            
            TimeStamp = (long)(DateTime.Today.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
#endif
        }
    }
}
