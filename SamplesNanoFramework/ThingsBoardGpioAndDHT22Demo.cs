using nanoFramework.Networking;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using ThingsBoardDotNet;

namespace SamplesNanoFramework
{
    public class ThingsBoardGpioAndDHT22Demo
    {
        private static ThingsBoardDeviceMqttClient thingsBoard = new ThingsBoardDeviceMqttClient();
       
        private static string thingsBoardHost = "demo.thingsboard.io";
        private static string thingsBoardAccessToken = "Access_TOKEN";

        private static int thingsBoardPort = 1883;

        private static Boolean[] pins = new Boolean[] { false, false, false, false}; // Pins array where index is pin number.
        private static double someDeviceValue;
        private static Random random = new Random();

        public static void Main()
        {           
            // if we are using TLS it requires valid date & time
            NetworkHelpers.SetupAndConnectNetwork(true);

            Debug.WriteLine("Waiting for network up and IP address...");
            NetworkHelpers.IpAddressAvailable.WaitOne();

            Debug.WriteLine("Waiting for valid Date & Time...");
            NetworkHelpers.DateTimeAvailable.WaitOne();

            someDeviceValue = random.Next(50);

            if (!SetupThingsBoard())
            {
                Debug.WriteLine("Error connecting to the server");
                return;
            }

            // launch telemetry thread
            Thread telemetryThread = new Thread(new ThreadStart(TelemetryLoop));
            telemetryThread.Start();

            Debug.WriteLine("Connected to the server.");

            while (true)
            {
                Thread.Sleep(1000);
            }
        }  
        
        private static bool SetupThingsBoard()
        {
            ReflectionHelper.FindRpcMethods(typeof(ThingsBoardGpioAndDHT22Demo));
            thingsBoard.OnRpcRequestTopic += OnRpcRequestTopic;           
            thingsBoard.OnRpcError += OnRpcError;
           
            if (!thingsBoard.Connect(thingsBoardHost, thingsBoardAccessToken, thingsBoardPort))
            {             
                return false;
            }
            return true;           
        }

        static void TelemetryLoop()
        {
            while (true)
            {
                TBTelemetry telemetry = new TBTelemetry();
                telemetry.Add("temperature", random.Next(30));
                telemetry.Add("humidity", random.Next(80));

                thingsBoard.SendTelemetry(telemetry);

                Debug.WriteLine("Message sent: " + telemetry.ToJson());

                Thread.Sleep(60000);
            }
        }

        [RpcName("getGpioStatus")]
        public static void GetGpioStatus(TBRpcRequest rpcRequest)
        {
            TBRpcResponse rpcResponse = new TBRpcResponse(rpcRequest.RpcRequestId);
            rpcResponse.Add(pins);
            thingsBoard.SendRPCReply(rpcResponse);

            Debug.WriteLine("Get Gpio Status: " + rpcResponse.ToJson());
        }

        [RpcName("setGpioStatus")]
        public static void SetGpioStatus(TBRpcRequest rpcRequest)
        {
            var pin = (Int64)(rpcRequest.RpcParams as Hashtable)["pin"];
            var enabled = (Boolean)(rpcRequest.RpcParams as Hashtable)["enabled"];

            pins[pin] = enabled;

            Debug.WriteLine("Set Gpio Pin:" + pin + " Value:" + enabled);

            TBRpcResponse rpcResponse = new TBRpcResponse(rpcRequest.RpcRequestId);
            rpcResponse.Add(pin, pins[pin]);

            thingsBoard.SendRPCReply(rpcResponse);
        }

        [RpcName("setValue")]
        public static void SetValue(TBRpcRequest rpcRequest)
        {
            someDeviceValue = (double)rpcRequest.RpcParams;
            Debug.WriteLine("Set Value: " + someDeviceValue);
        }

        [RpcName("getValue")]
        public static void GetValue(TBRpcRequest rpcRequest)
        {
            Debug.WriteLine("Get Value: " + someDeviceValue);

            TBRpcResponse rpcResponse = new TBRpcResponse(rpcRequest.RpcRequestId);
            rpcResponse.Add(someDeviceValue);
            thingsBoard.SendRPCReply(rpcResponse);
        }

        private static void OnRpcRequestTopic(object sender, RpcEventArgs e)
        {
            ReflectionHelper.InvokeRpcMethod(e.RpcRequest);
        }
        
        private static void OnRpcError(object sender, RpcEventArgs e)
        {
            Debug.WriteLine("Rpc Call with Id - " + e.RpcError.RpcResponsetId + " error :" + e.RpcError.Error);         
        }  
    }
}
