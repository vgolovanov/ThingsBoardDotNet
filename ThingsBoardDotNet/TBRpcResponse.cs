using Json.NetMF;
using System;
using System.Collections;
using System.Text;

namespace ThingsBoardDotNet
{
    public class TBRpcResponse
    {
        public int RpcResponsetId;
        private Hashtable jsonValues = new Hashtable();
        private string JsonString;
        private object JsonObject;
        
        public TBRpcResponse(int RpcResponsetId)
        {
            this.RpcResponsetId = RpcResponsetId;
        }
    
        public void Add(object ReturnKey, object ReturnValue)
        {
            jsonValues.Add(ReturnKey, ReturnValue);
        }

        public void Add(string JsonResponse)
        {
            JsonString = JsonResponse;
        }

        public void Add(object ReturnValue)
        {
            JsonObject = ReturnValue;
        }
    
        public string ToJson()
        {
            if (JsonString != null)
            {
                return JsonString;
            }
            else if (JsonObject != null)
            {
                return JsonSerializer.SerializeObject(JsonObject);
            }
            else if (jsonValues.Count > 0)
            {
                return JsonSerializer.SerializeObject(jsonValues);
            }
            return null;
        }
    }
}