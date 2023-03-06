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
    public class StateTable : IReadOnlyDictionary<int, State>
    {
        public StateTable()
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

        public State this[int stateCode]
        {
            get
            {
                string sqlCommand = $"SELECT * FROM public.\"State\" WHERE \"STATE_CODE\" = {stateCode};";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The STATE_CODE: {stateCode} doesn't exist.");
                }
                return new State(table.Rows[0]);
            }
        }

        public IEnumerable<int> Keys
        {
            get
            {
                ICollection<int> keys = new HashSet<int>();
                foreach (KeyValuePair<int, State> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<State> Values
        {
            get
            {
                ICollection<State> values = new List<State>();
                foreach (KeyValuePair<int, State> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(int stateCode)
        {
            string sqlCommand = $"SELECT * FROM public.\"State\" WHERE \"STATE_CODE\" = {stateCode};";
            DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<int, State>> GetEnumerator()
        {
            ICollection<KeyValuePair<int, State>> pairs = new HashSet<KeyValuePair<int, State>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                State state = new State(row);
                pairs.Add(state.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(int stateCode, [MaybeNullWhen(false)] out State value)
        {
            value = null;
            return ContainsKey(stateCode);
        }

        private IEnumerable<string> CsvContents
        {
            get
            {
                return Service.ToCollection("A23_STATE");
            }
        }

        private DataTable Table
        {
            get
            {
                string sqlCommand = "SELECT * FROM public.\"State\";";
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
                    int stateCode = (int)Service.ExpressValue(values[3]);
                    string stateName = (string)Service.ExpressValue(values[4]);
                    string stateAbbreviation = (string)Service.ExpressValue(values[5]);
                    string recordTypeCode = (string)Service.ExpressValue(values[0]);
                    if (!ContainsKey(stateCode))
                    {
                        string sqlCommand = $"INSERT INTO public.\"State\" ({headers[3]},{headers[4]}," + 
                            $"{headers[5]},{headers[0]}) VALUES ({stateCode},'{stateName}'," +
                            $"'{stateAbbreviation}','{recordTypeCode}');";
                        Service.GetDataTable(sqlCommand);
                    }
                }
            }
        }

        private void ConstructTable()
        {
            string sqlCommand = Service.GetCreateTableSQLCommand("state");
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
                contents.Add($"{values[3]},{values[4]},{values[5]},{values[0]}");
            }
            int position = 0;
            while (position < Count)
            {
                State state = new State(Table.Rows[position]);
                if (!contents.Contains(state.ToString()))
                {
                    string sqlCommand = $"DELETE FROM public.\"State\" WHERE \"STATE_CODE\" = {state.StateCode};";
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
