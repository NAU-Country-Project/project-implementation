using NAUCountryA;
using NAUCountryA.Models;

namespace NAUCountryATest.Tables
{
    public class RecordTypeTableTest
    {
        [SetUp]
        public void Setup()
        {
            Service.InitializeUserTo(ServiceTest.TEST_USER);
        }

        [Test]
        public void TestContainsKey1()
        {
            bool actual = Service.RecordTypeEntries.ContainsKey("A00420");
            Assert.That(actual, Is.True);
        }

        [Test]
        public void TestContainsKey2()
        {
            bool actual = Service.RecordTypeEntries.ContainsKey("A0042");
            Assert.That(actual, Is.False);
        }

        [Test]
        public void TestCount()
        {
            Assert.That(Service.RecordTypeEntries.Count, Is.EqualTo(5));
        }

        [Test]
        public void TestGetRecordTypeByKeyValid()
        {
            RecordType expected = new RecordType("A00440", 1, 2023);
            RecordType actual = Service.RecordTypeEntries["A00440"];
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetRecordTypeByKeyInvalid()
        {
            Assert.Throws<KeyNotFoundException>(() => ToDelegate("A00140"));
        }

        private void ToDelegate(string key)
        {
            RecordType type = Service.RecordTypeEntries[key];
        }

        [Test]
        public void TestGetKeys()
        {
            IEnumerable<string> expected = new string[]{"A00420", "A00440", "A00510", "A00520", "A00540"}.AsEnumerable();
            IEnumerable<string> actual = Service.RecordTypeEntries.Keys;
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void TestGetValues()
        {
            IEnumerable<RecordType> expected = new RecordType[]
            {new RecordType("A00420", 1, 2023),
            new RecordType("A00440", 1, 2023),
            new RecordType("A00510", 1, 2023),
            new RecordType("A00520", 1, 2023),
            new RecordType("A00540", 1, 2023)};
            IEnumerable<RecordType> actual = Service.RecordTypeEntries.Values;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetEnumerator()
        {
            ICollection<KeyValuePair<string,RecordType>> expected = new HashSet<KeyValuePair<string,RecordType>>();
            expected.Add(new KeyValuePair<string, RecordType>("A00420", new RecordType("A00420", 1, 2023)));
            expected.Add(new KeyValuePair<string, RecordType>("A00440", new RecordType("A00440", 1, 2023)));
            expected.Add(new KeyValuePair<string, RecordType>("A00510", new RecordType("A00510", 1, 2023)));
            expected.Add(new KeyValuePair<string, RecordType>("A00520", new RecordType("A00520", 1, 2023)));
            expected.Add(new KeyValuePair<string, RecordType>("A00540", new RecordType("A00540", 1, 2023)));
            ICollection<KeyValuePair<string,RecordType>> actual = new HashSet<KeyValuePair<string,RecordType>>();
            foreach (KeyValuePair<string,RecordType> pair in Service.RecordTypeEntries)
            {
                actual.Add(pair);
            }
            Assert.That(actual, Is.EqualTo(actual));
        }

        [Test]
        public void TestTryGet1()
        {
            RecordType test = null;
            Assert.That(Service.RecordTypeEntries.TryGetValue("A00420", out test), Is.True);
        }

        [Test]
        public void TestTryGet2()
        {
            RecordType test = null;
            Assert.That(Service.RecordTypeEntries.TryGetValue("A0042", out test), Is.False);
        }
    }
}