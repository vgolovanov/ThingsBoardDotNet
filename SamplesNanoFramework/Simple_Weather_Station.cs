using MBN.Modules;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using ThingsBoardDotNet;

namespace SamplesNanoFramework
{
    public class Simple_Weather_Station
    {
        private static ThingsBoardDeviceMqttClient thingsBoard = new ThingsBoardDeviceMqttClient();

        private static string thingsBoardHost = "demo.thingsboard.io";
        private static string thingsBoardAccessToken = "Access_TOKEN";
        private static string wifiSSID = "REPLACE_WITH-YOUR-SSID";
        private static string wifiApPASSWORD = "REPLACE_WITH-YOUR-WIFI-KEY";
        private static int thingsBoardPort = 1883;        
        private static BME280 bme280Sensor;

        public static void Main()
        {
            //Setup I2C pins for ESP32 board
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            // if we are using TLS it requires valid date & time     
            NetworkHelpers.SetupAndConnectNetwork(wifiSSID, wifiApPASSWORD, true);

            NetworkHelpers.CheckIP();
            Debug.WriteLine("Waiting for network up and IP address...");
            NetworkHelpers.IpAddressAvailable.WaitOne();

            bme280Sensor = new BME280(1, BME280.I2CAddresses.Address0);
           
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
                telemetry.Add("temperature", bme280Sensor.ReadTemperature());
                telemetry.Add("humidity", bme280Sensor.ReadHumidity());
                telemetry.Add("pressure", bme280Sensor.ReadPressure());

                thingsBoard.SendTelemetry(telemetry);

                Debug.WriteLine("Message sent: " + telemetry.ToJson());

                Thread.Sleep(60000);
            }
        }
    }
}
