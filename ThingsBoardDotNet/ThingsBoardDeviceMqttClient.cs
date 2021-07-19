
using Json.NetMF;
#if (NANOFRAMEWORK_1_0)
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
#else
//using uPLibrary.Networking.M2Mqtt;
//using uPLibrary.Networking.M2Mqtt.Messages;
using M2Mqtt;
using M2Mqtt.Messages;
#endif
using System;
using System.Collections;
using System.Text;




#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.ThingsBoard
{
#else
namespace dotNETCore.ThingsBoard
{
#endif 
    public class ThingsBoardDeviceMqttClient
    {
        public bool Connected = false;
        private MqttClient client;
        private string RPC_RESPONSE_TOPIC = "v1/devices/me/rpc/response/";
        private string RPC_REQUEST_TOPIC = "v1/devices/me/rpc/request/";
        private string ATTRIBUTES_TOPIC = "v1/devices/me/attributes";
        private string ATTRIBUTES_TOPIC_REQUEST = "v1/devices/me/attributes/request/";
        private string ATTRIBUTES_TOPIC_RESPONSE = "v1/devices/me/attributes/response/";
        private string TELEMETRY_TOPIC = "v1/devices/me/telemetry";
        private string CLAIMING_TOPIC = "v1/devices/me/claim";
        private string PROVISION_TOPIC_REQUEST = "/provision/request";
        private string PROVISION_TOPIC_RESPONSE = "/provision/response";
       

        public delegate void RpcEventHandler(object sender, RpcEventArgs e);

        public event RpcEventHandler OnRpcRequestTopic;
        public event RpcEventHandler OnRpcResponseTopic;
        public event RpcEventHandler OnRpcError;
        public event RpcEventHandler OnAttributesResponseTopic;
        
        private MqttQoSLevel QoS;
        private int tbRequestId = 0;
      
        public ThingsBoardDeviceMqttClient()
        {       
        }

        public bool Connect(string Host, string AccessToken, int Port = 1883)
        {
            if (Host == null || AccessToken == null) return false;

            string clientId = Guid.NewGuid().ToString(); ;
            client = new MqttClient(Host, Port, false, null, null, MqttSslProtocols.None);
           
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
            client.ConnectionClosed += Client_ConnectionClosed;
                     
            var result = client.Connect(clientId, AccessToken, null);

            if (result != 0) return false;

            Connected = true;
            
            this.QoS = MqttQoSLevel.AtLeastOnce;

            client.Subscribe(new string[] { ATTRIBUTES_TOPIC }, new MqttQoSLevel[] { QoS });
            client.Subscribe(new string[] { ATTRIBUTES_TOPIC + "/response/+" }, new MqttQoSLevel[] { QoS });
            client.Subscribe(new string[] { RPC_REQUEST_TOPIC + "+" }, new MqttQoSLevel[] { QoS });
            client.Subscribe(new string[] { RPC_RESPONSE_TOPIC + "+" }, new MqttQoSLevel[] { QoS });

            return true;
        }
  
        public void Disconnect()
        {
            client.Disconnect();
        }
            
        private void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
          //  Console.WriteLine();
        }
    
        private void Client_ConnectionClosed(object sender, EventArgs e)
        {           
            Connected = false;
        }

        void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic;
            string message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
            string[] splitTopic = topic.Split('/');
            int rpcActionId = int.Parse(splitTopic[splitTopic.Length - 1]);

            Hashtable deserializedObject = JsonSerializer.DeserializeString(message) as Hashtable;

            if (topic.StartsWith(RPC_REQUEST_TOPIC))
            {
                TBRpcRequest rpcRequest = new TBRpcRequest((string)deserializedObject["method"], deserializedObject["params"], rpcActionId);

                if (OnRpcRequestTopic != null)
                {
                    OnRpcRequestTopic(this, new RpcEventArgs(rpcRequest));
                }
            }
            else if (topic.StartsWith(RPC_RESPONSE_TOPIC))
            {
                string error = (string)deserializedObject["error"];


                if (error != null)
                {
                    RpcError rpcError = new RpcError(error, rpcActionId);

                    if (OnRpcError != null)
                    {
                        OnRpcError(this, new RpcEventArgs(rpcError));
                    }
                }
                else if (OnRpcResponseTopic != null)
                {
                    TBRpcResponse rpcResponse = new TBRpcResponse(rpcActionId);
                    OnRpcResponseTopic(this, new RpcEventArgs(rpcResponse));
                }
            }

            else if (topic == ATTRIBUTES_TOPIC)
            {

            }
            else if (topic.StartsWith(ATTRIBUTES_TOPIC_RESPONSE))
            {
                TBAttributesResponse tBAttributesResponse = new TBAttributesResponse(deserializedObject["client"], deserializedObject["shared"], rpcActionId);
                if (OnAttributesResponseTopic != null)
                {
                    OnAttributesResponseTopic(this, new RpcEventArgs(tBAttributesResponse));
                }
            }
        }

        public void SendRPCReply(TBRpcResponse rpcResponse)
        {
            client.Publish(RPC_RESPONSE_TOPIC + rpcResponse.RpcResponsetId, Encoding.UTF8.GetBytes(rpcResponse.ToJson()), QoS, false);            
        }

        public void SendRPCRequest(TBRpcRequest rpcRequest)
        {
            tbRequestId++;
            rpcRequest.RpcRequestId = tbRequestId;
            client.Publish(RPC_REQUEST_TOPIC + tbRequestId, Encoding.UTF8.GetBytes(rpcRequest.ToJson()), QoS, false);
        }

        public void SendTelemetry(TBTelemetry telemetry)
        {         
            client.Publish(TELEMETRY_TOPIC, Encoding.UTF8.GetBytes(telemetry.ToJson()), QoS, false);
        }

        public void SendAttributes(TBAttributes tBAttributes)
        {
            client.Publish(ATTRIBUTES_TOPIC, Encoding.UTF8.GetBytes(tBAttributes.ToJson()), QoS, false);
        }

        public void RequestAttributes(TBAttributesRequest attributesRequest)
        {
            tbRequestId++;
            string json = attributesRequest.ToJson();
            client.Publish(ATTRIBUTES_TOPIC_REQUEST + tbRequestId, Encoding.UTF8.GetBytes(json), QoS, false);
        }
    }
}
