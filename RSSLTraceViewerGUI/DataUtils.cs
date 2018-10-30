using System.Text.RegularExpressions;

namespace RSSLTraceViewerGUI
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml;

    using RDMDictDecoder;
    using RSSLTraceDecoder.Utils;

    public partial class XMLFragmentsData
    {
       
        private static string DecodePartialUpdate(int fidId, string buffer)
        {
            string pattern = @"(\x1B\x5B|\x9B|\x5B)([0-9]+)\x60([^\x1B^\x5B^\x9B]+)";

            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regEx.Matches(buffer);
            var page = string.Empty;
            page = fidId <= 339 && fidId >= 315 ? string.Empty.PadRight(80) : string.Empty.PadRight(64);

            if (matches.Count <= 0) return buffer;

            for (var i = 0; i < matches.Count; i++)
            {
                var group = matches[i].Groups;
                var partialIndex = System.Convert.ToInt32(@group[2].ToString());
                var partialValue = @group[3].ToString();

                // replace updated value at the position 
                page = page.Remove(partialIndex, partialValue.Length);
                page = page.Insert(partialIndex, partialValue);
            }
            return page;
        }

        public static bool FieldValueToString(int fidId,string value,RdmFieldDictionary fieldDictionary,
            RdmEnumTypeDef enumTypeDef,out string outputStr,out string errorMsg)
        {
            outputStr = string.Empty;
            errorMsg = string.Empty;
            if (value.Trim() == string.Empty)
            {
                errorMsg = $"Fid {fidId} Data is null or empty";
                return false;
            }

            if (!fieldDictionary.GetDictEntryById(fidId, out var rdmEntry))
            {
                errorMsg = $"Unable to find fid {fidId} in data dictionary";
                return false;
            }

            var pBuffer = new string(value.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
            try
            {
                switch (rdmEntry.RwfType)
                {
                    case RwfTypeEnum.Buffer:
                        outputStr = pBuffer;
                        break;
                    case RwfTypeEnum.Date:
                        var dt = RdmDataConverter.HexStringToDateTime(pBuffer);
                        outputStr =
                            $"{(dt.HasValue ? $"{dt.Value.Day}/{dt.Value.Month}/{dt.Value.Year}" : string.Empty)}";
                        break;
                    case RwfTypeEnum.Enum:
                        outputStr = enumTypeDef.GetEnumDisplayValue(
                                        fidId,
                                        RdmDataConverter.HexStringToInt(pBuffer) ?? 0,
                                        out var display)
                                        ? $"{display.EnumDisplay.Replace("\"", string.Empty)}({RdmDataConverter.HexStringToInt(pBuffer)})"
                                        : $"Unknown Enum({int.Parse(pBuffer)})";
                        break;
                    case RwfTypeEnum.Int64:
                    case RwfTypeEnum.Uint64:
                        var intVal = RdmDataConverter.HexStringToInt(pBuffer) ?? 0;
                        outputStr = rdmEntry.Acronym.Contains("_MS")
                                        ? $"{RdmDataConverter.TimeMsToString(intVal)}"
                                        : $"{intVal}";
                        break;
                    case RwfTypeEnum.Real64:
                        outputStr = $"{RdmDataConverter.RealStringtoDouble(pBuffer).ToString()}";
                        break;
                    case RwfTypeEnum.RmtesString:
                        {
                            // Check if RMTES Header contains 0x1B 0x25 0x30 follow by UTF-8 string. Then remove the header 
                            if (pBuffer.StartsWith("1B2530"))
                            {
                                pBuffer = pBuffer.Remove(0, 6);
                                outputStr = Encoding.UTF8.GetString(RdmDataConverter.StringToByteArray(pBuffer.Trim()));
                            }
                            else if (pBuffer.StartsWith("1B5B"))
                            {
                                outputStr = DecodePartialUpdate(
                                    fidId,
                                    Encoding.UTF8.GetString(RdmDataConverter.StringToByteArray(pBuffer.Trim())));
                            }
                            else
                            {
                                outputStr = Encoding.UTF8.GetString(RdmDataConverter.StringToByteArray(pBuffer.Trim()));
                            }

                            var validXmlString = new StringBuilder();
                            validXmlString.Append(outputStr.Where(XmlConvert.IsXmlChar).ToArray());
                            outputStr = validXmlString.ToString();
                        }
                        break;
                    case RwfTypeEnum.AsciiString:

                        outputStr = Encoding.UTF8.GetString(RdmDataConverter.StringToByteArray(pBuffer.Trim()));
                        break;
                    case RwfTypeEnum.Time:
                        outputStr = $"{RdmDataConverter.HexStringToTime(pBuffer.Trim())}";
                        break;
                    case RwfTypeEnum.Unhandled:
                        outputStr = $"{value}";
                        break;
                }
            }
            catch (Exception exception)
            {
                errorMsg = $"Fid {fidId} {exception.Message}";
                return false;
            }

            return true;
        }
    }
}