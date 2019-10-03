using System;

namespace RSSLTraceDecoder
{
    public class MrnFieldData
    {
        private string _guid;

        // Expected to recieve MsgType Refresh or Update
        public MsgType MsgType { get; set; }

        public int? PROD_PERM { get; set; }
        public DateTime? ACTIV_DATE { get; set; }
        public int? RECORDTYPE { get; set; }
        public int? RDN_EXCHD2 { get; set; }
        public string TIMEACTS_MS { get; set; }

        public string GUID
        {
            get => _guid ?? string.Empty;
            set => _guid = value;
        }

        public double? CONTEXT_ID { get; set; }
        public int? DDS_DSO_ID { get; set; }
        public string SPS_SP_RIC { get; set; }
        public string MRN_V_MAJ { get; set; }
        public string MRN_TYPE { get; set; }
        public string MRN_V_MIN { get; set; }
        public string MRN_SRC { get; set; }
        public int? FRAG_NUM { get; set; }
        public int? TOT_SIZE { get; set; }
        public string FRAGMENT { get; set; }
    }
}