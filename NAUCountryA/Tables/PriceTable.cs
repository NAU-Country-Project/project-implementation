using NAUCountryA.Models;
using Npgsql;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAUCountryA.Tables
{
    public class PriceTable : IReadOnlyDictionary<Offer, Price>
    {
        public PriceTable()
        {
            ConstructTable();
            TrimEntries();
            AddEntries();
        }

        public int Count
        {
            get
            {
                return Table.Rows.Count;
            }
        }

        public Price this[Offer offer]
        {
            get
            {
                string sqlCommand = $"SELECT * FROM public.\"Price\" WHERE \"OFFER_ID\" = {offer.OfferID};";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The OFFER_ID: {offer.OfferID} doesn't exist.");
                }
                return new Price(table.Rows[0]);
            }
        }

        public IEnumerable<Offer> Keys
        {
            get
            {
                ICollection<Offer> keys = new HashSet<Offer>();
                foreach (KeyValuePair<Offer, Price> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<Price> Values
        {
            get
            {
                ICollection<Price> values = new List<Price>();
                foreach (KeyValuePair<Offer, Price> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(Offer offer)
        {
            string sqlCommand = $"SELECT * FROM public.\"Price\" WHERE \"OFFER_ID\" = {offer.OfferID};";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<Offer, Price>> GetEnumerator()
        {
            ICollection<KeyValuePair<Offer, Price>> pairs = new HashSet<KeyValuePair<Offer, Price>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                Price price = new Price(row);
                pairs.Add(price.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(Offer offer, [MaybeNullWhen(false)] out Price value)
        {
            value = null;
            return ContainsKey(offer);
        }

        private ICollection<ICollection<string>> CsvContents
        {
            get
            {
                ICollection<ICollection<string>> contents = new List<ICollection<string>>();
                contents.Add(Service.ToCollection("A22_PRICE"));
                contents.Add(Service.ToCollection("A23_Price"));
                return contents;
            }
        }
        private DataTable Table
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"Price\";";
                return Service.GetDataTable(sqlCommand);
            }
        }

        private void AddEntries()
        {
            foreach (ICollection<string> contents in CsvContents)
            {
                IEnumerator<string> lines = contents.GetEnumerator();
                if (lines.MoveNext())
                {
                    string headerLine = lines.Current;
                    string[] headers = headerLine.Split(',');
                    while (lines.MoveNext())
                    {
                        string line = lines.Current;
                        string[] values = line.Split(",");
                        int offerID = (int)Service.ExpressValue(values[0]);
                        double expectedIndexValue = (double)Service.ExpressValue(values[1]);
                        IReadOnlyDictionary<int,Offer> offerEntries = new OfferTable();
                        if(!ContainsKey(offerEntries[offerID]))
                        {
                            string sqlCommand = $"INSERT INTO public.\"Price\" ({headers[0]},{headers[1]}) VALUES " +
                                $"({offerID},{expectedIndexValue});";
                            Service.GetDataTable(sqlCommand);

                        }
                    }
                }
            }

        }

        private void ConstructTable()
        {
            string sqlCommand = Service.GetCreateTableSQLCommand("price");
            NpgsqlCommand cmd = new NpgsqlCommand(sqlCommand, Service.User.Connection);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        private void TrimEntries()
        {
            ICollection<string> contents = new HashSet<string>();
            foreach (ICollection<string> contents1 in CsvContents)
            {
                foreach (string line in contents1)
                {
                    string[] values = line.Split(',');
                    contents.Add($"'{values[0]}','{values[1]}'");
                }
            }
            int position = 0;
            while(position < Count)
            {
                Price price = new Price(Table.Rows[position]);
                if(!contents.Contains(price.ToString()))
                {
                    string sqlCommand = "DELETE FROM public.\"Price\" WHERE \"ADM_INSURACE_OFFER_ID\" = '" +
                        price.Offer.OfferID + "\"";
                    Service.GetDataTable(sqlCommand);
                }
                else
                {
                    position++;
                }
            }
        }
    }
}