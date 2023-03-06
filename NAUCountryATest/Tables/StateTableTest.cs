using NAUCountryA;
using NAUCountryA.Models;
using NAUCountryA.Tables;
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
    public class StateTableTest
    {
        private StateTable tableMockup;
        [SetUp]
        public void Setup()
        {
            try
            {
                tableMockup = new StateTable();
            }
            catch (NullReferenceException)
            {
                Service.InitializeUserTo(new NAUUser("localhost", 2023, "postgres", "naucountrydev"));
            }
        }

        [Test]
        public void TestContainsKey1()
        {
            bool actual = tableMockup.ContainsKey(01);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void TestContainsKey2()
        {
            bool actual = tableMockup.ContainsKey(60);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void TestCount()
        {
            Assert.That(tableMockup.Count, Is.EqualTo(5));
        }

        [Test]
        public void TestGetStateByKeyValid()
        {
            State expected = new State(01, "Alabama", "AL", "A00520");
            State actual = tableMockup[01];
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetStateByKeyInvalid()
        {
            Assert.Throws<KeyNotFoundException>(() => ToDelegate(60));
        }

        private void ToDelegate(int key)
        {
            State type = tableMockup[key];
        }

        [Test]
        public void TestGetKeys()
        {
<<<<<<< HEAD
            IEnumerable<int> expected = new int[] { 01, 02, 04, 05, 06 }.AsEnumerable();
=======
            IEnumerable<int> expected = new int[] { 01, 02, 04, 05, 06 };
>>>>>>> b2a5fd01bc87f883e22bbfe499628390856aedf1
            IEnumerable<int> actual = tableMockup.Keys;
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void TestGetValues()
        {
            IEnumerable<State> expected = new State[]
            {new State(01, "Alabama", "AL", "A00520"),
            new State(02, "Alaska", "AK", "A00520"),
            new State(04, "Arizona", "AZ", "A00520"),
            new State(05, "Arkansas", "AR", "A00520"),
            new State(06, "California", "CA", "A00520")};
            IEnumerable<State> actual = tableMockup.Values;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetEnumerator()
        {
<<<<<<< HEAD
            ICollection < KeyValuePair<int, State>> expected = new HashSet<KeyValuePair<int, State>>();
=======
            ICollection<KeyValuePair<int,State>> expected = new HashSet<KeyValuePair<int,State>>();
>>>>>>> b2a5fd01bc87f883e22bbfe499628390856aedf1
            expected.Add(new KeyValuePair<int, State>(01, new State(01, "Alabama", "AL", "A00520")));
            expected.Add(new KeyValuePair<int, State>(02, new State(02, "Alaska", "AK", "A00520")));
            expected.Add(new KeyValuePair<int, State>(04, new State(04, "Arizona", "AZ", "A00520")));
            expected.Add(new KeyValuePair<int, State>(05, new State(05, "Arkansas", "AR", "A00520")));
            expected.Add(new KeyValuePair<int, State>(06, new State(06, "California", "CA", "A00520")));
            ICollection<KeyValuePair<int, State>> actual = new HashSet<KeyValuePair<int, State>>();
            foreach (KeyValuePair<int, State> pair in tableMockup)
            {
                actual.Add(pair);
            }
            Assert.That(actual, Is.EqualTo(actual));
        }

        [Test]
        public void TestTryGet1()
        {
            State test = null;
            Assert.That(tableMockup.TryGetValue(01, out test), Is.True);
        }

        [Test]
        public void TestTryGet2()
        {
            State test = null;
            Assert.That(tableMockup.TryGetValue(60, out test), Is.False);
        }
    }
}