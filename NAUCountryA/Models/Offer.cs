using NAUCountryA.Tables;
using System.Data;

namespace NAUCountryA.Models
{
    // DESIGN NOTE: The main issue with the errors was some missing parenthesis characters causing the class structure to get out of wack.
    //   Start fixing errors from the top of the class and work your way down and it can resolve most errors.  The main error was misleading
    //   as it was a symptom of the real issues.

    public class Offer : IEquatable<Offer>
    {
        public Offer(int offerID, int practiceCode, int countyCode, int typeCode, int irrigationPracticeCode)
        {
            OfferID = offerID;
            IReadOnlyDictionary<int, Practice> practiceEntries = new PracticeTable();
            this.Practice = practiceEntries[practiceCode];
            IReadOnlyDictionary<int, County> countyEntries = new CountyTable();
            County = countyEntries[countyCode];
            IReadOnlyDictionary<int, NauType> typeEntries = new TypeTable();
			NauType = typeEntries[typeCode];
            IrrigationPracticeCode = irrigationPracticeCode;

        }

        public Offer(DataRow row)
        : this((int)row["ADM_INSURANCE_OFFER_ID"], (int)row["PRACTICE_CODE"], (int)row["COUNTY_CODE"], (int)row["TYPE_CODE"], (int) row["IRRIGATION_PRACTICE_CODE"])
        {
        }

        public int OfferID
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

        public NauType NauType
        {
            get;
            private set;
        }

        public int IrrigationPracticeCode
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
            County == other.County && NauType == other.NauType && IrrigationPracticeCode == other.IrrigationPracticeCode;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Offer))
            {
                return false;
            }
            return Equals((Offer)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
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