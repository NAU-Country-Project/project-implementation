using NAUCountryA.Tables;
using System.Data;

namespace NAUCountryA.Models
{
    public class Commodity : IEquatable<Commodity>
    {
        public Commodity(int commodityCode, string commodityName, string commodityAbbreviation, char annualPlantingCode, int commodityYear, DateTime releasedDate, string recordTypeCode)
        {
            CommodityCode = commodityCode;
            CommodityName = commodityName;
            CommodityAbbreviation = commodityAbbreviation;
            AnnualPlantingCode = annualPlantingCode;
            CommodityYear = commodityYear;
            ReleasedDate = releasedDate;
            RecordType = Service.RecordTypeEntries[recordTypeCode];
        }

        public Commodity(DataRow row)
        :this((int)row["COMMODITY_CODE"], (string)row["COMMODITY_NAME"], (string)row["COMMODITY_ABBREVIATION"],row["ANNUAL_PLANTING_CODE"].ToString()[0], (int)row["COMMODITY_YEAR"], Service.ToDateTime(row["RELEASED_DATE"].ToString()), (string)row["RECORD_TYPE_CODE"])
        {
        }

        public int CommodityCode
        {
            get;
            private set;
        }

        public string CommodityName
        {
            get;
            private set;
        }

        public string CommodityAbbreviation
        {
            get;
            private set;
        }

        public char AnnualPlantingCode
        {
            get;
            private set;
        }

        public int CommodityYear
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

        public KeyValuePair<int,Commodity> Pair
        {
            get
            {
                return new KeyValuePair<int,Commodity>(CommodityCode, this);
            }
        }

        public bool Equals(Commodity other)
        {
            return CommodityCode == other.CommodityCode && CommodityName == other.CommodityName &&
            CommodityAbbreviation == other.CommodityAbbreviation && AnnualPlantingCode == other.AnnualPlantingCode &&
            CommodityYear == other.CommodityYear && Service.DateTimeEquals(ReleasedDate, other.ReleasedDate) &&
            RecordType == other.RecordType;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Commodity))
            {
                return false;
            }
            return Equals((Commodity)obj);
        }

        public string FormatCommodityCode()
        {
            if (CommodityCode < 10)
            {
                return $"\"000{CommodityCode}\"";
            }
            else if (CommodityCode < 100)
            {
                return $"\"00{CommodityCode}\"";
            }
            else if (CommodityCode < 1000)
            {
                return $"\"0{CommodityCode}\"";
            }
            return $"\"{CommodityCode}\"";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{FormatCommodityCode()},\"{CommodityName}\",\"{CommodityAbbreviation}\",\"" +
                $"{AnnualPlantingCode}\",\"{CommodityYear}\",\"{Service.ToString(ReleasedDate)}\"," +
                $"\"{RecordType.RecordTypeCode}\"";
        }

        public static bool operator== (Commodity a, Commodity b)
        {
            return a.Equals(b);
        }

        public static bool operator!= (Commodity a, Commodity b)
        {
            return !a.Equals(b);
        }
    }
}