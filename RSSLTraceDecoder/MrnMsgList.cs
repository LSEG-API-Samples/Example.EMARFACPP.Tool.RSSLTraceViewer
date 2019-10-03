using System.Collections.Generic;
using System.Linq;
using RSSLTraceDecoder.Utils;

namespace RSSLTraceDecoder.MRN
{
    public class MrnMsgList
    {
        private readonly Dictionary<int, MrnMsg> _mrnMsgList;

        public MrnMsgList()
        {
            _mrnMsgList = new Dictionary<int, MrnMsg>();
        }

        public int Count => _mrnMsgList.Count;

        public void Add(int index, MrnMsg msg)
        {
            if (_mrnMsgList.ContainsKey(index))
                _mrnMsgList[index] = msg;
            else
                _mrnMsgList.Add(index, msg);
        }

        public MrnMsg Get(int index)
        {
            if(_mrnMsgList.ContainsKey(index))
                  return _mrnMsgList[index];
            else
                return null;

        }

        public IEnumerable<MrnMsg> GetXMLFragmentsByDomainType(string domainType)
        {
            var list = from x in _mrnMsgList.Values
                where x.DomainType.Contains(domainType)
                select x;
            return list;
        }

        public List<MrnMsg> GetMrnByGuid(string guid)
        {
            var list = from x in GetXMLFragmentsByDomainType("RSSL_DMT_NEWS_TEXT_ANALYTICS")
                where x.ContainsFieldList && x.GetFieldList().ContainsKey(4271) &&
                      x.GetFieldList().ContainsValue(guid)
                select x;


            return list.ToList();
        }

        public List<MrnMsg> GetMrnByGuid(string guid, int fragmentNumber)
        {
            var dict = from x in GetMsgList()
                where x.Key >= fragmentNumber && x.Value.ContainsFieldList &&
                      x.Value.GetFieldList().ContainsKey(4271) && x.Value.GetFieldList().ContainsValue(guid)
                select x;

            var list = new List<MrnMsg>();
            foreach (var data in dict)
            {
                var bDuplicate = false;
                foreach (var mrn in list)
                {
                    if (!mrn.ContainsFieldList) continue;
                    if (RdmDataConverter.HexStringToInt(mrn.GetFieldList()[32479]) !=
                        RdmDataConverter.HexStringToInt(data.Value.GetFieldList()[32479]))
                        continue;
                    bDuplicate = true;
                    break;
                }

                if (!bDuplicate)
                    list.Add(data.Value);
            }

            return list;
        }

        public IEnumerable<string> GetMrnGuidList()
        {
            var list = from x in GetXMLFragmentsByDomainType("RSSL_DMT_NEWS_TEXT_ANALYTICS")
                where x.ContainsFieldList && x.GetFieldList().ContainsKey(4271) &&
                      x.GetFieldList()[4271] != string.Empty && x.GetFieldList().ContainsKey(32480)
                select x.GetFieldList()[4271];

            return list.Distinct();
        }

        public Dictionary<int, MrnMsg> GetMsgList()
        {
            return _mrnMsgList;
        }
    }
}