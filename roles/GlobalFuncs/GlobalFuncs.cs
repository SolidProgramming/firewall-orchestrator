﻿using System.Net.Sockets;
using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;
using NetTools;

namespace FWO.GlobalFunctions
{
    public static class GlobalFuncs
    {
        public static string ShowBool(bool boolVal)
        {
            return boolVal ? "\u2714" : "\u2716";
        }
        public static class IpOperations
        {              
            public static bool IsInSubnet(IPAddress address, string cidrString)
            {
                string[] parts = cidrString.Split('/');
                if (parts.Length != 2)
                {
                    throw new FormatException("Invalid CIDR format.");
                }

                var networkAddress = IPAddress.Parse(parts[0]);
                int prefixLength = int.Parse(parts[1]);

                if (address.AddressFamily != networkAddress.AddressFamily)
                {
                    // The IP versions must match (IPv4 vs IPv6)
                    return false;
                }

                if (address.AddressFamily == AddressFamily.InterNetwork)  // IPv4
                {
                    return IsIPv4InSubnet(address, networkAddress, prefixLength);
                }
                else if (address.AddressFamily == AddressFamily.InterNetworkV6)  // IPv6
                {
                    return IsIPv6InSubnet(address, networkAddress, prefixLength);
                }
                else
                {
                    throw new NotSupportedException("Only IPv4 and IPv6 are supported.");
                }
            }
            private static bool IsIPv4InSubnet(IPAddress address, IPAddress networkAddress, int prefixLength)
            {
                uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes().Reverse().ToArray(), 0);
                uint networkIpAddress = BitConverter.ToUInt32(networkAddress.GetAddressBytes().Reverse().ToArray(), 0);

                uint mask = ( uint.MaxValue << ( 32 - prefixLength ) ) & uint.MaxValue;

                return ( ipAddress & mask ) == ( networkIpAddress & mask );
            }
            private static bool IsIPv6InSubnet(IPAddress address, IPAddress networkAddress, int prefixLength)
            {
                BigInteger ipAddressBigInt = new(address.GetAddressBytes().Reverse().ToArray().Concat(new byte[] { 0 }).ToArray());
                BigInteger networkIpAddressBigInt = new(networkAddress.GetAddressBytes().Reverse().ToArray().Concat(new byte[] { 0 }).ToArray());

                BigInteger mask = BigInteger.Pow(2, 128) - BigInteger.Pow(2, 128 - prefixLength);

                return ( ipAddressBigInt & mask ) == ( networkIpAddressBigInt & mask );
            }
            // // TODO: rewrite this to universally deal with ipv4 and ipv6
            // // currently the input parameter subnetMask is unclear
            // // looks like it is not a mask but a CIDR as a string
            // public static bool IsInSubnet(IPAddress address, string subnetMask)
            // {
            //     var slashIdx = subnetMask.IndexOf("/");
            //     var maskAddress = IPAddress.Parse(slashIdx == -1 ? subnetMask : subnetMask.Substring(0, slashIdx));
            //     if (maskAddress.AddressFamily != address.AddressFamily)
            //     {
            //         return false;
            //     }

            //     int maskLength = slashIdx == -1 ? (maskAddress.AddressFamily == AddressFamily.InterNetwork ? 31 : 127) : int.Parse(subnetMask.Substring(slashIdx + 1));
            //     if (maskLength == 0)
            //     {
            //         return true;
            //     }

            //     if (maskAddress.AddressFamily == AddressFamily.InterNetwork)
            //     {

            //         var maskAddressBits = BitConverter.ToUInt32(maskAddress.GetAddressBytes().Reverse().ToArray(), 0);
            //         var ipAddressBits = BitConverter.ToUInt32(address.GetAddressBytes().Reverse().ToArray(), 0);
            //         uint mask = uint.MaxValue << (32 - maskLength);
            //         return (maskAddressBits & mask) == (ipAddressBits & mask);
            //     }

            //     if (maskAddress.AddressFamily == AddressFamily.InterNetworkV6)
            //     {
            //         var maskAddressBits = new BitArray(maskAddress.GetAddressBytes().Reverse().ToArray());
            //         var ipAddressBits = new BitArray(address.GetAddressBytes().Reverse().ToArray());
            //         var ipAddressLength = ipAddressBits.Length;

            //         if (maskAddressBits.Length != ipAddressBits.Length)
            //         {
            //             throw new ArgumentException("Length of IP Address and Subnet Mask do not match.");
            //         }

            //         for (var i = ipAddressLength - 1; i >= ipAddressLength - maskLength; i--)
            //         {
            //             if (ipAddressBits[i] != maskAddressBits[i])
            //             {
            //                 return false;
            //             }
            //         }
            //         return true;
            //     }
            //     return false;
            // }
            public static string StripOffNetmask(string ip)
            {
                if (ip.TryGetNetmask(out string netmask))
                    return ip.Replace(netmask, "");

                return ip;
            }
            public static string SanitizeIp(string cidr_str)
            {
                cidr_str = StripOffNetmask(cidr_str);

                if (IPAddress.TryParse(cidr_str, out IPAddress? ip))
                {
                    if (ip != null)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            cidr_str = ip.ToString();
                            if (cidr_str.IndexOf('/') < 0) // a single ip without mask
                            {
                                cidr_str += "/128";
                            }
                            if (cidr_str.IndexOf('/') == cidr_str.Length - 1) // wrong format (/ at the end, fixing this by adding 128 mask)
                            {
                                cidr_str += "128";
                            }
                        }
                        else if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            cidr_str = ip.ToString();
                            if (cidr_str.IndexOf('/') < 0) // a single ip without mask
                            {
                                cidr_str += "/32";
                            }
                            if (cidr_str.IndexOf('/') == cidr_str.Length - 1) // wrong format (/ at the end, fixing this by adding 32 mask)
                            {
                                cidr_str += "32";
                            }
                        }
                    }
                }
                return cidr_str;
            }
            public static bool OverlapExists(IPAddressRange a, IPAddressRange b)
            {
                return IpToUint(a.Begin) <= IpToUint(b.End) && IpToUint(b.Begin) <= IpToUint(a.End);
            }
            public static uint IpToUint(IPAddress ipAddress)
            {
                byte[] bytes = ipAddress.GetAddressBytes();

                // flip big-endian(network order) to little-endian
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }

                return BitConverter.ToUInt32(bytes, 0);
            }
            public static bool CheckOverlap(string ip1, string ip2)
            {
                IPAddressRange range1 = GetIPAdressRange(ip1);
                IPAddressRange range2 = GetIPAdressRange(ip2);

                if (range1.Begin.AddressFamily != range2.Begin.AddressFamily)
                    return false;


                return OverlapExists(range1, range2);
            }
            public static IPAddressRange GetIPAdressRange(string ip)
            {
                IPAddressRange ipAddressRange;

                if (ip.TryGetNetmask(out _))
                {
                    (string Start, string End) = ip.CidrToRangeString();
                    ipAddressRange = new(IPAddress.Parse(Start), IPAddress.Parse(End));
                }
                else if (ip.TrySplit('-', 1, out _) && IPAddressRange.TryParse(ip, out IPAddressRange ipRange))
                {
                    ipAddressRange = ipRange;
                }
                else
                {
                    ipAddressRange = new IPAddressRange(IPAddress.Parse(ip), IPAddress.Parse(ip));
                }

                return ipAddressRange;
            }          
        }
    }
    public static class Extensions
    {
        public static byte[] NormalizeBytes(byte[] bytes, int targetLength)
        {
            if (bytes.Length < targetLength)
            {
                // Pad the byte array to the required length
                byte[] padded = new byte[targetLength];
                Array.Copy(bytes, padded, bytes.Length);
                return padded;
            }
            return bytes.Take(targetLength).ToArray();  // Ensure it's exactly targetLength bytes
        }
        public static bool TrySplit(this string text, char separator, int index, out string output)
        {
            string[] splits = text.Split(separator);

            output = "";

            if (splits.Length < 2 || splits.Length < index + 1)
                return false;

            output = splits[index];

            return true;
        }
        public static bool TrySplit(this string text, char separator, out int length)
        {
            string[] splits = text.Split(separator);

            length = 0;

            if (splits.Length < 2)
                return false;

            length = splits.Length;

            return true;
        }       
        public static bool TryGetNetmask(this string ip, out string netmask)
        {
            netmask = "";

            Match match = Regex.Match(ip, @"(\/[\d\.\:]+)\D?");

            if (match.Success)
                netmask = match.Groups[1].Value;

            return match.Success;
        }
        public static bool IsIPv4(this string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out IPAddress? addr))
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                    return true;

                return false;
            }

            return false;
        }
        public static bool IsIPv6(this string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out IPAddress? addr))
            {
                if (addr.AddressFamily == AddressFamily.InterNetworkV6)
                    return true;

                return false;
            }

            return false;
        }
        public static (IPAddress start, IPAddress end) CidrToRange(this string cidr)
        {
            string[] parts = cidr.Split('/');
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid CIDR format.");
            }

            var ip = IPAddress.Parse(parts[0]);
            int prefixLength = int.Parse(parts[1]);

            var addressBytes = ip.GetAddressBytes();

            if (addressBytes.Length == 4)
            {
                // IPv4 case
                return IPv4CidrToRange(addressBytes, prefixLength);
            }
            else if (addressBytes.Length == 16)
            {
                // IPv6 case
                return IPv6CidrToRange(addressBytes, prefixLength);
            }
            else
            {
                throw new FormatException("Invalid IP address format.");
            }
        }
        public static (string start, string end) CidrToRangeString(this string cidr)
        {
            IPAddress ipStart;
            IPAddress ipEnd;

            (ipStart, ipEnd) = CidrToRange(cidr);

            return (ipStart.ToString(), ipEnd.ToString());
        }
        public static (IPAddress start, IPAddress end) IPv4CidrToRange(byte[] addressBytes, int prefixLength)
        {
            uint ipAddress = BitConverter.ToUInt32(addressBytes.Reverse().ToArray(), 0);
            uint mask = ( uint.MaxValue << ( 32 - prefixLength ) ) & uint.MaxValue;

            uint startIp = ipAddress & mask;
            uint endIp = startIp | ~mask;

            return (new IPAddress(BitConverter.GetBytes(startIp).Reverse().ToArray()),
                    new IPAddress(BitConverter.GetBytes(endIp).Reverse().ToArray()));
        }
        public static (IPAddress start, IPAddress end) IPv6CidrToRange(byte[] addressBytes, int prefixLength)
        {
            if (BitConverter.IsLittleEndian)
            {
                addressBytes = addressBytes.Reverse().ToArray();  // Reverse byte array for BigInteger compatibility
            }

            var addressBigInt = new BigInteger(addressBytes.Concat(new byte[] { 0 }).ToArray());  // Treat as unsigned

            var mask = BigInteger.Pow(2, 128) - BigInteger.Pow(2, 128 - prefixLength);  // Compute mask
            var startIpBigInt = addressBigInt & mask;  // Apply mask for the start IP
            var endIpBigInt = startIpBigInt | ( ~mask & BigInteger.Pow(2, 128) - 1 );  // Compute end IP

            var startIpBytes = startIpBigInt.ToByteArray();
            var endIpBytes = endIpBigInt.ToByteArray();

            // Ensure the byte arrays are 16 bytes (128 bits) for IPv6
            startIpBytes = NormalizeBytes(startIpBytes, 16);
            endIpBytes = NormalizeBytes(endIpBytes, 16);

            if (BitConverter.IsLittleEndian)
            {
                startIpBytes = startIpBytes.Reverse().ToArray();
                endIpBytes = endIpBytes.Reverse().ToArray();
            }

            return (new IPAddress(startIpBytes), new IPAddress(endIpBytes));
        }
    }
}

