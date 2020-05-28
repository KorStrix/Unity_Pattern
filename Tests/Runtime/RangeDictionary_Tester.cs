using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace StrixLibrary_Test
{
    [Category("StrixLibrary")]
    public class RangeDictionary_Tester
    {
        [Test]
        public void RangeDictionary_Dont_Add_OverlapRange()
        {
            // Arrange
            RangeDictionary<int, string> rangeDictionary = new RangeDictionary<int, string>();



            // Act && Assert
            Assert.IsTrue(rangeDictionary.Add(1, 10, "1~10"));
            Assert.IsFalse(rangeDictionary.Add(5, 10, "5~10")); // Fail
            Assert.IsTrue(rangeDictionary.Add(11, 20, "11~20"));

            Assert.IsTrue(rangeDictionary.Remove(1, 10));
            Assert.IsTrue(rangeDictionary.Add(5, 10, "5~10")); // 위에선 실패했으나 이제 성공
        }

        [Test]
        public void RangeDictionary_Is_Working()
        {
            // Arrange
            RangeDictionary<int, string> rangeDictionary = new RangeDictionary<int, string>();



            // Act
            Assert.IsTrue(rangeDictionary.Add(-10, 0, "-10~0"));
            Assert.IsTrue(rangeDictionary.Add(1, 10, "1~10"));
            Assert.IsTrue(rangeDictionary.Add(11, 20, "11~20"));


            // Assert
            // True Case
            for (int i = -10; i <= 0; i++)
                Assert.AreEqual(rangeDictionary.GetValue(i), "-10~0");

            for (int i = 1; i <= 10; i++)
                Assert.AreEqual(rangeDictionary.GetValue(i), "1~10");

            for (int i = 11; i <= 20; i++)
                Assert.AreEqual(rangeDictionary.GetValue(i), "11~20");


            // False Case
            for (int i = 0; i < 10; i++)
                Assert.AreNotEqual(rangeDictionary.GetValue(UnityEngine.Random.Range(11, 100000)), "1~10");

            // Null Case
            for (int i = 0; i < 10; i++)
                Assert.AreEqual(rangeDictionary.GetValue(UnityEngine.Random.Range(21, 100000)), null);
        }


        [Test]
        public void 레인지딕셔너리는_IDictionary를_상속받습니다()
        {
            RangeDictionary<int, string> rangeDictionary = new RangeDictionary<int, string>();

            Assert.IsTrue(rangeDictionary.Add(1, 10, "1~10"));
            Assert.IsTrue(rangeDictionary.Add(11, 20, "11~20"));



            Dictionary<Range<int>, string> dictionary_Readonly = new Dictionary<Range<int>, string>(rangeDictionary, new Range<int>.Comparer());


            int iValue = 1;
            Assert.IsTrue(rangeDictionary.ContainsKey(iValue));

            // Todo 차후 해야함
            // Assert.IsTrue(dictionary_Readonly.ContainsKey(iValue));

            // Assert.AreEqual(rangeDictionary[iValue], dictionary_Readonly[new Range<int>(iValue)]);
            // Assert.AreEqual(rangeDictionary[iValue], dictionary_Readonly[iValue]);
        }

        [Test]
        public void 레인지딕셔너리는_현재키보다_작은키의데이터를_얻을수있습니다()
        {
            // Arrange
            RangeDictionary<DateTime, string> rangeDictionary = new RangeDictionary<DateTime, string>();
            DateTime sDateTimeCurrent = DateTime.Now;
            Assert.IsTrue(rangeDictionary.Add(sDateTimeCurrent.AddDays(0), sDateTimeCurrent.AddDays(10), "0~10"));
            Assert.IsTrue(rangeDictionary.Add(sDateTimeCurrent.AddDays(11), sDateTimeCurrent.AddDays(30), "11~30"));



            // Act
            // Assert
            for (int i = 10; i >= 0; i--)
                Assert.AreEqual(rangeDictionary.GetValue(sDateTimeCurrent.AddDays(i)), "0~10");

            for (int i = 30; i >= 11; i--)
                Assert.AreEqual(rangeDictionary.GetValue(sDateTimeCurrent.AddDays(i)), "11~30");


            for (int i = 11; i < 20; i++)
            {
                Assert.IsTrue(rangeDictionary.TryGetValue_LesserThenKey(sDateTimeCurrent.AddDays(i), out string strValue));
                Assert.AreEqual(strValue, "0~10");
            }

            for (int i = 30; i >= 11; i--)
            {
                Assert.IsTrue(rangeDictionary.TryGetValue_LesserThenKey(sDateTimeCurrent.AddDays(i), out string strValue));
                Assert.AreEqual(strValue, "0~10");
            }
        }
    }

}