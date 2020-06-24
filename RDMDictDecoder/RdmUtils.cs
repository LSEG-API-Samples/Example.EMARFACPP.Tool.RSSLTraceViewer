using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RDMDictDecoder
{
    public class RdmUtils
    {
        public static RwfTypeEnum RwfStringToTypeEnum(string rwfTypeString)
        {
            switch (rwfTypeString)
            {
                case "UINT64":
                    return RwfTypeEnum.Uint64;
                case "INT64":
                    return RwfTypeEnum.Int64;
                case "ASCII_STRING":
                    return RwfTypeEnum.AsciiString;
                case "RMTES_STRING":
                    return RwfTypeEnum.RmtesString;
                case "REAL64":
                    return RwfTypeEnum.Real64;
                case "DATE":
                    return RwfTypeEnum.Date;
                case "TIME":
                    return RwfTypeEnum.Time;
                case "BUFFER":
                    return RwfTypeEnum.Buffer;
                case "ENUM":
                    return RwfTypeEnum.Enum;
                default:
                    return RwfTypeEnum.Unhandled;
            }
        }

        public static bool LoadEnumTypeDef(string filename, out RdmEnumTypeDef rdmEnumTypeDef, out string errorMsg)
        {
            var result = Task.Run(() => LoadEnumTypeDefAsync(filename)).Result;
            rdmEnumTypeDef = result.Item2;
            errorMsg = result.Item3;
            return result.Item1;
        }

        public static async Task<Tuple<bool, RdmEnumTypeDef, string>> LoadEnumTypeDefAsync(string filename)
        {
            var errorMsg = string.Empty;
            var enumFidList = new List<RdmEnumEntry>();
            var enumDisplayValueList = new List<RdmEnumDisplayValue>();
            var rdmEnumTypeDef = new RdmEnumTypeDef(enumFidList, enumDisplayValueList);
            if (!File.Exists(filename))
            {
                errorMsg += $"\nCannot find file {filename}";
                return new Tuple<bool, RdmEnumTypeDef, string>(false, rdmEnumTypeDef, errorMsg);
            }

            var enumIndex = 0;
            var isFidList = false;
            try
            {
                using (var fs = File.OpenRead(filename))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    string readLine;
                    while ((readLine = await sr.ReadLineAsync()) != null)
                        try
                        {
                            if (readLine[0] == '!') continue;
                            var splitStr = Regex.Matches(readLine, @"[\""].+?[\""]|[^ ]+")
                                .Cast<Match>()
                                .Select(column => column.Value)
                                .ToList();

                            if (!int.TryParse(splitStr[0], out var enumValue))
                            {
                                if (!isFidList)
                                {
                                    enumIndex++;
                                    isFidList = true;
                                }

                                var enumEntry = new RdmEnumEntry
                                {
                                    Acronym = splitStr[0],
                                    Fid = int.Parse(splitStr[1]),
                                    EnumGroupId = enumIndex
                                };
                                enumFidList.Add(enumEntry);
                            }
                            else
                            {
                                if (isFidList)
                                    isFidList = false;

                                var enumDisplayValue = new RdmEnumDisplayValue
                                {
                                    EnumGroupId = enumIndex,
                                    EnumValue = enumValue,
                                    EnumDisplay = splitStr[1]
                                };


                                if (splitStr.Count > 2)
                                {
                                    var tempArray = new string[splitStr.Count - 2];
                                    Array.Copy(splitStr.ToArray(), 2, tempArray, 0, splitStr.Count - 2);
                                    var meaningStr = string.Join(" ", tempArray);
                                    enumDisplayValue.EnumMeaning = meaningStr;
                                }
                                else
                                {
                                    enumDisplayValue.EnumMeaning = string.Empty;
                                }

                                enumDisplayValueList.Add(enumDisplayValue);
                            }
                        }
                        catch (Exception exception)
                        {
                            errorMsg = $"{exception.Message}\n{exception.StackTrace}";
                            return new Tuple<bool, RdmEnumTypeDef, string>(false, rdmEnumTypeDef, errorMsg);
                        }
                }

                return new Tuple<bool, RdmEnumTypeDef, string>(true, rdmEnumTypeDef, errorMsg);
            }
            catch (Exception exception)
            {
                errorMsg += $"\n{exception.Message}\n{exception.StackTrace}";
                return new Tuple<bool, RdmEnumTypeDef, string>(false, rdmEnumTypeDef, errorMsg);
            }
        }

        public static bool LoadRdmDictionary(string filename, out RdmFieldDictionary rdmDict, out string errorMsg)
        {
            var result = Task.Run(() => LoadRdmDictionaryAsync(filename)).Result;

            rdmDict = result.Item2;
            errorMsg = result.Item3;
            return result.Item1;
        }

        public static async Task<Tuple<bool, RdmFieldDictionary, string>> LoadRdmDictionaryAsync(string filename)
        {
            var rdmDict = new RdmFieldDictionary();
            var errorMsg = string.Empty;

            if (!File.Exists(filename))
            {
                errorMsg += $"\nCannot find file {filename}";
                return new Tuple<bool, RdmFieldDictionary, string>(false, rdmDict, errorMsg);
            }

            try
            {
                using (var fs = File.OpenRead(filename))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    string s;
                    while ((s = await sr.ReadLineAsync()) != null)
                        if (s[0] != '!')
                        {
                            var splitStr = Regex.Matches(s, @"[\""].+?[\""]|[^ ]+")
                                .Cast<Match>()
                                .Select(column => column.Value)
                                .ToList();
                            var isEnum = splitStr[4].Contains("ENUMERATED");

                            var entry = new RdmDictEntry();
                            entry.Acronym = splitStr[0];
                            entry.DDEAcronym = splitStr[1];
                            if (int.TryParse(splitStr[2], out var fid))
                            {
                                entry.Fid = fid;
                            }
                            else
                            {
                                entry.Fid = 0;
                                errorMsg += "\nUnable to get FID set it to 0 instead";
                            }

                            entry.RipplesTo = splitStr[3];
                            entry.FieldType = splitStr[4];

                            if (isEnum)
                            {
                                var newLength = $"{splitStr[5]} {splitStr[6]} {splitStr[7]} {splitStr[8]}";
                                entry.Length = newLength;
                                entry.RwfType = RwfStringToTypeEnum(splitStr[9]);
                                if (int.TryParse(splitStr[10], out var rwfLen))
                                {
                                    entry.RwfLen = rwfLen;
                                }
                                else
                                {
                                    entry.RwfLen = 0;
                                    errorMsg += "\nUnable to get RWFLEN set it to 0 instead";
                                }
                            }
                            else
                            {
                                entry.Length = splitStr[5];
                                entry.RwfType = RwfStringToTypeEnum(splitStr[6]);
                                if (int.TryParse(splitStr[7], out var rwfLen))
                                {
                                    entry.RwfLen = rwfLen;
                                }
                                else
                                {
                                    entry.RwfLen = 0;
                                    errorMsg += "\nUnable to get RWFLEN set it to 0 instead";
                                }
                            }

                            rdmDict.Add(entry.Fid, entry);
                        }
                }

                return new Tuple<bool, RdmFieldDictionary, string>(true, rdmDict, errorMsg);
            }
            catch (Exception ex)
            {
                errorMsg += $"\n{ex.Message}\n{ex.StackTrace}";
                return new Tuple<bool, RdmFieldDictionary, string>(false, rdmDict, errorMsg);
            }
        }
    }
}