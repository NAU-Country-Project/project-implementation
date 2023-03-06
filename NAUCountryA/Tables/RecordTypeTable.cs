using NAUCountryA.Models;
using Npgsql;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace NAUCountryA.Tables
{
    public class RecordTypeTable : IReadOnlyDictionary<string, RecordType>
    {
        public RecordTypeTable()
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

        public RecordType this[string recordTypeCode]
        {
            get
            {
                string sqlCommand = $"SELECT * FROM public.\"RecordType\" WHERE \"RECORD_TYPE_CODE\" = '{recordTypeCode}';";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The RECORD_TYPE_CODE: {recordTypeCode} doesn't exist.");
                }
                return new RecordType(table.Rows[0]);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                ICollection<string> keys = new HashSet<string>();
                foreach (KeyValuePair<string,RecordType> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<RecordType> Values
        {
            get
            {
                ICollection<RecordType> values = new List<RecordType>();
                foreach (KeyValuePair<string,RecordType> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(string recordTypeCode)
        {
            string sqlCommand = $"SELECT * FROM public.\"RecordType\" WHERE \"RECORD_TYPE_CODE\" = '{recordTypeCode}';";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<string,RecordType>> GetEnumerator()
        {
            ICollection<KeyValuePair<string,RecordType>> pairs = new HashSet<KeyValuePair<string,RecordType>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                RecordType recordType = new RecordType(row);
                pairs.Add(recordType.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(string recordTypeCode, [MaybeNullWhen(false)] out RecordType value)
        {
            value = null;
            return ContainsKey(recordTypeCode);
        }

        private ICollection<ICollection<string>> CsvContents
        {
            get
            {
                ICollection<ICollection<string>> contents = new List<ICollection<string>>();
                contents.Add(Service.ToCollection("A23_Commodity"));
                contents.Add(Service.ToCollection("A23_County"));
                contents.Add(Service.ToCollection("A23_Practice"));
                contents.Add(Service.ToCollection("A23_STATE"));
                contents.Add(Service.ToCollection("A23_Type"));
                return contents;
            }
        }

        private DataTable Table
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"RecordType\";";
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
                        string[] values = line.Split(',');
                        string recordTypeCode = (string)Service.ExpressValue(values[0]);
                        int recordCategoryCode = (int)Service.ExpressValue(values[1]);
                        int reinsuranceYear = (int)Service.ExpressValue(values[2]);
                        if (!ContainsKey(recordTypeCode))
                        {
                            string sqlCommand = $"INSERT INTO public.\"RecordType\" ({headers[0]},{headers[1]}," + 
                                $"{headers[2]}) VALUES ('{recordTypeCode}',{recordCategoryCode}," +
                                reinsuranceYear + ");";
                            Service.GetDataTable(sqlCommand);
                        }
                    }
                }
            }
        }

        private void ConstructTable()
        {
            string sqlCommand = Service.GetCreateTableSQLCommand("record_type");
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
                foreach(string line in contents1)
                {
                    string[] values = line.Split(',');
                    contents.Add($"{values[0]},{values[1]},{values[2]}");
                }
            }
            int position = 0;
            while (position < Count)
            {
                RecordType recordType = new RecordType(Table.Rows[position]);
                if (!contents.Contains(recordType.ToString()))
                {
                    string sqlCommand = $"DELETE FROM public.\"RecordType\" WHERE \"RECORD_CATEGORY_CODE\" = '{recordType.RecordTypeCode}';";
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