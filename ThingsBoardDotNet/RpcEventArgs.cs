using System;
using System.Collections;

#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.ThingsBoard
{
#else
namespace dotNETCore.ThingsBoard
{
#endif  
    public class RpcEventArgs
    {
        public TBRpcRequest RpcRequest { get; set; }
        public TBRpcResponse RpcResponse { get; set; }
        public TBAttributesResponse AttributesResponse { get; set; }
        public RpcError RpcError { get; set; }

        public RpcEventArgs()
        {
        }

        public RpcEventArgs(TBRpcRequest RpcRequest)
        {
            this.RpcRequest = RpcRequest;
        }

        public RpcEventArgs(TBRpcResponse RpcResponse)
        {
            this.RpcResponse = RpcResponse;
        }

        public RpcEventArgs(RpcError RpcError)
        {
            this.RpcError = RpcError;
        }

        public RpcEventArgs(TBAttributesResponse AttributesResponse)
        {
            this.AttributesResponse = AttributesResponse;
        }      
    }
}
