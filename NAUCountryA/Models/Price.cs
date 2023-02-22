using System;
using System.Collections.Generic;
using NAUCountryA.Tables;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NAUCountryA.Models
{
    public class Price : IEquatable<Price> 
    {
        public Price(int offerID, double expectedIndexValue)
        {
            IReadOnlyDictionary<int,Offer> offerEntries = new OfferTable();
            Offer = offerEntries[offerID];
            ExpectedIndexValue = expectedIndexValue;
        }
        
        public Price(DataRow row)
        :this((int)row["OFFER_ID"], (double)row["EXPECTED_INDEX_VALUE"])
        {
        }

        public Offer Offer
        {
            get;
            private set;
        }

        public double ExpectedIndexValue
        {
            get;
            private set;
        }

        public bool Equals(Price other)
        {
            return Offer == other.Offer && ExpectedIndexValue== other.ExpectedIndexValue;
        }

        public override bool Equals(object obj)
        {
           if(!(obj is Price))
            {
                return false;
            }
           return Equals((Price)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static bool operator ==(Price a, Price b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Price a, Price b)
        {
            return !a.Equals(b);
        }
    }
}