using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DotXxlJob.Core
{
    /// <summary>
    /// ip utility
    /// </summary>
    internal static  class IPUtility
    {
        #region Private Members
        /// <summary>
        /// A类: 10.0.0.0-10.255.255.255
        /// </summary>
        private static long ipABegin, ipAEnd;
        /// <summary>
        /// B类: 172.16.0.0-172.31.255.255   
        /// </summary>
        private static long ipBBegin, ipBEnd;
        /// <summary>
        /// C类: 192.168.0.0-192.168.255.255
        /// </summary>
        private static long ipCBegin, ipCEnd;
        #endregion

        #region Constructors
        /// <summary>
        /// static new
        /// </summary>
        static IPUtility()
        {
            ipABegin = ConvertToNumber("10.0.0.0");
            ipAEnd = ConvertToNumber("10.255.255.255");

            ipBBegin = ConvertToNumber("172.16.0.0");
            ipBEnd = ConvertToNumber("172.31.255.255");

            ipCBegin = ConvertToNumber("192.168.0.0");
            ipCEnd = ConvertToNumber("192.168.255.255");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// ip address convert to long
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private static long ConvertToNumber(string ipAddress)
        {
            return ConvertToNumber(IPAddress.Parse(ipAddress));
        }
        /// <summary>
        /// ip address convert to long
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private static long ConvertToNumber(IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();
            return bytes[0] * 256 * 256 * 256 + bytes[1] * 256 * 256 + bytes[2] * 256 + bytes[3];
        }
        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsIntranet(string ipAddress)
        {
            return IsIntranet(ConvertToNumber(ipAddress));
        }
        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private static bool IsIntranet(IPAddress ipAddress)
        {
            return IsIntranet(ConvertToNumber(ipAddress));
        }
        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="longIP"></param>
        /// <returns></returns>
        private static bool IsIntranet(long longIP)
        {
            return ((longIP >= ipABegin) && (longIP <= ipAEnd) ||
                    (longIP >= ipBBegin) && (longIP <= ipBEnd) ||
                    (longIP >= ipCBegin) && (longIP <= ipCEnd));
        }
        /// <summary>
        /// 获取本机内网IP
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalIntranetIP()
        {
           return NetworkInterface
            .GetAllNetworkInterfaces()
            .Select(p => p.GetIPProperties())
            .SelectMany(p => 
                p.UnicastAddresses
            ).FirstOrDefault(p => p.Address.AddressFamily == AddressFamily.InterNetwork 
                                  && !IPAddress.IsLoopback(p.Address)
                                  && IsIntranet(p.Address))?.Address;
        }
        /// <summary>
        /// 获取本机内网IP列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIntranetIPList()
        {
            var infList =NetworkInterface.GetAllNetworkInterfaces()
            .Select(p => p.GetIPProperties())
            .SelectMany(p => p.UnicastAddresses)
            .Where(p => 
                p.Address.AddressFamily == AddressFamily.InterNetwork 
                && !IPAddress.IsLoopback(p.Address)
                && IsIntranet(p.Address)            
            );
                     
            var result = new List<IPAddress>();
            foreach (var child in infList)
            {
                result.Add(child.Address);
            }

            return result;
        }
        #endregion
    }
}