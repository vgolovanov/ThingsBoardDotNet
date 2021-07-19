using System;
using System.Text;

#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.ThingsBoard
{
#else
namespace dotNETCore.ThingsBoard
{
#endif  
    public class RpcError
    {
        public string Error;
        public int RpcResponsetId;

        public RpcError(string Error, int RpcResponsetId)
        {
            this.Error = Error;
            this.RpcResponsetId = RpcResponsetId;
        }
    }
}
