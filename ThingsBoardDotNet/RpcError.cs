using System;
using System.Text;

namespace ThingsBoardDotNet
{
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
