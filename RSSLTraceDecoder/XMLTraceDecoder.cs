using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RSSLTraceDecoder
{
    public enum MsgType
    {
        Request = 0,
        Refresh = 1,
        Update = 2,
        Close = 3,
        Unknown = 4
    }
    public class XMLTraceDecoder
    {
   
        public static bool LoadXmlFile(string xmlFilePath, out XmlFragmentList fragments, out string errorMsg,bool useRDMStructMode=true)
        {
            var result=LoadXmlFileAsync(xmlFilePath,useRDMStructMode).GetAwaiter().GetResult();
            fragments = result.Item2;
            errorMsg = result.Item3;
            return result.Item1;
        }

      
        public static async Task<Tuple<bool,XmlFragmentList, string>> LoadXmlFileAsync(string xmlFilePath,bool useRDMStrictMode=true)
        {
            var errorMsg = string.Empty;
            if (!File.Exists(xmlFilePath))
            {
                errorMsg = $"Unable to load file from {xmlFilePath}";
                return new Tuple<bool, XmlFragmentList, string>(false, null, errorMsg);
            }

            using (var fs = new FileStream(xmlFilePath, FileMode.Open))
            {
                var task = await ProcessXmlDataAsync(fs,useRDMStrictMode);
                return new Tuple<bool, XmlFragmentList, string>(task.Item1,task.Item2,task.Item3);
            }


        }

        // ProcessXMLDataAsync method, used to parse the the XML data, input is Stream 
        // Returns multiple values as the method is async method so it's not allow out or ref syntax
        // Return value as <success(boolean),XMLFragmentList,error message>
        private static async Task<Tuple<bool,XmlFragmentList, string>> ProcessXmlDataAsync(Stream xmlStream,bool useRDMStrictMode=true)
        {
            var errorMessage = string.Empty;
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, Async = true };
            var fragments = new XmlFragmentList();
            var xmlFragmentIndex = 0;
            bool bSuccess = true;
            bool bValidTag= false;
            using (var reader = XmlReader.Create(xmlStream, settings))
            {
                try
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Comment:
                                {
                                    if (useRDMStrictMode)
                                    {

                                        var commentVal = reader.Value.Trim();
                                        if (commentVal.ToLower().StartsWith("incoming")
                                            || commentVal.ToLower().StartsWith("outgoing"))
                                        {
                                            xmlFragmentIndex++;
                                            var fragment = new XMLFragment
                                                               {
                                                                   FragmentNumber = xmlFragmentIndex,
                                                                   IsIncomingMsg =
                                                                       commentVal.ToLower().StartsWith("incoming"),
                                                                   IsOutGoingMsg =
                                                                       commentVal.ToLower().StartsWith("outgoing"),
                                                                   MsgTypeRawXmlData = reader.Value
                                                               };
                                            bValidTag = true;
                                            fragments.Add(xmlFragmentIndex, fragment);
                                        }
                                        else if (bValidTag && commentVal.ToLower().StartsWith("rwfmajorver"))
                                        {
                                            fragments.Get(xmlFragmentIndex).RWFMajorMinorVersion = commentVal;
                                            fragments.Get(xmlFragmentIndex).RWFMajorMinorVersionRawXmlData =
                                                reader.Value;
                                        }
                                        else if (bValidTag && StartWithDayOfWeek(commentVal.ToLower())) // For EMA Java Trace
                                        {

                                            fragments.Get(xmlFragmentIndex).TimeStamp = commentVal;
                                            fragments.Get(xmlFragmentIndex).TimeRawXmlData = reader.Value;
                                        }
                                        else if (bValidTag && commentVal.ToLower().StartsWith("time:") && fragments.Count > 0)
                                        {
                                            var strArray = commentVal.Split();
                                            var timeStr = strArray.Length > 1 ? (string)strArray[1].Trim() : "";

                                            fragments.Get(xmlFragmentIndex).TimeStamp = timeStr;
                                            fragments.Get(xmlFragmentIndex).TimeRawXmlData = reader.Value;
                                        }
                                    }

                                    await reader.SkipAsync().ConfigureAwait(false);
                                }
                                break;

                            case XmlNodeType.Element:
                                {
                                    if (!useRDMStrictMode)
                                    {
                                        xmlFragmentIndex++;
                                        var fragment = new XMLFragment
                                                           {
                                                               FragmentNumber = xmlFragmentIndex,
                                                               IsIncomingMsg = false,
                                                               IsOutGoingMsg = false,
                                                               MsgTypeRawXmlData = String.Empty
                                                           };

                                        fragments.Add(xmlFragmentIndex, fragment);
                                    }

                                   
                                    if (fragments.Get(xmlFragmentIndex) == null)
                                    {
                                        bSuccess = false;
                                        errorMessage = $"Cannot find Fragment# {xmlFragmentIndex} in the list\r\nThere could be problem from missing required comments before parsing the XML Element\r\n";
                                 
                                        continue;
                                        
                                    }
                                    fragments.Get(xmlFragmentIndex).RdmMessageType = reader.Name;
                                    var nodeElem = XNode.ReadFrom(reader);
                                    var xmlRawValue = nodeElem;
                                    fragments.Get(xmlFragmentIndex).RawXmlData = xmlRawValue.ToString();
                                    bValidTag = false;
                                }
                                break;
                            default:
                                await reader.SkipAsync().ConfigureAwait(false);
                                continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    bSuccess = false;
                    fragments.Remove(xmlFragmentIndex);
                    errorMessage = $"Detect and error when parsing the XML fragment\r\n{ex.Message}\r\nFragment {xmlFragmentIndex} has been removed";
                    await reader.SkipAsync();
                }
                
            }

            return new Tuple<bool,XmlFragmentList, string>(bSuccess,fragments, errorMessage);

        }
        private static bool StartWithDayOfWeek(string stringVal)
        {
            var stringArray = stringVal.Split(' ');
            if (stringArray.Length > 0)
            {
                var testString = stringArray[0].Trim().ToLower();
                string[] DayOfWeek = {"Mon", "Tue", "Wen", "Thu", "Fri", "Sat", "Sun"};
                return DayOfWeek.Any(data => testString.Contains(data.ToLower()));
            }

            return false;

        }
       
    }
}