using NAUCountryA.Models;
using Npgsql;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace NAUCountryA.Tables
{
    public class NauTypeTable : IReadOnlyDictionary<int, NAUType>
    {
        public NauTypeTable()
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

        public NAUType this[int typeCode]
        {
            get
            {
                string sqlCommand = $"SELECT * FROM public.\"Type\" WHERE \"TYPE_CODE\" = {typeCode};";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The TYPE_CODE: {typeCode} doesn't exist.");
                }
                return new NAUType(table.Rows[0]);
            }
        }

        public IEnumerable<int> Keys
        {
            get
            {
                ICollection<int> keys = new HashSet<int>();
                foreach (KeyValuePair<int, NAUType> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<NAUType> Values
        {
            get
            {
                ICollection<NAUType> values = new List<NAUType>();
                foreach (KeyValuePair<int, NAUType> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(int typeCode)
        {
            string sqlCommand = $"SELECT * FROM public.\"Type\" WHERE \"TYPE_CODE\" = {typeCode};";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<int, NAUType>> GetEnumerator()
        {
            ICollection<KeyValuePair<int, NAUType>> pairs = new HashSet<KeyValuePair<int, NAUType>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                NAUType type = new NAUType(row);
                pairs.Add(type.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(int typeCode, [MaybeNullWhen(false)] out NAUType value)
        {
            value = null;
            return ContainsKey(typeCode);
        }

        private IEnumerable<string> CsvContents
        {
            get
            {
                return Service.ToCollection("A23_TYPE");
            }
        }

        private DataTable Table
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"Type\";";
                return Service.GetDataTable(sqlCommand);
            }
        }

        private void AddEntries()
        {
            IEnumerator<string> lines = CsvContents.GetEnumerator();
            if(lines.MoveNext())
            {
                string headerLine = lines.Current;
                string[] headers = headerLine.Split(',');
                while (lines.MoveNext())
                {
                    string line = lines.Current;
                    string[] values = line.Split(",");
                    int typeCode = (int)Service.ExpressValue(values[4]);
                    string typeName = (string)Service.ExpressValue(values[5]);
                    string typeAbbreviation = (string)Service.ExpressValue(values[6]);
                    int commodityCode = (int)Service.ExpressValue(values[3]); 
                    DateTime releasedDate = (DateTime)Service.ExpressValue(values[8]);
                    string recordTypeCode = (string)Service.ExpressValue(values[0]);
                    if(!ContainsKey(typeCode))
                    {
                        string sqlCommand = $"INSERT INTO public.\"State\" ({headers[4]},{headers[5]}," + 
                            $"{headers[6]},{headers[3]},{headers[8]},{headers[0]}) VALUES ({typeCode}," +
                            $"'{typeName}','{typeAbbreviation}',{commodityCode},'{Service.ToString(releasedDate)}'" +
                            $",'{recordTypeCode}');";
                        Service.GetDataTable(sqlCommand);
                    }
                }
            }
        }

        private void ConstructTable()
        {
            string sqlCommand = Service.GetCreateTableSQLCommand("type");
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
                contents.Add($"{values[4]},{values[5]},{values[6]},{values[3]},{values[8]},{values[0]}");
            }
            int position = 0;
            while (position < Count)
            {
                NAUType type = new NAUType(Table.Rows[position]);
                if(!contents.Contains(type.ToString()))
                {
                    string sqlCommand = "DELETE FROM public.\"Type\" WHERE \"TYPE_CODE\" = " +
                        type.TypeCode + ";";
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
