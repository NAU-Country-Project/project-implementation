using NAUCountryA;
using NAUCountryA.Models;
using NAUCountryA.Tables;

namespace NAUCountryATest
{
    public class ServiceTest
    {
        internal readonly static NAUUser TEST_USER = new NAUUser("localhost", 2023, "postgres", "naucountrydev");
        [Test]
        public void TestCreateDatabaseSQLCommand()
        {
            string expected = "CREATE DATABASE \"NAUCountryData\"    WITH    OWNER = postgres    TEMPLATE = template0    ENCODING = 'UTF8'    LC_COLLATE = 'English_United States.1252'    LC_CTYPE = 'English_United States.1252'    TABLESPACE = pg_default    CONNECTION LIMIT = -1    IS_TEMPLATE = False;";
            Assert.That(Service.CreateDatabaseSQLCommand, Is.EqualTo(expected));
        }

        [Test]
        public void TestInitialPathLocation()
        {
            string expectedPath = "c:\\Users\\zakme\\OneDrive - North Dakota University System\\Documents\\project-implementation";
            Assert.That(Service.InitialPathLocation, Is.EqualTo(expectedPath));
        }

        [Test]
        public void TestToCollectionOfA22_INSURANCE_OFFERCount()
        {
            int expected = 2393564;
            Assert.That(Service.ToCollection("A22_INSURANCE_OFFER"), Has.Count.EqualTo(expected));
        }

        [Test]
        public void TestToCollectionOfA22_INSURANCE_OFFERHeaders()
        {
            string expectedHeaders = "\"ADM_INSURANCE_OFFER_ID\",\"STATE_CODE\",\"COUNTY_CODE\",\"TYPE_CODE\",\"PRACTICE_CODE\",\"IRRIGATION_PRACTICE_CODE\"";
            Assert.That(Service.ToCollection("A22_INSURANCE_OFFER").First(), Is.EqualTo(expectedHeaders));
        }

        [Test]
        public void TestToCollectionOfA22_INSURANCE_OFFERFirstEntry()
        {
            string expectedFirstEntry = "\"25360764\",\"06\",\"019\",\"997\",\"092\",\"997\"";
            IEnumerable<string> lines = Service.ToCollection("A22_INSURANCE_OFFER");
            IEnumerator<string> lineEnumerator = lines.GetEnumerator();
            string actualFirstEntry = "";
            if (lineEnumerator.MoveNext() && lineEnumerator.MoveNext())
            {
                actualFirstEntry = lineEnumerator.Current;
            }
            Assert.That(actualFirstEntry, Is.EqualTo(expectedFirstEntry));
        }

        [Test]
        public void TestToCollectionOfA23_CountyCount()
        {
            int expected = 3192;
            Assert.That(Service.ToCollection("A23_County"), Has.Count.EqualTo(expected));
        }

        [Test]
        public void TestToCollectionOfA23_CountyHeaders()
        {
            string expectedHeaders = "\"RECORD_TYPE_CODE\",\"RECORD_CATEGORY_CODE\",\"REINSURANCE_YEAR\",\"STATE_CODE\",\"COUNTY_CODE\",\"COUNTY_NAME\",\"LAST_RELEASED_DATE\",\"RELEASED_DATE\",\"DELETED_DATE\"";
            Assert.That(Service.ToCollection("A23_County").First(), Is.EqualTo(expectedHeaders));
        }

        [Test]
        public void TestToCollectionOfA23_CountyFirstEntry()
        {
            string expectedFirstEntry = "\"A00440\",\"01\",\"2023\",\"01\",\"001\",\"Autauga\",\"\",\"4/28/2022\",\"\"";
            IEnumerable<string> lines = Service.ToCollection("A23_County");
            IEnumerator<string> lineEnumerator = lines.GetEnumerator();
            string actualFirstEntry = "";
            if (lineEnumerator.MoveNext() && lineEnumerator.MoveNext())
            {
                actualFirstEntry = lineEnumerator.Current;
            }
            Assert.That(actualFirstEntry, Is.EqualTo(expectedFirstEntry));
        }

        [Test]
        public void TestToCollectionOfA23_CommodityCount()
        {
            int expected = 141;
            Assert.That(Service.ToCollection("A23_Commodity"), Has.Count.EqualTo(expected));
        }

        [Test]
        public void TestToCollectionOfA23_CommodityHeaders()
        {
            string expectedHeaders = "\"RECORD_TYPE_CODE\",\"RECORD_CATEGORY_CODE\",\"REINSURANCE_YEAR\",\"COMMODITY_YEAR\",\"COMMODITY_CODE\",\"COMMODITY_NAME\",\"COMMODITY_ABBREVIATION\",\"ANNUAL_PLANTING_CODE\",\"LAST_RELEASED_DATE\",\"RELEASED_DATE\",\"DELETED_DATE\"";
            Assert.That(Service.ToCollection("A23_Commodity").First(), Is.EqualTo(expectedHeaders));
        }

        [Test]
        public void TestToCollectionOfA23_CommodityFirstEntry()
        {
            string expectedFirstEntry = "\"A00420\",\"01\",\"2023\",\"2022\",\"0037\",\"Raisins\",\"RAISINS\",\"P\",\"\",\"4/7/2022\",\"\"";
            IEnumerable<string> lines = Service.ToCollection("A23_Commodity");
            IEnumerator<string> lineEnumerator = lines.GetEnumerator();
            string actualFirstEntry = "";
            if (lineEnumerator.MoveNext() && lineEnumerator.MoveNext())
            {
                actualFirstEntry = lineEnumerator.Current;
            }
            Assert.That(actualFirstEntry, Is.EqualTo(expectedFirstEntry));
        }

        [Test]
        public void TestToCollectionOfA23_STATECount()
        {
            int expected = 51;
            Assert.That(Service.ToCollection("A23_STATE"), Has.Count.EqualTo(expected));
        }

        [Test]
        public void TestToCollectionOfA23_STATEHeaders()
        {
            string expectedHeaders = "\"RECORD_TYPE_CODE\",\"RECORD_CATEGORY_CODE\",\"REINSURANCE_YEAR\",\"STATE_CODE\",\"STATE_NAME\",\"STATE_ABBREVIATION\",\"REGIONAL_OFFICE_CODE\",\"REGIONAL_OFFICE_NAME\",\"LAST_RELEASED_DATE\",\"RELEASED_DATE\",\"DELETED_DATE\"";
            Assert.That(Service.ToCollection("A23_STATE").First(), Is.EqualTo(expectedHeaders));
        }

        [Test]
        public void TestToCollectionOfA23_STATEFirstEntry()
        {
            string expectedFirstEntry = "\"A00520\",\"01\",\"2023\",\"01\",\"Alabama\",\"AL\",\"02\",\"Valdosta\",\"\",\"4/28/2022\",\"\"";
            IEnumerable<string> lines = Service.ToCollection("A23_STATE");
            IEnumerator<string> lineEnumerator = lines.GetEnumerator();
            string actualFirstEntry = "";
            if (lineEnumerator.MoveNext() && lineEnumerator.MoveNext())
            {
                actualFirstEntry = lineEnumerator.Current;
            }
            Assert.That(actualFirstEntry, Is.EqualTo(expectedFirstEntry));
        }

        [Test]
        public void TestExpressValue1()
        {
            string expected = "A00420";
            string actual = (string)Service.ExpressValue("\"A00420\"");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestExpressValue2()
        {
            int expected = 1;
            int actual = (int)Service.ExpressValue("\"01\"");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]

        public void TestExpressValue3()
        {
            double expected = 0.0;
            double actual = (double)Service.ExpressValue("\"\"");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestExpressValue4()
        {
            char expected = 'P';
            char actual = (char)Service.ExpressValue("\"P\"");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestExpressValue5()
        {
            double expected = 42.9;
            double actual = (double)Service.ExpressValue("\"42.9000\"");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestExpressValue6()
        {
            DateTime expected = new DateTime(2023, 2, 25);
            DateTime actual = (DateTime)Service.ExpressValue("\"2/25/2023\"");
            Assert.That(Service.DateTimeEquals(expected, actual), Is.True);
        }

        [Test]
        public void TestGetCreateTableSQLCommand()
        {
            string expected = "CREATE TABLE IF NOT EXISTS public.\"Commodity\"" +
            "(    " +
                "\"COMMODITY_CODE\" integer NOT NULL,    " +
                "\"COMMODITY_NAME\" character varying(30) NOT NULL,    " +
                "\"COMMODITY_ABBREVIATION\" character varying(15) NOT NULL,    " +
                "\"ANNUAL_PLANTING_CODE\" character(1) NOT NULL,    " +
                "\"COMMODITY_YEAR\" integer NOT NULL,    " +
                "\"RELEASED_DATE\" character varying(10) NOT NULL,    " +
                "\"RECORD_TYPE_CODE\" character varying(10) NOT NULL,    " +
                "CONSTRAINT \"Pk_Commodity\" PRIMARY KEY (\"COMMODITY_CODE\"),    " +
                "CONSTRAINT \"Fk_RecordType\" FOREIGN KEY (\"RECORD_TYPE_CODE\")        " +
                    "REFERENCES public.\"RecordType\" (\"RECORD_TYPE_CODE\") MATCH SIMPLE        " +
                    "ON UPDATE NO ACTION        " +
                    "ON DELETE NO ACTION        " +
                    "NOT VALID" +
            ")" +

            "TABLESPACE pg_default;" +

            "ALTER TABLE IF EXISTS public.\"Commodity\"    " +
                "OWNER to postgres;";
            string actual = Service.GetCreateTableSQLCommand("commodity");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestInitializeUserTo()
        {
            NAUUser user = new NAUUser("localhost", 2023, "postgres", "naucountrydev");
            Service.InitializeUserTo(user);
            Assert.That(user.Connection, Is.EqualTo(Service.User.Connection));
        }

        [Test]
        public void TestToString1()
        {
            string expected = "2/25/2023";
            string actual = Service.ToString(new DateTime(2023, 2, 25));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestToString2()
        {
            string expected = "";
            string actual = Service.ToString(0);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestToString3()
        {
            string expected = "42.9000";
            string actual = Service.ToString(42.9);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}