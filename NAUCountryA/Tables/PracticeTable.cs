using NAUCountryA.Models;
using Npgsql;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace NAUCountryA.Tables
{
    public class PracticeTable : IReadOnlyDictionary<int, Practice>
    {
        public PracticeTable()
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

        public Practice this[int practiceCode]
        {
            get
            {
                string sqlCommand = $"SELECT * FROM public.\"Practice\" WHERE \"PRACTICE_CODE\" = {practiceCode};";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The PRACTICE_CODE: {practiceCode} doesn't exist.");
                }
                return new Practice(table.Rows[0]);
            }
        }

        public IEnumerable<int> Keys
        {
            get
            {
                ICollection<int> keys = new HashSet<int>();
                foreach (KeyValuePair<int,Practice> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<Practice> Values
        {
            get
            {
                ICollection<Practice> values = new List<Practice>();
                foreach (KeyValuePair<int,Practice> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(int practiceCode)
        {
            string sqlCommand = $"SELECT * FROM public.\"Practice\" WHERE \"PRACTICE_CODE\" = {practiceCode};";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<int,Practice>> GetEnumerator()
        {
            ICollection<KeyValuePair<int,Practice>> pairs = new HashSet<KeyValuePair<int,Practice>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                Practice practice = new Practice(row);
                pairs.Add(practice.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(int practiceCode, [MaybeNullWhen(false)] out Practice value)
        {
            value = null;
            return ContainsKey(practiceCode);
        }
        private IEnumerable<string> CsvContents
        {
            get
            {
                return Service.ToCollection("A23_Practice");
            }
        }

        private DataTable Table
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"Practice\";";
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
                    int practiceCode = (int)Service.ExpressValue(values[4]);
                    string practiceName = (string)Service.ExpressValue(values[5]);
                    string practiceAbbreviation = (string)Service.ExpressValue(values[6]);
                    int commodityCode = (int)Service.ExpressValue(values[3]);
                    DateTime releasedDate = (DateTime)Service.ExpressValue(values[8]);
                    string recordTypeCode = (string)Service.ExpressValue(values[0]);
                    if (!ContainsKey(practiceCode))
                    {
                        string sqlCommand = $"INSERT INTO public.\"Practice\" ({headers[4]},{headers[5]}," +
                        $"{headers[6]},{headers[3]},{headers[8]},{headers[0]}) VALUES ({practiceCode}," + 
                        $"'{practiceName}','{practiceAbbreviation}',{commodityCode},'{Service.ToString(releasedDate)}'," +
                        $"'{recordTypeCode}');";
                        Service.GetDataTable(sqlCommand);
                    }
                }
            }
        }

        private void ConstructTable()
        {
            string sqlCommand = Service.GetCreateTableSQLCommand("practice");
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
                contents.Add($"{values[4]},{values[5]},{values[6]},{values[3]},{values[8]},{values[0]}");
            }
            int position = 0;
            while (position < Count)
            {
                Practice practice = new Practice(Table.Rows[position]);
                if (!contents.Contains(practice.ToString()))
                {
                    string sqlCommand = "DELETE FROM public.\"Practice\" WHERE \"PRACTICE_CODE\" = " + 
                        practice.PracticeCode + ";";
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