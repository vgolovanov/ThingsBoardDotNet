using System;
using System.Text;

namespace Json.NetMF
{
    public class JsonPropertyNameAttribute : Attribute
    {
        private readonly string name;

        public JsonPropertyNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}
