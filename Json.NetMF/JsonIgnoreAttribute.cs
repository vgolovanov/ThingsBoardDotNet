using System;

namespace Json.NetMF
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonIgnoreAttribute : Attribute
    {
    }
}
