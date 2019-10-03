using System.Collections.Generic;
using System.Linq;

namespace RDMDictDecoder
{
    public class RdmEnumTypeDef
    {
        private readonly List<RdmEnumDisplayValue> _enumDisplayValues;
        private readonly List<RdmEnumEntry> _enumEntries;

        public RdmEnumTypeDef(List<RdmEnumEntry> enumEntries, List<RdmEnumDisplayValue> enumDisplayValues)
        {
            _enumDisplayValues = enumDisplayValues;
            _enumEntries = enumEntries;
        }
        //Return all fids/fields listed in enumertype.def
        public Dictionary<string, int> EnumeratedFields
        {
            get { return _enumEntries.ToDictionary(x => x.Acronym, x => x.Fid); }
        }

        //Each enumerate set will have its own GroupID or Set number this function will return the list of set number as List<int>
        public List<int> EnumeratedGroups
        {
            get { return _enumEntries.Select(x => x.EnumGroupId).ToList(); }
        }

        

        //This function passing fidId and return GroupID or Set number 
        public int GetFidGroupId(int fidId)
        {
            var groupId = from x in _enumEntries where x.Fid == fidId select x.EnumGroupId;
            var enumerable = groupId as int[] ?? groupId.ToArray();
            return enumerable.Any() ? enumerable.First() : 0;
        }

        //This function used to get field list which is a member of the Set number or GroupID.
        public bool GetFieldListFromGroupId(int groupId, out List<RdmEnumDisplayValue> fidList)
        {
            fidList = new List<RdmEnumDisplayValue>();
            var retDisplayValues = from x in _enumDisplayValues where x.EnumGroupId == groupId select x;
            var rdmEnumDisplayValues = retDisplayValues as RdmEnumDisplayValue[] ?? retDisplayValues.ToArray();
            if (!rdmEnumDisplayValues.Any())
                return false;

            fidList = rdmEnumDisplayValues.ToList();
            return true;
        }

        // This function used to get the list of RDMEnumDisplayValue by passing Set number or Group Id.
        public bool GetDisplayValueFromGroupId(int groupId, out List<RdmEnumDisplayValue> displayList)
        {
            var ret = from x in _enumDisplayValues where x.EnumGroupId == groupId select x;
            displayList= ret!=null?ret.ToList():new List<RdmEnumDisplayValue>();
            return displayList.Count > 0;
        }

        //Used to get Enum display string by passing fid id and enum value;
        //This function return true if the enum value for the fid exists and return false if it's not available. 
        //The displayValue will be empty in case that it return false; So user can re-format the string themselves.
        public bool GetEnumDisplayString(int fidId, int enumValue, out string displayValue)
        {
            var ret = GetEnumDisplayValue(fidId, enumValue, out var enumDisplay);
            
            displayValue = ret?enumDisplay.EnumDisplay:string.Empty;
            return ret;
        }
        // This function used to get EnumDisplayString by passing fid Id and Enum value.
        // The difference between this function and above function is just the case that it's unknown value.
        // The return value will be "Unknown(enumValue)" instead.
        public string GetEnumDisplayString(int fidId, int enumValue)
        {
            return GetEnumDisplayValue(fidId, enumValue, out var enumDisplay)
                ? enumDisplay.EnumDisplay
                : $"Unknown({enumValue}";
        }

        //This function is the main method to get enum value by passing fidId and enumValue and return true/false when it found or not found the value in enumtype.def.
        //This function will return RDMEnumDisplayValue object which contains all information about the enum value including the description.
        public bool GetEnumDisplayValue(int fidId, int enumValue, out RdmEnumDisplayValue displayValue)
        {
            displayValue = null;
            var groupId = from x in _enumEntries where x.Fid == fidId select x.EnumGroupId;
            var enumerable = groupId as int[] ?? groupId.ToArray();
            if (!enumerable.Any()) return false;
            var enumDisplay = from y in _enumDisplayValues
                where y.EnumGroupId == enumerable.First() && y.EnumValue == enumValue
                select y;

            var enumDisplayValues = enumDisplay as RdmEnumDisplayValue[] ?? enumDisplay.ToArray();
            if (!enumDisplayValues.Any()) return false;
            displayValue = enumDisplayValues.First();
            return true;
        }

        // Return all Fields that sharing the same enumerate set
        // Return null if fid id is not available in the enumerate dict
        // Return blank Dictionary<string,int> if it does have other Fields sharing the same enumerate set/values
        public bool GetFieldListFromEnumeratedDict(int fidId, out Dictionary<string, int> fidList)
        {
            fidList = new Dictionary<string, int>();
            var groupId = from x in _enumEntries where x.Fid == fidId select x.EnumGroupId;
            var enumerable = groupId as int[] ?? groupId.ToArray();
            if (!enumerable.Any())
            {
                fidList = null;
                return false;
            }

            var enumDisplay = from y in _enumEntries
                where y.EnumGroupId == enumerable.First() && y.Fid != fidId
                select y;

            var rdmEnumEntries = enumDisplay as RdmEnumEntry[] ?? enumDisplay.ToArray();
            if (!rdmEnumEntries.Any()) return false;

            fidList = rdmEnumEntries.ToDictionary(x => x.Acronym, x => x.Fid);
            return true;
        }

        public void GetEnumValuesByFidId(int fidId, out List<RdmEnumDisplayValue> enumDisplayList)
        {
            enumDisplayList = new List<RdmEnumDisplayValue>();
            var groupId = from x in _enumEntries where x.Fid == fidId select x.EnumGroupId;
            var enumerable = groupId as int[] ?? groupId.ToArray();
            if (!enumerable.Any()) return;

            var enumDisplay = from y in _enumDisplayValues
                where y.EnumGroupId == enumerable.First()
                select y;
            enumDisplayList = enumDisplay.ToList();
        }
    }
}