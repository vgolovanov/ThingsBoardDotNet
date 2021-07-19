//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Runtime.Events;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace nanoFramework.Networking
{
    public class NetworkHelpers
    {
        private static string c_SSID ;
        private static string c_AP_PASSWORD ;

        private static bool _requiresDateTime;

        static public ManualResetEvent IpAddressAvailable = new ManualResetEvent(false);
        static public ManualResetEvent DateTimeAvailable = new ManualResetEvent(false);
        private static NetworkInterface ni;
        private static byte[] hardwareAddress;

        public static void SetupAndConnectNetwork(string WifiApSSID, string WifiApPassword, bool requiresDateTime = false)
        {
            c_SSID = WifiApSSID;
            c_AP_PASSWORD = WifiApPassword;

            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);

            _requiresDateTime = requiresDateTime;
            //  new Thread(WorkingThread).Start();
            WorkingThread();
        }

        private static void WorkingThread()
        {
            do
            {
                Debug.WriteLine("Waiting for network available...");

                Thread.Sleep(500);
            }
            while (!NetworkInterface.GetIsNetworkAvailable());

            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();

            if (nis.Length > 0)
            {
                // get the first interface
                ni = nis[0];

                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    // network interface is Wi-Fi
                    Debug.WriteLine("Network connection is: Wi-Fi");
                    hardwareAddress = ni.PhysicalAddress;

                    //  var ara = Wireless80211Configuration.GetAllWireless80211Configurations();
                    Wireless80211Configuration wc = Wireless80211Configuration.GetAllWireless80211Configurations()[ni.SpecificConfigId];


                    // note on checking the 802.11 configuration
                    // on secure devices (like the TI CC3220SF) the password can't be read
                    // so we can't use the code block bellow to automatically set the profile
                
                    if (wc.Ssid != c_SSID || wc.Password != c_AP_PASSWORD) //&& (wc.Ssid != "" && wc.Password == ""))
                    {
                        // have to update Wi-Fi configuration
                        wc.Ssid = c_SSID;
                        wc.Password = c_AP_PASSWORD;
                        wc.SaveConfiguration();                       
                    }
                    else
                    {
                        // Wi-Fi configuration matches
                        // (or can't be validated)
                    }
                }
                else
                {
                    // network interface is Ethernet
                    Debug.WriteLine("Network connection is: Ethernet");
                }

                //// check if we have an IP
                CheckIP();

                //if (_requiresDateTime)
                //{
                //    IpAddressAvailable.WaitOne();

                //    SetDateTime();
                //}
            }
            else
            {
                throw new NotSupportedException("ERROR: there is no network interface configured.\r\nOpen the 'Edit Network Configuration' in Device Explorer and configure one.");
            }
        }

        public static void SetDateTime()
        {
            Debug.WriteLine("Setting up system clock...");

            // if SNTP is available and enabled on target device this can be skipped because we should have a valid date & time
            while (DateTime.UtcNow.Year < 2018)
            {
                Debug.WriteLine("Waiting for valid date time...");
                // wait for valid date & time
                Thread.Sleep(1000);
            }

            Debug.WriteLine($"System time is: {DateTime.UtcNow.ToString()}");

            DateTimeAvailable.Set();
        }

        public static bool CheckIP()
        {
            //var myAddress = IPGlobalProperties.GetIPAddress();

            //if (myAddress != IPAddress.Any)
            //{
            //    Debug.WriteLine($"We have and IP: {myAddress}");
            //    IpAddressAvailable.Set();
            //    return true;
            //}
            //else
            //{
            //    Debug.WriteLine("No IP...");
            //    return false;
            //}
            Debug.WriteLine("Checking for IP");

            NetworkInterface ni = NetworkInterface.GetAllNetworkInterfaces()[0];
            if (ni.IPv4Address != null && ni.IPv4Address.Length > 0)
            {
                if (ni.IPv4Address[0] != '0')
                {
                    Debug.WriteLine($"We have and IP: {ni.IPv4Address}");
                    IpAddressAvailable.Set();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
                    
        public static string GetPhysicalAddress(char Separator=Char.MinValue)
        {
            StringBuilder hardwareAddrStr = new StringBuilder();

          //  var len = ni.PhysicalAddress.Length;

            foreach (var addr in hardwareAddress)
            {
                hardwareAddrStr.Append(addr.ToString("x2").ToUpper());
                if(Separator!=Char.MinValue)
                {
                    hardwareAddrStr.Append(Separator);
                }
            }

            if (hardwareAddrStr.Length > 0)
            {
                hardwareAddrStr.Remove(hardwareAddrStr.Length - 1, 1);
            }

            return hardwareAddrStr.ToString();            
        }

        static void AddressChangedCallback(object sender, EventArgs e)
        {
            CheckIP();
        }
    }
}