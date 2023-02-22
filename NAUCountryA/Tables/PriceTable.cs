using NAUCountryA.Models;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace NAUCountryA.Tables
{
    public class PriceTable : IReadOnlyDictionary<string, Price>
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

        public Price this[string offerID]
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"Price\" WHERE \"OFFER_ID\" = '"
                + offerID + "';";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException("The OFFER_ID: " + offerID + " doesn't exist.");
                }
                return new Price(table.Rows[0]);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                ICollection<string> keys = new HashSet<string>();
                foreach (KeyValuePair<string, State> pair in this)
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
                foreach (KeyValuePair<string, Price> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(int offerID)
        {
            string sqlCommand = "SELECT * FROM public.\"Price\" WHERE \"OFFER_ID\" = '"
                + offerID + "';";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<string, Price>> GetEnumerator()
        {
            ICollection<KeyValuePair<string, Price>> pairs = new HashSet<KeyValuePair<string, Price>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                Price price = new Price(row);
                pairs.Add(Price.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(string offerID, [MaybeNullWhen(false)] out Price value)
        {
            value = null;
            return ContainsKey(offerID);
        }

        private ICollection<ICollection<string>> CsvContents
        {
            get
            {
                ICollection<ICollection<string>> contents = new List<ICollection<string>>();
                contents.Add(Service.ToCollection("A23_INSURANCE_OFFER"));
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
            ICollection<ICollection<string>> csvContents = CsvContents;
            foreach (ICollection<string> contents in csvContents)
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
                        if (!ContainsKey(offerID))
                        {
                            string sqlCommand = "INSERT INTO public.\"Price\" (" +
                                headers[0] + "," + headers[1] ") VALUES" +
                                "('" + offerID + "," + expectedIndexValue + ");";
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
                    contents.Add(values[0] + "," + values[1] + "," + values[2] + "," + values[3]);
                }
            }
            int position = 0;
            while (position < Count)
            {
                Price price = new Price(Table.Rows[position]);
                string lineFromTable = "\"" + price.OfferID + "\"";
                if (price.OfferID < 10)
                {
                    lineFromTable += "0";
                }
                lineFromTable += price.ExpectedIndexValue "\"";
                if (!contents.Contains(lineFromTable))
                {
                    string sqlCommand = "DELETE FROM public.\"Price\" WHERE \"EXPECTED_INDEX_VALUE\" = '" +
                        price.ExpectedIndexValue + "\"";
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