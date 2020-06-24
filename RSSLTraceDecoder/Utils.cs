using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace RSSLTraceDecoder
{
    namespace Utils
    {
        public partial class RdmDataConverter
        {
            public static double? RealStringtoDouble(string hexString)
            {
                var magnitude = HexStringToUInt(hexString.Substring(0, 2));
                var hexValue = hexString.Substring(2, hexString.Length - 2).Trim();
                var val = HexStringToInt64(hexString.Substring(2, hexString.Length - 2));
                switch (magnitude)
                {
                    case (int) MagnitudeType.Infinity:
                        return double.PositiveInfinity;
                    case (int) MagnitudeType.NegInfinity:
                        return double.NegativeInfinity;
                    case (int) MagnitudeType.NotANumber:
                        return double.NaN;
                }

                if (hexValue.StartsWith("F"))
                {
                    var diff = 16 - hexValue.Length;
                    var newValue = string.Empty;
                    for (var i = 0; i < diff; i++) newValue += "F";
                    newValue += hexValue;


                    val = long.Parse(newValue, NumberStyles.HexNumber);
                }

                if (magnitude < (int) MagnitudeType.Divisor1)
                {
                    var exponent = (int) magnitude - MagnitudeType.Exponent0;
                    return val * Math.Pow(10, (int) exponent);
                }

                if (magnitude >= (int) MagnitudeType.Divisor1)
                {
                    var exponent = magnitude - (int) MagnitudeType.Divisor1;
                    return val / Math.Pow(2, (uint) exponent);
                }


                return null;
            }

            public static byte[] StringToByteArray(string hex)
            {
                return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
            }

            public static ulong? HexStringToUInt64(string hexString)
            {
                return Convert.ToUInt64(hexString, 16);
            }

            public static long? HexStringToInt64(string hexString)
            {
                return Convert.ToInt64(hexString, 16);
            }

            public static uint? HexStringToUInt(string hexString)
            {
                return Convert.ToUInt32(hexString, 16);
            }

            public static int? HexStringToInt(string hexString)
            {
                return Convert.ToInt32(hexString, 16);
            }

            public static string TraceStringToString(string hexString)
            {
                return new string(hexString
                    .ToCharArray()
                    .Where(c => !char.IsWhiteSpace(c))
                    .ToArray());
            }

            public static string TimeMsToString(int timeMs)
            {
                if (timeMs == 0)
                    return "";
                var msec = timeMs % 1000;
                var seconds = timeMs / 1000 % 60;
                var minutes = timeMs / 60000 % 60;
                var hours = timeMs / 3600000 % 24;
                return $"{hours}:{minutes}:{seconds}:{msec}";
            }

            public static DateTime? HexStringToDateTime(string hexDate)
            {
                if (hexDate.Trim() == string.Empty)
                    return null;

                var day = HexStringToInt(hexDate.Substring(0, 2));
                var month = HexStringToInt(hexDate.Substring(2, 2));
                var year = HexStringToInt(hexDate.Substring(4, 4));
                return new DateTime((int) year, (int) month, (int) day);
            }

            public static string HexStringToTime(string hexTime)
            {
                if (hexTime.Trim() == string.Empty)
                    return null;

                var times = new List<string>();
                for (var i = 0; i < hexTime.Length;)
                    if (i >= 0 && i < 6)
                    {
                        times.Add($"{HexStringToInt(hexTime.Substring(i, 2)):00}");
                        i += 2;
                    }
                    else if (i >= 6)
                    {
                        times.Add($"{HexStringToInt(hexTime.Substring(i, hexTime.Length - 6))}");
                        i += hexTime.Length - 6;
                    }

                return string.Join(":", times.ToArray());
            }
        }

        public class XmlHelper
        {
            public static IEnumerable<XElement> GetElement(string xmlString, string elementName)
            {
                using (var reader = XmlReader.Create(new StringReader(xmlString)))
                {
                    while (reader.Name == elementName || reader.ReadToFollowing(elementName))
                        yield return (XElement) XNode.ReadFrom(reader);
                }
            }
        }
    }
}