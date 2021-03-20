using System;

namespace ThingsBoardDotNet
{
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
