using Json.NetMF;
using System;
using System.Collections;
using System.Text;

namespace ThingsBoardDotNet
{
    public class TBAttributesResponse
    {
        [JsonPropertyName("Client")]
        public Hashtable AttributesClient;
        [JsonPropertyName("Shared")]
        public Hashtable AttributesShared;

        [JsonIgnore]
        public int ResponseId;

        public TBAttributesResponse(object Client, object Shared, int RpcRequestId)
        {
            AttributesClient = (Hashtable)Client;
            AttributesShared = (Hashtable)Shared;
            ResponseId = RpcRequestId;
        }

        public override string ToString()
        {
            string printOut="";

            if (AttributesClient != null)
            {
                foreach(DictionaryEntry attr in AttributesClient)
                {
                    printOut += attr.Key + ":" + attr.Value;
                }
            }

            if (AttributesShared != null)
            {
                foreach (DictionaryEntry attr in AttributesShared)
                {
                    printOut += attr.Key + ":" + attr.Value;
                }
            }

            return printOut;
        }
    }
}
