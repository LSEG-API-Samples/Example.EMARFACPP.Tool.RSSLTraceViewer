using System.Collections.Generic;
using System.Linq;

namespace RSSLTraceDecoder
{
    public class XmlFragmentList
    {
        public XmlFragmentList()
        {
            XMLFragments = new Dictionary<int, XMLFragment>();
        }

        public int Count => XMLFragments.Count;
        public IEnumerable<XMLFragment> Fragments => XMLFragments.Values.ToList();
        public Dictionary<int, XMLFragment> XMLFragments { get; }

        public void Add(int index, XMLFragment fragment)
        {
            if (XMLFragments != null && XMLFragments.ContainsKey(index))
                XMLFragments[index] = fragment;
            else
                XMLFragments?.Add(index, fragment);
        }

        public bool Remove(int index)
        {
            return XMLFragments.Remove(index);
        }

        public XMLFragment Get(int index)
        {
            return XMLFragments.ContainsKey(index) ? XMLFragments[index] : null;
        }
    }
}