using Iot.Device.Bmxx80;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using System;
using System.Collections;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
//using System.Collections.Generic;
//using System.Text;

namespace nanoFramework.ThingsBoard.Samples
{
    class ThingsBoard_Rpc_and_Telemetry_Demo
    {
        private static ThingsBoardDeviceMqttClient thingsBoard = new ThingsBoardDeviceMqttClient();

        private static string thingsBoardHost = "demo.thingsboard.io";
        //private static string thingsBoardAccessToken = "Access_TOKEN";
        //private static string wifiSSID = "REPLACE_WITH-YOUR-SSID";
        //private static string wifiApPASSWORD = "REPLACE_WITH-YOUR-WIFI-KEY";
        private static int sleepTimeMinutes = 60000;

        private static string thingsBoardAccessToken = "ESP32_DEMO_TOKEN2";// "Access_TOKEN";
        private static string wifiSSID = "Xiaomi_A7EB";//"REPLACE_WITH-YOUR-SSID";
        private static string wifiApPASSWORD = "SLAWEK0000000";// "REPLACE_WITH-YOUR-WIFI-KEY";

        private static int thingsBoardPort = 1883;

        private static Boolean[] pins = new Boolean[] { false, false, false, false }; // Pins array where index is pin number.
        private static double someDeviceValue;
        private static Random random = new Random();
        private static DateTime allupOperation = DateTime.UtcNow;
        private static int minutesToGoToSleep = 2;

        //   private static BME280 bme280Sensor;
        private static Bme280 bme280Sensor;

        public static void Main()
        {
            //Setup I2C pins for ESP32 board
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            CancellationTokenSource cs = new(sleepTimeMinutes);

            var success = NetworkHelper.ConnectWifiDhcp(wifiSSID, wifiApPASSWORD, setDateTime: true, token: cs.Token);

            if (!success)
            {

                Debug.WriteLine($"Can't connect to wifi: {NetworkHelper.ConnectionError.Error}");
                if (NetworkHelper.ConnectionError.Exception != null)
                {
                    Debug.WriteLine($"NetworkHelper.ConnectionError.Exception");
                }

                GoToSleep();
            }

            // Reset the time counter if the previous date was not valid
            if (allupOperation.Year < 2018)
            {
                allupOperation = DateTime.UtcNow;
            }

            Debug.WriteLine($"Date and time is now {DateTime.UtcNow}");

            const int busId = 1;
            I2cConnectionSettings i2cSettings = new(busId, Bmp280.DefaultI2cAddress);           
            I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            
            bme280Sensor = new Bme280(i2cDevice);
            
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
                Thread.Sleep(10000);
            }
        }

        private static void GoToSleep()
        {
            Debug.WriteLine($"Full operation took: {DateTime.UtcNow - allupOperation}");
            Debug.WriteLine($"Set wakeup by timer for {minutesToGoToSleep} minutes to retry.");
            Sleep.EnableWakeupByTimer(new TimeSpan(0, 0, minutesToGoToSleep, 0));
            Debug.WriteLine("Deep sleep now");
            Sleep.StartDeepSleep();
        }

        private static bool SetupThingsBoard()
        {
            ReflectionHelper.FindRpcMethods(typeof(ThingsBoard_Rpc_and_Telemetry_Demo));
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
                var bmeResult = bme280Sensor.Read();
                TBTelemetry telemetry = new TBTelemetry();
                telemetry.Add("temperature", bmeResult.Temperature.DegreesCelsius);
                telemetry.Add("humidity", bmeResult.Humidity.Percent);
                telemetry.Add("pressure", bmeResult.Pressure.Hectopascals);

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
