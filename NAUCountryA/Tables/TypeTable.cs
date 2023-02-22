using NAUCountryA.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NauType = NAUCountryA.Models.NauType;

namespace NAUCountryA.Tables
{
    public class TypeTable : IReadOnlyDictionary<int, NauType>
    {
        public TypeTable()
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

        public NauType this[string typeCode]
        {
            get
            {
                // Design Note: Using $ at the start of the string and injecting C# variables with {} can help make string concat/formats more readable.
                string sqlCommand = $"SELECT * FROM public.\"Type\" WHERE \"TYPE_CODE\" = '{typeCode}';";
                DataTable table = Service.GetDataTable(sqlCommand);
                if (table.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"The TYPE_CODE: {typeCode} doesn't exist.");
                }
                return new NauType(table.Rows[0]);

            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                ICollection<string> keys = new HashSet<string>();
                foreach (KeyValuePair<string, NauType> pair in this)
                {
                    keys.Add(pair.Key);
                }
                return keys;
            }
        }

        public IEnumerable<NauType> Values
        {
            get
            {
                ICollection<NauType> values = new List<NauType>();
                foreach (KeyValuePair<string, NauType> pair in this)
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }

        public bool ContainsKey(int typeCode)
        {
			string sqlCommand = $"SELECT * FROM public.\"Type\" WHERE \"TYPE_CODE\" = '{typeCode}';";

			//string sqlCommand = "SELECT * FROM public.\"Type\" WHERE \"TYPE_CODE\" = '"
			//             + typeCode + "';";
			DataTable table = Service.GetDataTable(sqlCommand);
            return table.Rows.Count >= 1;
        }

        public IEnumerator<KeyValuePair<string, NauType>> GetEnumerator()
        {
            ICollection<KeyValuePair<string, NauType>> pairs = new HashSet<KeyValuePair<string, NauType>>();
            DataTable table = Table;
            foreach (DataRow row in table.Rows)
            {
                NauType type = new NauType(row);
                pairs.Add(type.Pair);
            }
            return pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(int typeCode, [MaybeNullWhen(false)] out NauType value)
        {
            value = null;
            return ContainsKey(typeCode);
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
                string sqlCommand = "SELECT * FROM public.\"Type\";";
                return Service.GetDataTable(sqlCommand);
            }
        }

		IEnumerable<int> IReadOnlyDictionary<int, Models.NauType>.Keys => throw new NotImplementedException();

		public Models.NauType this[int key] => throw new NotImplementedException();

		private void AddEntries()
        {
            ICollection<ICollection<string>> csvContents = CsvContents;
            foreach (ICollection<string> contents in csvContents)
            {
                IEnumerator<string> lines = contents.GetEnumerator();
                if(lines.MoveNext())
                {
                    string headerLine = lines.Current;
                    string[] headers = headerLine.Split(',');
                    while (lines.MoveNext())
                    {
                        string line = lines.Current;
                        string[] values = line.Split(",");
                        int typeCode = (int)Service.ExpressValue(values[0]);
                        string typeName = (string)Service.ExpressValue(values[1]);
                        string typeAbbreviation = (string)Service.ExpressValue(values[2]);
                        Commodity commodity = (Commodity)Service.ExpressValue(values[3]); 
                        DateTime releasedDate = (DateTime)Service.ExpressValue(values[4]);
                        string recordType = (string)Service.ExpressValue(values[5]);
                        if(!ContainsKey(typeCode))
                        {
                            string sqlCommand = "INSERT INTO public.\"State\" (" +
                                 headers[0] + "," + headers[1] + "," + headers[2] + "," + headers[3] + "," + headers[4] + "," + headers[5] ") VALUES " +
                                "('" + typeCode + "', " + typeName + "," +
                                typeAbbreviation + "," + commodity + "," + releasedDate +
                                "," + recordType ");";
                            Service.GetDataTable(sqlCommand);
                        }
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
                NauType type = new Models.NauType(Table.Rows[position]);
                string lineFromTable = "\"" + type.TypeCode + "\"";
                if(type.TypeName < 10)
                {
                    lineFromTable += "0";
                }
                lineFromTable += type.TypeName + "\",\"" + type.TypeAbbreviation + 
                    "\",\"" + type.Commodity + "\",\"" + type.ReleasedDate 
                    + "\",\"" + type.RecordType "\"";
                if(!contents.Contains(lineFromTable))
                {
                    string sqlCommand = "DELETE FROM public.\"Type\" WHERE \"TYPE_NAME\" = '" +
                        type.TypeName + "';";
                    Service.GetDataTable(sqlCommand);
                }
                else
                {
                    position++;
                }
            }

        }

		IEnumerator<KeyValuePair<int, Models.NauType>> IEnumerable<KeyValuePair<int, Models.NauType>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
