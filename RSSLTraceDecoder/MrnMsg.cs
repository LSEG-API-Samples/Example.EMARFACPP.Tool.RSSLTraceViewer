using System.Collections.Generic;

namespace RSSLTraceDecoder
{
    namespace MRN
    {
        public class MrnMsg
        {
            private Dictionary<string, string> _fragmentAttribs;

            private Dictionary<string, string> _keyAtrribs;

            private Dictionary<int, string> fieldList;

            public MrnMsg(MsgType fragmentType)
            {
                FragmentType = fragmentType;
                SetFragmentAttribs(new Dictionary<string, string>());
                SetKeyAtrribs(new Dictionary<string, string>());
                ContainsKey = false;
                ContainsFieldList = false;
            }

            public MsgType FragmentType { get; set; }
            public int FragmentNumber { get; set; }
            public string DomainType { get; set; }

            public bool ContainsKey { get; set; }
            public string RequestKeyName { get; set; }

            public bool ContainsFieldList { get; set; }

            public Dictionary<string, string> GetFragmentAttribs()
            {
                return _fragmentAttribs;
            }

            public void SetFragmentAttribs(Dictionary<string, string> value)
            {
                _fragmentAttribs = value;
            }

            public Dictionary<string, string> GetKeyAtrribs()
            {
                return _keyAtrribs;
            }

            public void SetKeyAtrribs(Dictionary<string, string> value)
            {
                _keyAtrribs = value;
            }

            public Dictionary<int, string> GetFieldList()
            {
                return fieldList;
            }

            public void SetFieldList(Dictionary<int, string> value)
            {
                fieldList = value;
            }
        }
    }
}