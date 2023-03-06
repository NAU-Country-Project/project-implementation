using NAUCountryA.Tables;
using System.Collections.Generic;
using System.Data;

namespace NAUCountryA.Models
{
    public class Offer : IEquatable<Offer>
    {
        public Offer(int offerID, int stateCode, int practiceCode, int countyCode, int typeCode, int irrigationPracticeCode, int year)
        {
            OfferID = offerID;
            State = Service.StateEntries[stateCode];
            Practice = Service.PracticeEntries[practiceCode];
            County = Service.CountyEntries[countyCode];
            Type = Service.TypeEntries[typeCode];
            IrrigationPracticeCode = irrigationPracticeCode;
            Year = year;
        }

        public Offer(DataRow row)
        : this((int)row["ADM_INSURANCE_OFFER_ID"], (int)row["STATE_CODE"], (int)row["PRACTICE_CODE"], (int)row["COUNTY_CODE"], (int)row["TYPE_CODE"], (int)row["IRRIGATION_PRACTICE_CODE"], (int)row["YEAR"])
        {
        }

        public int OfferID
        {
            get;
            private set;
        }

        public State State
        {
            get;
            private set;
        }

        public Practice Practice
        {
            get;
            private set;
        }

        public County County
        {
            get;
            private set;
        }

        public NAUType Type
        {
            get;
            private set;
        }

        public int IrrigationPracticeCode
        {
            get;
            private set;
        }

        public int Year
        {
            get;
            private set;
        }


        public KeyValuePair<int, Offer> Pair
        {
            get
            {
                return new KeyValuePair<int, Offer>(OfferID, this);
            }
        }

        public bool Equals(Offer other)
        {
            return OfferID == other.OfferID && Practice == other.Practice &&
            County == other.County && Type == other.Type && IrrigationPracticeCode == other.IrrigationPracticeCode
            && Year == other.Year;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Offer))
            {
                return false;
            }
            return Equals((Offer)obj);
        }

        public string FormatOfferID()
        {
            if (OfferID < 10)
            {
                return $"\"0000000{OfferID}\"";
            }
            else if (OfferID < 100)
            {
                return $"\"000000{OfferID}\"";
            }
            else if (OfferID < 1000)
            {
                return $"\"00000{OfferID}\"";
            }
            else if (OfferID < 10000)
            {
                return $"\"0000{OfferID}\"";
            }
            else if (OfferID < 100000)
            {
                return $"\"000{OfferID}\"";
            }
            else if (OfferID < 1000000)
            {
                return $"\"00{OfferID}\"";
            }
            else if (OfferID < 10000000)
            {
                return $"\"0{OfferID}\"";
            }
            return $"\"{OfferID}\"";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{FormatOfferID()},{State.FormatStateCode()},{Practice.FormatPracticeCode()}," +
                $"{County.FormatCountyCode()},{Type.FormatTypeCode()},\"{IrrigationPracticeCode}\"," +
                $"{Year}";
        }

        public static bool operator ==(Offer a, Offer b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Offer a, Offer b)
        {
            return !a.Equals(b);
        }
    }
}