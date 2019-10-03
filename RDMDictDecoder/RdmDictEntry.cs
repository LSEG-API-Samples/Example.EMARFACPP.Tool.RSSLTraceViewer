namespace RDMDictDecoder
{
    public enum RwfTypeEnum
    {
        Unhandled = 0,
        Int64 = 1,
        Uint64 = 2,
        Real64 = 3,
        Buffer = 4,
        RmtesString = 5,
        AsciiString = 6,
        Time = 7,
        Date = 8,
        Enum = 9
    }

    public class RdmDictEntry
    {
        public string Acronym { get; set; }
        public string DDEAcronym { get; set; }
        public int Fid { get; set; }
        public string RipplesTo { get; set; }
        public string FieldType { get; set; }
        public string Length { get; set; }
        public RwfTypeEnum RwfType { get; set; }
        public int RwfLen { get; set; }
    }
}