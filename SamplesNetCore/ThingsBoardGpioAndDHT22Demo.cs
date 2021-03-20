using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ThingsBoardDotNet;

namespace SamplesNetCore
{
    public class ThingsBoardGpioAndDHT22Demo
    {
        private static ThingsBoardDeviceMqttClient thingsBoard = new ThingsBoardDeviceMqttClient();

        private static string thingsBoardHost = "demo.thingsboard.io";
        private static string thingsBoardAccessToken = "Access_TOKEN";

        private static int thingsBoardPort = 1883;

        private static Boolean[] pins = new Boolean[] { false, false, false, false }; // Pins array where index is pin number.
        private static double someDeviceValue;
        private static Random random = new Random();

        public static void Main()
        {           
            someDeviceValue = random.Next(50);

            if (!SetupThingsBoard())
            {
                Console.WriteLine("Error connecting to the server");
                return;
            }

            // launch telemetry thread
            Thread telemetryThread = new Thread(new ThreadStart(TelemetryLoop));
            telemetryThread.Start();

            Console.WriteLine("Connected to the server.");

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

                Console.WriteLine("Message sent: " + telemetry.ToJson());

                Thread.Sleep(60000);
            }
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

            Console.WriteLine("Set Gpio Pin:" + pin + " Value:" + enabled);

            TBRpcResponse rpcResponse = new TBRpcResponse(rpcRequest.RpcRequestId);
            rpcResponse.Add(pin, pins[pin]);

            thingsBoard.SendRPCReply(rpcResponse);
        }

        [RpcName("setValue")]
        public static void SetValue(TBRpcRequest rpcRequest)
        {
            someDeviceValue = (double)rpcRequest.RpcParams;
            Console.WriteLine("Set Value: " + someDeviceValue);
        }

        [RpcName("getValue")]
        public static void GetValue(TBRpcRequest rpcRequest)
        {
            Console.WriteLine("Get Value: " + someDeviceValue);

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
            Console.WriteLine("Rpc Call with Id - " + e.RpcError.RpcResponsetId + " error :" + e.RpcError.Error);
        }
    }
}
