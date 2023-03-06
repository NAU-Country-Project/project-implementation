using NAUCountryA;
using NAUCountryA.Models;

namespace NAUCountryATest.Tables
{
    public class CommodityTableTest
    {
        [SetUp]
        public void Setup()
        {
            Service.InitializeUserTo(ServiceTest.TEST_USER);
        }

        [Test]
        public void TestCount()
        {
            Assert.That(Service.CommodityEntries.Count,Is.EqualTo(140));
        }
    }
}