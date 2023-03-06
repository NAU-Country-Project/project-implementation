using Microsoft.VisualBasic.FileIO;
using NAUCountryA.Exceptions;
using NAUCountryA.Models;
using NAUCountryA.Tables;
using Npgsql;
using System.Data;
using System.Data.Common;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using System.Text.RegularExpressions;

namespace NAUCountryA
{
    public class Service
    {
        public static IReadOnlyDictionary<int, Commodity> CommodityEntries
        {
            get;
            private set;
        }

        public static IReadOnlyDictionary<int, County> CountyEntries
        {
            get;
            private set;
        }
        public static string CreateDatabaseSQLCommand
        {
            get
            {
                string sqlCommand = "";
                string filePath = InitialPathLocation + "\\NAUCountryA\\Resources\\database_construction.sql";
                TextFieldParser sqlParcer = new TextFieldParser(filePath);
                while (!sqlParcer.EndOfData)
                {
                    sqlCommand += sqlParcer.ReadLine();
                }
                return sqlCommand;
            }
        }
        public static string InitialPathLocation
        {
            get
            {
                return GetInitialPathLocation(System.IO.Path.GetFullPath("."));
            }
        }

        public static IReadOnlyDictionary<int, Offer> OfferEntries
        {
            get;
            private set;
        }

        public static IReadOnlyDictionary<int, Practice> PracticeEntries
        {
            get;
            private set;
        }

        public static IReadOnlyDictionary<Offer,Price> PriceEntries
        {
            get;
            private set;
        }

        public static IReadOnlyDictionary<string,RecordType> RecordTypeEntries
        {
            get;
            private set;
        }

        public static IReadOnlyDictionary<int,State> StateEntries
        {
            get;
            private set;
        }

        public static IReadOnlyDictionary<int, NAUType> TypeEntries
        {
            get;
            private set;
        }

        public static NAUUser User
        {
            get;
            private set;
        }

        public static void ConstructUser()
        {
            Console.WriteLine("Enter your server name: ");
            string serverName = Console.ReadLine();
            int portNumber;
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter your port number");
                    portNumber = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("The port number must be an integer. Try Again.");
                }
            }
            Console.WriteLine("Enter your User ID: ");
            string userID = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();
            User = new NAUUser(serverName, portNumber, userID, password);
            LoadTables();
        }

        public static bool DateTimeEquals(DateTime a, DateTime b)
        {
            return a.Month == b.Month && a.Day == b.Day && a.Year == b.Year;
        }

        public static object ExpressValue(string value)
        {
            if ("\"\"".Equals(value))
            {
                return 0.0;
            }
            string temp = value.Substring(1, value.Length - 2);
            if (temp.Length == 1)
            {
                return temp[0];
            }
            else if (IsInt(temp))
            {
                return Convert.ToInt32(temp);
            }
            else if (IsDouble(temp))
            {
                return Convert.ToDouble(temp);
            }
            else if (IsDate(temp))
            {
                return ToDateTime(temp);
            }
            return temp;
        }

        public static string GetCreateTableSQLCommand(string tableName)
        {
            string sqlCommand = "";
            string filePath = InitialPathLocation + "\\NAUCountryA\\Resources\\" + tableName.ToLower() + "_construction.sql";
            TextFieldParser sqlParcer = new TextFieldParser(filePath);
            while (!sqlParcer.EndOfData)
            {
                sqlCommand += sqlParcer.ReadLine();
            }
            return sqlCommand;
        }
        public static DataTable GetDataTable(string sqlCommand)
        {
            DataTable table = new DataTable();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlCommand, User.Connection);
            DbDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            string command = sqlCommand.Substring(0, sqlCommand.IndexOf(' ')).ToUpper();
            switch (command)
            {
                case "DELETE":
                    adapter.DeleteCommand = cmd;
                    adapter.DeleteCommand.Connection.Open();
                    adapter.DeleteCommand.ExecuteNonQuery();
                    adapter.Update(table);
                    adapter.DeleteCommand.Connection.Close();
                    break;
                case "INSERT":
                    adapter.InsertCommand = cmd;
                    adapter.InsertCommand.Connection.Open();
                    adapter.InsertCommand.ExecuteNonQuery();
                    adapter.Update(table);
                    adapter.InsertCommand.Connection.Close();
                    break;
                case "SELECT":
                    adapter.SelectCommand = cmd;
                    adapter.SelectCommand.Connection.Open();
                    adapter.SelectCommand.ExecuteNonQuery();
                    adapter.Fill(table);
                    adapter.SelectCommand.Connection.Close();
                    break;
                case "UPDATE":
                    adapter.UpdateCommand = cmd;
                    adapter.UpdateCommand.Connection.Open();
                    adapter.UpdateCommand.ExecuteNonQuery();
                    adapter.Update(table);
                    adapter.UpdateCommand.Connection.Close();
                    break;
                default: throw new UnrecognizedSQLCommandException("The command isn't recognized in PostgreSQL.");
            }
            return table;
        }

        public IEnumerable<County> GetStateCounties(State state)
        {
            ICollection<County> counties = new List<County>();
            foreach (County county in CountyEntries.Values)
            {
                if (county.State == state)
                {
                    counties.Add(county);
                }
            }
            return counties;
        }

        public static void InitializeUserTo(NAUUser user)
        {
            User = user;
            LoadTables();
        }

        public static ICollection<string> ToCollection(string csvFileName)
        {
            ICollection<string> lines = new List<string>();
            string filePath = InitialPathLocation + "\\NAUCountryA\\Resources\\" + csvFileName + ".csv";
            TextFieldParser csvParcer = new TextFieldParser(filePath);
            csvParcer.TextFieldType = FieldType.Delimited;
            while (!csvParcer.EndOfData)
            {
                lines.Add(csvParcer.ReadLine());
            }
            csvParcer.Close();
            return lines;
        }

        public static DateTime ToDateTime(string value)
        {
            string[] parts = value.Split("/");
            if (parts.Length != 3)
            {
                throw new FormatException("This is not a date.");
            }
            return new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
        }

        public static string ToString(DateTime date)
        {
            return date.Month + "/" + date.Day + "/" + date.Year;
        }

        public static string ToString(double number)
        {
            if (number == 0.0)
            {
                return "";
            }
            try
            {
                string temp = number.ToString().Split('.')[1];
                switch (temp.Length)
                {
                    case 1: return $"{number.ToString()}000";
                    case 2: return $"{number.ToString()}00";
                    case 3: return $"{number.ToString()}0";
                }
                return $"'{number.ToString()}'";
            }
            catch (IndexOutOfRangeException)
            {
                return $"{number.ToString()}.0000";
            }
        }

        private static string GetInitialPathLocation(string currentLocation)
        {
            DirectoryInfo temp = Directory.GetParent(currentLocation);
            if (temp.Name.Equals("project-implementation"))
            {
                return temp.FullName;
            }
            return GetInitialPathLocation(temp.FullName);
        }

        public static string GetPath(string filePath)
        {
            var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return System.IO.Path.Combine(appRoot, filePath);
        }

        private static bool IsDate(string value)
        {
            try
            {
                ToDateTime(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool IsDouble(string value)
        {
            try
            {
                Convert.ToDouble(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool IsInt(string value)
        {
            try
            {
                Convert.ToInt32(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static void LoadTables()
        {
            RecordTypeEntries = new RecordTypeTable();
            CommodityEntries = new CommodityTable();
            StateEntries = new StateTable();
            CountyEntries = new CountyTable();
            TypeEntries = new NauTypeTable();
            PracticeEntries = new PracticeTable();
            OfferEntries = new OfferTable();
            PriceEntries = new PriceTable();
        }

        public static void GeneratePDF(State state, Practice practice, NAUType type)
        {
            Document doc = new Document();
            Page page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
            doc.Pages.Add(page);
            ICollection<Price> prices = new List<Price>();
            foreach (Price price in PriceEntries.Values)
            {
                if (price.Offer.State == state && price.Offer.Practice == practice && price.Offer.Type == type && price.Offer.Year == 2023)
                {
                    prices.Add(price);
                }
            }
            string labelText = $"{state.StateName} {practice.Commodity.CommodityName} {practice.PracticeName} {type.TypeName} {2023}";
            Label label = new Label(labelText, 0, 0, 504, 100, Font.Helvetica, 18, TextAlign.Center);
            page.Elements.Add(label);
            TextArea textArea = new TextArea("", 100, 100, 400, 30, ceTe.DynamicPDF.Font.HelveticaBoldOblique, 18);
            foreach (Price price in prices)
            {
                textArea.Text = textArea.Text + price.Offer.County + ": " + price.ExpectedIndexValue + "\n";
            }
            page.Elements.Add(textArea);
            doc.Draw(GetPath("PDFOutput/CreatePDF.pdf")); ;
        }
    }
}
