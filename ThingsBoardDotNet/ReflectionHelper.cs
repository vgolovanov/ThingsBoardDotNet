using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ThingsBoardDotNet
{
    public static class ReflectionHelper
    {
        private static Hashtable calledMethods;
        private static Type calledType;

        public static void FindRpcMethods(Type type)
        {
            calledType = type;
            calledMethods = new Hashtable();

            MethodInfo[] methods = calledType.GetMethods();
                
            foreach (MethodInfo info in methods)
            {
#if NETCORE
                var attrs = info.GetCustomAttributes();
#else
                var attrs = info.GetCustomAttributes(true);
#endif
                string customAttrName = "";
                foreach (var attr in attrs)
                {
                    if (attr.GetType().Name == "RpcNameAttribute")
                    {
                        customAttrName = (attr as RpcNameAttribute).Name;
                        break;
                    }
                }
                if (customAttrName != "")
                {
                    calledMethods.Add(customAttrName, info.Name);
                }
            }
        }

        public static void InvokeRpcMethod(TBRpcRequest rpcRequest)
        {
            string ssss = rpcRequest.RpcMethod;

            string rpcMethod = (string)calledMethods[(object)rpcRequest.RpcMethod];

            if (rpcMethod != null)
            {
                calledType.GetMethod(rpcMethod).Invoke(null, new object[] { rpcRequest });
            }
        }
    }
}
