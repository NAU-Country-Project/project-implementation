using NAUCountryA;
using NAUCountryA.Models;
using NAUCountryA.Tables;

namespace NAUCountryATest.Models
{
    public class CommodityTest
    {
        private Commodity commodity;
        [SetUp]
        public void Setup()
        {
            while (true)
            {
                try
                {
                    commodity = new Commodity(37, "Raisins", "RAISINS", 'P', 2022, Service.ToDateTime("4/7/2022"), "A00420");
                    break;
                }
                catch (NullReferenceException)
                {
                    Service.InitializeUserTo(ServiceTest.TEST_USER);
                }
            }
        }

        [Test]
        public void TestEquals1()
        {
            object other = new Commodity(37, "Raisins", "RAISINS", 'P', 2022, new DateTime(2022, 4, 7), "A00420");
            Assert.That(commodity.Equals(other), Is.True);
        }

        [Test]
        public void TestEquals2()
        {
            object other = new Commodity(3,"Raisins", "RAISINS", 'P', 2022, new DateTime(2022, 4, 7), "A00420");
            Assert.That(commodity.Equals(other), Is.False);
        }

        [Test]
        public void TestEquals3()
        {
            object other = 37;
            Assert.That(commodity.Equals(other), Is.False);
        }

        [Test]
        public void TestFormatCommodityCode()
        {
            string expected = "\"0037\"";
            string actual = commodity.FormatCommodityCode();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestToString()
        {
            string expected = "\"0037\",\"Raisins\",\"RAISINS\",\"P\",\"2022\",\"4/7/2022\",\"A00420\"";
            string actual = commodity.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}