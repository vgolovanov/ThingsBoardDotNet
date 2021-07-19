using System;

#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.ThingsBoard
{
#else
namespace dotNETCore.ThingsBoard
{
#endif  
    public class RpcNameAttribute : Attribute
    {
        private readonly string name;

        public RpcNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}
