namespace RSSLTraceDecoder
{
    public class XMLFragment
    {
        //FragmentNumber: is the order of XML fragment in the XML Tracing log file.
        public int FragmentNumber { get; set; }
        public string RdmMessageType { get; set; }
        public bool IsOutGoingMsg { get; set; }

        public bool IsIncomingMsg { get; set; }

        // MsgTypeRawXmlData: Use when found Incomming or Outgoing message
        public string MsgTypeRawXmlData { get; set; }
        public string RWFMajorMinorVersion { get; set; }
        public string RWFMajorMinorVersionRawXmlData { get; set; }
        // TimeStamp: Time when it receive Incomming or Outgoing message
        public string TimeStamp { get; set; }

        // TimeRawXmlData: Raw XML data for the Timestamp when it receive Incomming or Outgoing message
        public string TimeRawXmlData { get; set; }

        public string RawXmlData { get; set; }
    }
}