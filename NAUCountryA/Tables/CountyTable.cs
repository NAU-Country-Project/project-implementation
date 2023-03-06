using NAUCountryA.Models;
using Npgsql;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;


namespace NAUCountryA.Tables
{
    public class CountyTable : IReadOnlyDictionary<int, County>
    {
        public CountyTable()
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

        public County this[int countyCode]
        {
            get
            {
                string sqlCommand = $"SELECT * FROM public.\"County\" WHERE \"COUNTY_CODE\"={countyCode};";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The COUNTY_CODE: {countyCode} doesn't exist.");
                }
                return new County(table.Rows[0]);
            }
        }
        public IEnumerable<int> Keys
        {
            get
            {
                ICollection<int> keys = new HashSet<int>();
                foreach (KeyValuePair<int, County> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<County> Values
        {
            get
            {
                ICollection<County> values = new List<County>();
                foreach (KeyValuePair<int, County> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(int countyCode)
        {
            string sqlCommand = $"SELECT * FROM public.\"County\" WHERE \"COUNTY_CODE\"={countyCode};";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<int, County>> GetEnumerator()
        {
            ICollection<KeyValuePair<int, County>> pairs = new HashSet<KeyValuePair<int, County>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                County county = new County(row);
                pairs.Add(county.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(int countyCode, [MaybeNullWhen(false)] out County value)
        {
            value = null;
            return ContainsKey(countyCode);
        }
        private IEnumerable<string> CsvContents
        {
            get
            {
                return Service.ToCollection("A23_County");
            }
        }

        private DataTable Table
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"County\";";
                return Service.GetDataTable(sqlCommand);
            }
        }

        private void AddEntries()
        {
            IEnumerator<string> lines = CsvContents.GetEnumerator();
            if (lines.MoveNext())
            {
                string headerLine = lines.Current;
                string[] headers = headerLine.Split(',');
                while (lines.MoveNext())
                {
                    string line = lines.Current;
                    string[] values = line.Split(',');
                    int countyCode = (int)Service.ExpressValue(values[4]);
                    int stateCode = (int)Service.ExpressValue(values[3]);
                    string countyName = (string)Service.ExpressValue(values[5]);
                    string recordTypeCode = (string)Service.ExpressValue(values[0]);
                    if (!ContainsKey(countyCode))
                    {
                        string sqlCommand = $"INSERT INTO public.\"County\" ({headers[4]},{headers[3]}" +
                            $",{headers[5]},{headers[0]}) VALUES ({countyCode},{stateCode}," +
                            $"'{countyName}','{recordTypeCode}');";
                        Service.GetDataTable(sqlCommand);
                    }
                }
            }
        }

        private void ConstructTable()
        {
            string sqlCommand = Service.GetCreateTableSQLCommand("county");
            NpgsqlCommand cmd = new NpgsqlCommand(sqlCommand, Service.User.Connection);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        private void TrimEntries()
        {
            ICollection<string> contents = new HashSet<string>();
            foreach (string line in CsvContents)
            {
                string[] values = line.Split(',');
                contents.Add($"{values[4]},{values[3]},{values[5]},{values[0]}");
            }
            int position = 0;
            while (position < Count)
            {
                County county = new County(Table.Rows[position]);
                if (!contents.Contains(county.ToString()))
                {
                    string sqlCommand = "DELETE FROM public.\"County\" WHERE \"COUNTY_CODE\" = " +
                        county.CountyCode + ";";
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