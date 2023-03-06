using NAUCountryA;
using NAUCountryA.Models;

namespace NAUCountryATest.Models
{
    public class RecordTypeTest
    {
        private RecordType recordType;
        [SetUp]
        public void Setup()
        {
            recordType = new RecordType("A00420", 1, 2023);
        }

        [Test]
        public void TestEquals1()
        {
            object other = new RecordType("A00420", 1, 2023);
            Assert.That(recordType.Equals(other), Is.True);
        }

        [Test]
        public void TestEquals2()
        {
            object other = new RecordType("B00420", 1, 2023);
            Assert.That(recordType.Equals(other), Is.False);
        }

        [Test]
        public void TestEquals3()
        {
            object other = "A00420";
            Assert.That(recordType.Equals(other), Is.False);
        }

        [Test]
        public void TestToString()
        {
            string expected = "\"A00420\",\"01\",\"2023\"";
            string actual = recordType.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}