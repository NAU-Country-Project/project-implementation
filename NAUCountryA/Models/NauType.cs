using NAUCountryA.Tables;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NAUCountryA.Models
{
    // DESIGN NOTE: Changing the name to something like NAUType will help prevent conflicts with System.Type throughout your code.

    public class NauType : IEquatable<NauType>
    {
        public NauType(int typeCode, string typeName, string typeAbbreviation, 
            int commodityCode, DateTime releasedDate, string recordTypeCode)
        {
            TypeCode = typeCode;
            TypeName = typeName;
            TypeAbbreviation = typeAbbreviation;
            IReadOnlyDictionary<int,Commodity> commodityEntries = new CommodityTable();
            Commodity = commodityEntries[commodityCode];
            ReleasedDate = releasedDate;
            IReadOnlyDictionary<string,RecordType> recordTypeEntries = new RecordTypeTable();
            RecordType = recordTypeEntries[recordTypeCode];
        }

        public NauType(DataRow row)
        :this((int)row["TYPE_CODE"], (string)row["TYPE_NAME"], (string)row["TYPE_ABBREVIATION"], (int)row["COMMODITY_CODE"], (DateTime)row["RELEASED_DATE"], (string)row["RECORD_TYPE_CODE"])
        {
        }


        public int TypeCode
        {
            get;
            private set;
        }

        public string TypeName
        {
            get;
            private set;
        }

        public string TypeAbbreviation
        {
            get;
            private set;
        }

        public Commodity Commodity
        {
            get;
            private set;
        }

        public DateTime ReleasedDate
        {
            get;
            private set;
        }

        public RecordType RecordType
        {
            get;
            private set;
        }

        public bool Equals(NauType other)
        {
            return TypeCode == other.TypeCode &&
                TypeName == other.TypeName &&
                TypeAbbreviation == other.TypeAbbreviation &&
                Commodity == other.Commodity &&
                Service.DateTimeEquals(ReleasedDate, other.ReleasedDate) &&
                RecordType == other.RecordType;

        }

        public override bool Equals(object obj)
        {
            if (!(obj is NauType))
            {
                return false;
            }
            return Equals((NauType)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static bool operator ==(NauType a, NauType b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NauType a, NauType b)
        {
            return !a.Equals(b);
        }
    }
}