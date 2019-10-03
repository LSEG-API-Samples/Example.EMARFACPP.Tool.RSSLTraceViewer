using System.Collections.Generic;
using System.Linq;

namespace RDMDictDecoder
{
    public class RdmFieldDictionary
    {
        //Use Fid/Field ID as dictionary key and RDMDictEntry is a field definition from RDMFieldDict
        private readonly Dictionary<int, RdmDictEntry> _dict;

        public RdmFieldDictionary()
        {
            _dict = new Dictionary<int, RdmDictEntry>();
        }

        public bool IsEmpty => !_dict.Any();

        public void Add(int fidId, RdmDictEntry entry)
        {
            _dict.Add(fidId, entry);
        }

        public void Remove(int fidId)
        {
            _dict.Remove(fidId);
        }

        public bool GetDictEntryById(int fidId, out RdmDictEntry rdmDictEntry)
        {
            rdmDictEntry = null;
            if (!_dict.ContainsKey(fidId))
                return false;

            rdmDictEntry = _dict[fidId];
            return true;
        }

        public bool GetDictEntryByAcronym(string fidname, out RdmDictEntry rdmDictEntry)
        {
            var result = from x in _dict.Values where x.Acronym == fidname select x;
            var rdmDictEntries = result as RdmDictEntry[] ?? result.ToArray();
            rdmDictEntry = rdmDictEntries.First();
            return rdmDictEntries.Any();
        }

        public bool GetFidIdByAcronym(string name, out int fidId)
        {
            fidId = 0;
            if (GetDictEntryByAcronym(name, out var rdmDictEntry))
            {
                fidId = rdmDictEntry.Fid;
                return true;
            }

            return false;
        }

        public bool GetAcronymByFidId(int fidId, out string acronymString)
        {
            if (!GetDictEntryById(fidId, out var rdmDictEntry))
            {
                acronymString = $"UnknownFid({fidId}";
                return false;
            }

            acronymString = rdmDictEntry.Acronym;
            return true;
        }

        // Get All dictionary as List<RDMDictEntry>
        public List<RdmDictEntry> GetAllDicts()
        {
            return _dict.Values.ToList();
        }
    }
}