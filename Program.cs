using Json.NetMF;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading;
using ThingsBoardDotNet;

namespace SamplesNetCore
{
    class Program
    {
        private static ThingsBoardDeviceMqttClient thingsBoard = new ThingsBoardDeviceMqttClient();
        private static Boolean[] pins = new Boolean[] { true, true, false, false, false, false, false, false, false, false }; // Pins array where index is pin number.
        private static double someDeviceValue;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            someDeviceValue = random.Next(50, 90);

            ReflectionHelper.FindRpcMethods(typeof(Program));            
            thingsBoard.OnRpcRequestTopic += OnRpcRequestTopic;
            thingsBoard.OnRpcResponseTopic += OnRpcResponseTopic;
            thingsBoard.OnRpcError += OnRpcError;
            thingsBoard.OnAttributesResponseTopic += OnAttributesResponseTopic;

            if (!thingsBoard.Connect("gatekeeper.co", "gB7qRrBjmiyDxoH1kihB", 3883))
            {
                Console.WriteLine("Error connecting to the server");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Connected to the server.");

            while (thingsBoard.Connected)
            {
                Console.Write(">");
                string line = Console.ReadLine().Trim();
                if (line == "quit" || line == "exit")
                {
                    thingsBoard.Disconnect();
                    return;
                }

                string cmdName = line.Split(' ')[0];
                string[] cmdArgs = line.Split(' ').Skip(1).ToArray();

                switch (cmdName)
                {
                    case "rpccall":
                        DoRpcCall();
                        break;

                    case "telemetry":
                        DoTelemetry(cmdArgs);
                        break;

                    case "attributes":
                    case "attr":
                        DoAttributes(cmdArgs);
                        break;

                    case "requestattr":
                        DoRequestAttributes(cmdArgs);
                        break;
                }
            }
            Console.WriteLine("Disconnected.");
            Console.ReadKey();
        }

        private static void DoRpcCall()
        {          
            TBRpcRequest rpcRequest = new TBRpcRequest("getTime");
            var asdsdaa = rpcRequest.ToJson();
            thingsBoard.SendRPCRequest(rpcRequest);
        }

        private static void DoTelemetry(string[] args)
        {
            TBTelemetry telemetry = new TBTelemetry();

            foreach(var telValue in args)
            {
                var dataToSend = telValue.Split(':');
                telemetry.Add(dataToSend[0], dataToSend[1]);
            }
            thingsBoard.SendTelemetry(telemetry);
        }

        private static void DoAttributes(string[] args)
        {
            TBAttributes tBAttributes = new TBAttributes();

            foreach (var attr in args)
            {
                var dataToSend = attr.Split(':');
                tBAttributes.Add(dataToSend[0], dataToSend[1]);
            }
            thingsBoard.SendAttributes(tBAttributes);
        }

        private static void DoRequestAttributes(string[] args)
        {
            TBAttributesRequest tBAttributesRequest = new TBAttributesRequest();
            
            foreach (var attr in args)
            {
                var dataToSend = attr.Split(':');
                if (dataToSend[0] == "ca")
                {
                    tBAttributesRequest.AddClientAttribute(dataToSend[1]);
                }
                else if (dataToSend[0] == "sa")
                {
                    tBAttributesRequest.AddSharedAttribute(dataToSend[1]);
                }              
            }
            thingsBoard.RequestAttributes(tBAttributesRequest);
        }

        [RpcName("getGpioStatus")]
        public static void GetGpioStatus(TBRpcRequest rpcRequest)
        {            
            TBRpcResponse rpcResponse = new TBRpcResponse(rpcRequest.RpcRequestId);
            rpcResponse.Add(pins);          
            thingsBoard.SendRPCReply(rpcResponse);

            Console.WriteLine("Get Gpio Status: " + rpcResponse.ToJson());
        }

        [RpcName("setGpioStatus")]
        public static void SetGpioStatus(TBRpcRequest rpcRequest)
        {          
            var pin = (Int64)(rpcRequest.RpcParams as Hashtable)["pin"];
            var enabled = (Boolean)(rpcRequest.RpcParams as Hashtable)["enabled"];
       
            pins[pin] = enabled;

            Console.WriteLine("Set Gpio Pin:" + pin + " Value:" + enabled );

            TBRpcResponse rpcResponse = new TBRpcResponse(rpcRequest.RpcRequestId);
            rpcResponse.Add(pin, pins[pin]);
                      
            thingsBoard.SendRPCReply(rpcResponse);
        }

        [RpcName("setValue")]
        public static void SetValue(TBRpcRequest rpcRequest)
        {
            someDeviceValue = (double) rpcRequest.RpcParams;            
        }

        [RpcName("getValue")]
        public static void GetValue(TBRpcRequest rpcRequest)
        {           
            TBRpcResponse rpcResponse = new TBRpcResponse(rpcRequest.RpcRequestId);
            rpcResponse.Add(someDeviceValue);            
            thingsBoard.SendRPCReply(rpcResponse);
        }

        private static void OnRpcRequestTopic(object sender, RpcEventArgs e)
        {
            ReflectionHelper.InvokeRpcMethod(e.RpcRequest);         
        }

        private static void OnRpcResponseTopic(object sender, RpcEventArgs e)
        {
            Console.WriteLine("Rpc Call with Id" + e.RpcResponse.RpcResponsetId + " ok");
        }

        private static void OnRpcError(object sender, RpcEventArgs e)
        {           
            Console.WriteLine("Rpc Call with Id - " + e.RpcError.RpcResponsetId + " error :" + e.RpcError.Error);
        }

        private static void OnAttributesResponseTopic(object sender, RpcEventArgs e)
        {
            Console.WriteLine("Attributes Response" + e.AttributesResponse.ToString() + " ok");
        }
    }
}
