using System;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using NAUCountryA;
using NAUCountryA.Models;
using NAUCountryA.Tables;

namespace NAUCountryATest.Tables
{
    public class OfferTableTest
    {
        /*private OfferTable tableMockup;
        [SetUp]
        public void Setup()
        {
            try
            {
                tableMockup = new OfferTable();
            }
            catch (NullReferenceException)
            {
                Service.InitializeUserTo(new NAUUser("localhost", 2023, "postgres", "naucountrydev"));
            }
        }

        [Test]
        public void TestContainsKey1()
        {
            bool actual = tableMockup.ContainsKey(25360764);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void TestContainsKey2()
        {
            bool actual = tableMockup.ContainsKey(2536076);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void TestCount()
        {
            Assert.That(tableMockup.Count, Is.EqualTo(5));
        }

        [Test]
        public void TestGetOfferByKeyValid()
        {
            Offer expected = new Offer(25360764, 06, 092, 019, 997, 997, 2022);
            Offer actual = tableMockup[25360764];
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOfferByKeyInvalid()
        {
            Assert.Throws<KeyNotFoundException>(() => ToDelegate(2536076));
        }

        private void ToDelegate(int key)
        {
            Offer type = tableMockup[key];
        }

        [Test]
        public void TestGetKeys()
        {
            IEnumerable<int> expected = new int[] { 25360764, 25343586, 25375014, 27870877, 27869719 }.AsEnumerable();
            IEnumerable<int> actual = tableMockup.Keys;
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void TestGetValues()
        {
            IEnumerable<Offer> expected = new Offer[]
            {new Offer(25360764, 06, 092, 019, 997, 997, 2022),
            new Offer(25343586, 06, 093, 019, 997, 997, 2022),
            new Offer(25375014, 06, 775, 019, 997, 997, 2022),
            new Offer(27870877, 06, 092, 019, 997, 997, 2023),
            new Offer(27869719, 06, 093, 019, 997, 997, 2023)};
            IEnumerable<Offer> actual = tableMockup.Values;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetEnumerator()
        {
            ICollection < KeyValuePair<int,Offer>> expected = new HashSet<KeyValuePair<int,Offer>>();
            expected.Add(new KeyValuePair<int, Offer>(25360764, new Offer(25360764, 06, 092, 019, 997, 997, 2022)));
            expected.Add(new KeyValuePair<int, Offer>(25343586, new Offer(25343586, 06, 093, 019, 997, 997, 2022)));
            expected.Add(new KeyValuePair<int, Offer>(25375014, new Offer(25375014, 06, 775, 019, 997, 997, 2022)));
            expected.Add(new KeyValuePair<int, Offer>(27870877, new Offer(27870877, 06, 092, 019, 997, 997, 2023)));
            expected.Add(new KeyValuePair<int, Offer>(27869719, new Offer(27869719, 06, 093, 019, 997, 997, 2023)));
            ICollection<KeyValuePair<int, Offer>> actual = new HashSet<KeyValuePair<int,Offer>>();
            foreach (KeyValuePair<int, Offer> pair in tableMockup)
            {
                actual.Add(pair);
            }
            Assert.That(actual, Is.EqualTo(actual));
        }

        [Test]
        public void TestTryGet1()
        {
            Offer test = null;
            Assert.That(tableMockup.TryGetValue(27869719, out test), Is.True);
        }

        [Test]
        public void TestTryGet2()
        {
            Offer test = null;
            Assert.That(tableMockup.TryGetValue(2786971, out test), Is.False);
        }*/
    }
}