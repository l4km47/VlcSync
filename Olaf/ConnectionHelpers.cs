﻿using System;
using System.Net;
using System.Net.NetworkInformation;

namespace AuraWave.WinApi.Olaf
{
    /// <summary>
    ///     copyright © Unknown Devalopers
    ///     http://www.github.com/unkdevt
    ///     Author @ Manoj Lakmal
    ///     http://www.github.com/l4km47
    ///     Email @ manojlakmal999@gmail.com
    /// </summary>
    public static class ConnectionHelpers
    {

        /// <summary>
        ///     Indicates whether any network connection is available
        ///     Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailablex { get { return IsNetworkAvailable(0); } }

        /// <summary>
        ///     Indicates whether any network connection is available.
        ///     Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if (ni.OperationalStatus != OperationalStatus.Up ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if (ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }

        #region Network testing functions

        /// <summary>
        ///     Return Current network connection IpV4 address
        /// </summary>
        /// <returns>
        ///     Ipaddress ipv4
        ///     <c>error message</c>network connection is unavailable: otherwise, <c>success</c>.
        /// </returns>
        public static string GetCurrentIpv4 => GetIP("InterNetwork");

        /// <summary>
        ///     Return Current network connection IpV4 address
        /// </summary>
        /// <return>Ipaddress ipv4</return>
        public static string GetCurrentIpv6 => GetIP("InterNetworkV6");

        /// <summary>
        ///     get ip addres on current host
        /// </summary>
        /// <param name="version">
        ///     ip address version   </br>if IPv6 : InterNetworkV6   if IPv4 : InterNetwork
        /// </param>
        /// <returns>ip</returns>
        private static string GetIP(string version)
        {
            IPHostEntry host; //variable for current ip address
            var ip = "?";
            host = Dns.GetHostEntry(Dns.GetHostName()); //get current ip address
            foreach (var ip_ in host.AddressList)
            {
                if (version == "InterNetwork") //check ip address version is IPv4
                    if (ip_.AddressFamily.ToString() == "InterNetwork")
                    {
                        ip = ip_.ToString();
                        return ip;
                    }
                if (version == "InterNetworkV6") //check ip address version is IPv6
                    if (ip_.AddressFamily.ToString() == "InterNetworkV6")
                    {

                        ip = ip_.ToString();
                        if (ip == "::1")
                            return "IPv6 is currently not avilable";
                        return ip;
                    }
            }
            return "";
        }

        /// <summary>
        ///     ping using host name or ip address
        /// </summary>
        /// <param name="HostnameorAddress">example :- www.google.com or 192.168.1.2</param>
        /// <returns>
        ///     <c>error message</c>if ip address or host name is invalid or network connection is unavailable: otherwise,
        ///     <c>success</c>.
        /// </returns>
        ///
        public class PingOut
        {
            public string msg = "";
            public IPAddress ip = null;
            public bool isAlive = false;
        }
        public static PingOut PingHost(string HostnameorAddress, out string host)
        {
            host = HostnameorAddress;
            try
            {
                if (HostnameorAddress != string.Empty)
                {
                    var ping = new Ping();
                    var result = ping.Send(HostnameorAddress);
                    var pin = new string[6];
                    if (result != null && result.Status == IPStatus.Success)
                        return new PingOut()
                        {
                            isAlive = result != null && (result.Status == IPStatus.Success), ip = result.Address.MapToIPv4(),
                            msg = result.Status.ToString()
                        };
                    return new PingOut() { isAlive = false, ip = null, msg = "Ping fail" };
                }

                return new PingOut() { isAlive = false, ip = null, msg = "Address not found" };
            }
            catch (NetworkInformationException e)
            {
                return new PingOut() { isAlive = false, ip = null, msg = e.Message };
            }
            catch (Exception e)
            {
                return new PingOut() { isAlive = false, ip = null, msg = e.Message };
            }

        }

        /// <summary>
        ///     ping using ip address
        /// </summary>
        /// <param name="IPaddress">example :- 192.168.1.1</param>
        /// <returns>
        ///     <c>error message</c>if ip address or host name is invalid or network connection is unavailable: otherwise,
        ///     <c>success</c>.
        /// </returns>
        public static string PingHost(IPAddress IPaddress)
        {
            try
            {
                var ping = new Ping();
                var result = ping.Send(IPaddress);
                var pin = new string[6];
                if (result.Status == IPStatus.Success)
                    return result.Status + "\n" + result.Address.MapToIPv4();
                return result.Status.ToString();
            }
            catch (NetworkInformationException e)
            {
                return e.Message;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion
    }
}
