using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RSSLTraceDecoder.Utils;

namespace RSSLTraceDecoder
{
    namespace MRN
    {
        public class MrnFragmentDecoder
        {
           

            public static bool DecodeMrnData(XmlFragmentList fragmentList,out MrnMsgList mrnList, out string errorMsg)
            {
                var result = Task.Run(() => DecodeMrnDataAsync(fragmentList)).Result;
                mrnList = result.Item2;
                errorMsg = result.Item3;
                return result.Item1;

            }
            public static async Task<Tuple<bool,MrnMsgList,string>> DecodeMrnDataAsync(XmlFragmentList fragmentList)
            {
                var mrnList = new MrnMsgList();
                var errorMsg = string.Empty;
                await Task.Run( () =>
                {            
                    try
                    {

                        List<XElement> elements;
                        foreach (var fragment in fragmentList.Fragments)
                        {
                            //process Attribues 
                            if(string.IsNullOrEmpty(fragment.RawXmlData))
                                continue;

                            var list = XmlHelper.GetElement(fragment.RawXmlData, fragment.RdmMessageType);
                            elements = list.ToList();
                            if (!elements.Any()) continue;


                            var mrndata = new MrnMsg(StringtoType(fragment.RdmMessageType));

                            if (elements.First().HasAttributes)
                            {
                                var elementAttribs = new Dictionary<string, string>();
                                foreach (var attrib in elements.First().Attributes())
                                    elementAttribs.Add(attrib.Name.LocalName, attrib.Value);

                                // The following check is for EMA Java as it use domainType News_TEXT_ANALYTICS rather than RSSL_DMT_NEWS_TEXT_ANALYTICS as C++ and .NET
                                mrndata.DomainType = elementAttribs["domainType"] == "NEWS_TEXT_ANALYTICS"
                                    ? "RSSL_DMT_NEWS_TEXT_ANALYTICS"
                                    : elementAttribs["domainType"];
                                mrndata.SetFragmentAttribs(elementAttribs);
                            }

                            //process Key
                            var keys = XmlHelper.GetElement(fragment.RawXmlData, "key");
                            var enumerable = keys.ToList();
                            if (enumerable.Any())
                            {
                                mrndata.ContainsKey = true;
                                var keysAttrib = new Dictionary<string, string>();
                                foreach (var key in enumerable.First().Attributes())
                                {
                                    if (key.Name == "name")
                                    {
                                        mrndata.RequestKeyName = key.Value;
                                    }
                                    keysAttrib.Add(key.Name.LocalName, key.Value);
                                }
                                mrndata.SetKeyAtrribs(keysAttrib);
                            }

                            if (mrndata.DomainType == "RSSL_DMT_NEWS_TEXT_ANALYTICS" ||
                                mrndata.DomainType == "RSSL_DMT_MARKET_PRICE")
                            {
                                //process fieldList
                                var fields = XmlHelper.GetElement(fragment.RawXmlData, "fieldEntry");
                                var xElements = fields.ToList();
                                if (xElements.Any())
                                {
                                    mrndata.ContainsFieldList = true;
                                    var fieldEntrys = new Dictionary<int, string>();
                                    foreach (var elem in xElements)
                                    {
                                        var fid = from id in elem.Attributes()
                                            where id.Name == "fieldId"
                                            select id.Value;
                                        var fidId = int.Parse(fid.First().Trim());
                                        var fValue = from fieldvalue in elem.Attributes()
                                            where fieldvalue.Name == "data"
                                            select fieldvalue.Value;
                                        var pBuffer = new string(fValue.First().ToCharArray()
                                            .Where(c => !char.IsWhiteSpace(c))
                                            .ToArray());
                                        //4271 is GUID.
                                        fieldEntrys.Add(fidId,
                                            fidId == 4271
                                                ? Encoding.UTF8.GetString(
                                                    RdmDataConverter.StringToByteArray(pBuffer.Trim()))
                                                : pBuffer);
                                    }

                                    mrndata.SetFieldList(fieldEntrys);
                                }
                            }

                            mrndata.FragmentNumber = fragment.FragmentNumber;
                            mrnList.Add(fragment.FragmentNumber, mrndata);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = $"DecodeMrnData error {ex.Message}\r\n{ex.StackTrace}";
                        return new Tuple<bool, MrnMsgList, string>(false, mrnList, errorMsg);
                    }

                    return new Tuple<bool, MrnMsgList, string>(true, mrnList, errorMsg);
                });
                return new Tuple<bool, MrnMsgList, string>(true, mrnList, errorMsg);
            }

            public static bool UnpackMrnData(IEnumerable<MrnMsg> list, out string jsonString, out string errorMsg)
            {
                var result = Task.Run(() => UnpackMrnDataAsync(list)).Result;
                jsonString = result.Item2;
                errorMsg = result.Item3;
                return result.Item1;

            }
            public static async Task<Tuple<bool,string,string>> UnpackMrnDataAsync(IEnumerable<MrnMsg> list)
            {
                var jsonString = string.Empty;
                var errorMsg = string.Empty;
                var bSuccess = true;
                await Task.Run(async () =>
                {
                  
                    try
                    {
                        var totSize = 0;
                        var bytesArraySize = 0;
                        jsonString = string.Empty;
                        errorMsg = string.Empty;

                        var bytesStr = new StringBuilder();
                        foreach (var data in list)
                        {
                            if (!data.ContainsFieldList) continue;
                            if (data.GetFieldList().ContainsKey(32480))
                                totSize = RdmDataConverter.HexStringToInt(data.GetFieldList()[32480])??0;

                            if (data.GetFieldList().ContainsKey(32641))
                                bytesStr.Append(RdmDataConverter.TraceStringToString(data.GetFieldList()[32641]));
                        }

                        var pByteArray = RdmDataConverter.StringToByteArray(bytesStr.ToString());
                        bytesArraySize = pByteArray.Length;
                        if (bytesArraySize != totSize)
                        {
                            errorMsg =
                                $"Detect incomplete XML Fragments list\r\nCurrent XML Fragments size is {bytesArraySize} while Total Size is {totSize}\r\n";
                            errorMsg += $"Data might be corrupted or missing some fragment";
                            bSuccess = false;
                            return new Tuple<bool, string, string>(bSuccess, jsonString, errorMsg);
                        }

                        using (var compressedStream = new MemoryStream(pByteArray))
                        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                        using (var resultStream = new MemoryStream())
                        {
                            await zipStream.CopyToAsync(resultStream).ConfigureAwait(false); 
                            jsonString = Encoding.UTF8.GetString(resultStream.ToArray());
                        }

                        return new Tuple<bool, string, string>(bSuccess, jsonString, errorMsg);
                    }
                    catch (Exception ex)
                    {
                        errorMsg = $"Exception Error: {ex.Message}";
                        jsonString = "";
                        bSuccess = false;
                        return new Tuple<bool, string, string>(bSuccess, jsonString, errorMsg);
                    }
             
                });

                return new Tuple<bool, string, string>(bSuccess, jsonString, errorMsg);
            }
            public static MsgType StringtoType(string elementName)
            {
                if (elementName.ToLower().Contains("requestmsg"))
                    return MsgType.Request;
                if (elementName.ToLower().Contains("refreshmsg"))
                    return MsgType.Refresh;
                if (elementName.ToLower().Contains("updatemsg"))
                    return MsgType.Update;
                if (elementName.ToLower().Contains("closeMsg"))
                    return MsgType.Close;
                return MsgType.Unknown;
            }
        }
    }
}