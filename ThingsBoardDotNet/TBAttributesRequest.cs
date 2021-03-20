using Json.NetMF;
using System;
using System.Collections;


namespace ThingsBoardDotNet
{
    public class TBAttributesRequest
    {
        public string clientKeys { get; set; }
        public string sharedKeys { get; set; }

        private ArrayList sharedKeysArray = new ArrayList();
        private ArrayList clientKeysArray = new ArrayList();

        public void AddSharedAttribute(Object SharedKey)
        {
            sharedKeysArray.Add(SharedKey);
        }

        public void AddClientAttribute(Object ClientKey)
        {
            clientKeysArray.Add(ClientKey);
        }

        public String ToJson()
        {

            if (sharedKeysArray.Count > 0)
            {
                sharedKeys = "";

                foreach (string sharedKey in sharedKeysArray)
                {
                    sharedKeys += sharedKey + ",";
                }
#if NETCORE
                sharedKeys = sharedKeys.Remove(sharedKeys.Length - 1);
#else
                sharedKeys = new string(sharedKeys.ToCharArray(), 0, sharedKeys.Length - 1);
#endif
            }
            else
            {
                sharedKeys = "{}";
            }

            if (clientKeysArray.Count > 0)
            {
                clientKeys = "";

                foreach (string clientKey in clientKeysArray)
                {
                    clientKeys += clientKey + ",";
                }

#if NETCORE                
                clientKeys = clientKeys.Remove(clientKeys.Length - 1);
#else
                clientKeys = new string(clientKeys.ToCharArray(),0, clientKeys.Length - 1);
#endif
            }
            else
            {
                clientKeys = "{}";
            }


            return JsonSerializer.SerializeObject(this);
        }
    }    
}
