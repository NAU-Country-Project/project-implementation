using NAUCountryA.Models;
using Npgsql;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace NAUCountryA.Tables
{
    public class CommodityTable : IReadOnlyDictionary<int, Commodity>
    {
        public CommodityTable()
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

        public Commodity this[int commodityCode]
        {
            get
            {
                string sqlCommand = $"SELECT * FROM public.\"Commodity\" WHERE \"COMMODITY_CODE\" = {commodityCode};";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The COMMODITY_CODE: {commodityCode} doesn't exist.");
                }
                return new Commodity(table.Rows[0]);
            }
        }

        public IEnumerable<int> Keys
        {
            get
            {
                ICollection<int> keys = new HashSet<int>();
                foreach (KeyValuePair<int,Commodity> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<Commodity> Values
        {
            get
            {
                ICollection<Commodity> values = new List<Commodity>();
                foreach (KeyValuePair<int,Commodity> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(int commodityCode)
        {
            string sqlCommand = $"SELECT * FROM public.\"Commodity\" WHERE \"COMMODITY_CODE\" = {commodityCode};";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<int,Commodity>> GetEnumerator()
        {
            ICollection<KeyValuePair<int,Commodity>> pairs = new HashSet<KeyValuePair<int,Commodity>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                Commodity commodity = new Commodity(row);
                pairs.Add(commodity.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(int commodityCode, [MaybeNullWhen(false)] out Commodity value)
        {
            value = null;
            return ContainsKey(commodityCode);
        }
        private IEnumerable<string> CsvContents
        {
            get
            {
                return Service.ToCollection("A23_Commodity");
            }
        }

        private DataTable Table
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"Commodity\";";
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
                    int commodityCode = (int)Service.ExpressValue(values[4]);
                    string commodityName = (string)Service.ExpressValue(values[5]);
                    string commodityAbbreviation = (string)Service.ExpressValue(values[6]);
                    char annualPlantingCode = (char)Service.ExpressValue(values[7]);
                    int commodityYear = (int)Service.ExpressValue(values[3]);
                    DateTime releasedDate = (DateTime)Service.ExpressValue(values[9]);
                    string recordTypeCode = (string)Service.ExpressValue(values[0]);
                    if (!ContainsKey(commodityCode))
                    {
                        string sqlCommand = $"INSERT INTO public.\"Commodity\" ({headers[4]},{headers[5]},{headers[6]},{headers[7]}," +
                            $"{headers[3]},{headers[9]},{headers[0]}) VALUES (" + 
                            $"{commodityCode},'{commodityName}','{commodityAbbreviation}','{annualPlantingCode}',{commodityYear}," +
                            $"'{Service.ToString(releasedDate)}','{recordTypeCode}');";
                        Service.GetDataTable(sqlCommand);
                    }
                }
            }
        }

        private void ConstructTable()
        {
            string sqlCommand = Service.GetCreateTableSQLCommand("commodity");
            NpgsqlCommand cmd = new NpgsqlCommand(sqlCommand, Service.User.Connection);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        private void TrimEntries()
        {
            ICollection<string> contents = new HashSet<string>();
            foreach(string line in CsvContents)
            {
                string[] values = line.Split(',');
                contents.Add($"{values[4]},{values[5]},{values[6]},{values[7]}," +
                    $"{values[3]},{values[9]},{values[0]}");
            }
            int position = 0;
            while (position < Count)
            {
                Commodity commodity = new Commodity(Table.Rows[position]);
                if (!contents.Contains(commodity.ToString()))
                {
                    string sqlCommand = "DELETE FROM public.\"Commodity\" WHERE \"COMMODITY_CODE\" =" + 
                        $"{commodity.CommodityCode};";
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