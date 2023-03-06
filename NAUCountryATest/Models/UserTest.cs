using NAUCountryA.Models;
namespace NAUCountryATest.Models
{
    public class UserTest
    {
        
        [Test]
        public void TestConnection()
        {
            string expected = "Host=localhost;Port=2023;Username=postgres;Database=NAUCountryData";
            Assert.That(ServiceTest.TEST_USER.Connection.ConnectionString, Is.EqualTo(expected));
        }
    }
}