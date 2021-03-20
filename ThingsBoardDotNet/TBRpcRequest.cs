
using Json.NetMF;
using System;
using System.Text;

namespace ThingsBoardDotNet
{   
    public class TBRpcRequest
    {
        [JsonPropertyName("method")]
        public string RpcMethod;
        [JsonPropertyName("params")]
        public object RpcParams;
        [JsonIgnore]
        public int RpcRequestId;

        public TBRpcRequest()
        { }
            
        public TBRpcRequest(string RpcMethod, object RpcParams,int RpcRequestId)
        {
            this.RpcMethod = RpcMethod;
            this.RpcParams = RpcParams;
            this.RpcRequestId = RpcRequestId;
        }

        public TBRpcRequest(string RpcMethod, int RpcRequestId)
        {
            this.RpcMethod = RpcMethod;        
            this.RpcRequestId = RpcRequestId;
        }

        public TBRpcRequest(string RpcMethod)
        {
            this.RpcMethod = RpcMethod;          
        }

        public string ToJson()
        {
            if (RpcParams == null)
            {
                RpcParams = "{}";
            }

            if (RpcParams.GetType() == typeof(string)) 
            {
                if ((string)RpcParams == "")
                {
                    RpcParams = "{}";
                }                
            }
                       
            return JsonSerializer.SerializeObject(this);           
        }
    }
}
