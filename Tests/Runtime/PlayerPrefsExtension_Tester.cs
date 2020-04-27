#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-21 오후 4:43:48
 *	개요 : 
   ============================================ */
#endregion Header

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Unity_Pattern;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    [Category("StrixLibrary")]
    public class PlayerPrefsWrapper_Tester
    {
        [System.Serializable]
        public class SaveTest_Safe
        {
            public int iValue;
            public ulong uValue;
            public string strValue;
            public float fValue;
        }

        public class SaveTest_UnSafe
        {
            [System.NonSerialized]
            public int iValue;
            [System.NonSerialized]
            public string strValue;
            [System.NonSerialized]
            public float fValue;
        }

        [Test]
        public void SaveLoad_BasicTest()
        {
            SaveTest_Safe pTest = new SaveTest_Safe();
            pTest.iValue = Random.Range(1, 100);
            pTest.uValue = ulong.MaxValue;
            
            pTest.strValue = pTest.iValue.ToString();
            pTest.fValue = Random.Range(-1f, 1f);

            PlayerPrefsExtension.SetObject(nameof(SaveLoad_BasicTest), pTest, null);

            SaveTest_Safe pTest_ForCheck = new SaveTest_Safe();

            Assert.AreNotEqual(pTest.iValue, pTest_ForCheck.iValue);
            Assert.AreNotEqual(pTest.uValue, pTest_ForCheck.uValue);
            Assert.AreNotEqual(pTest.strValue, pTest_ForCheck.strValue);
            Assert.AreNotEqual(pTest.fValue, pTest_ForCheck.fValue);

            bool bResult_Safe = PlayerPrefsExtension.GetObject(nameof(SaveLoad_BasicTest), ref pTest_ForCheck);
            Assert.AreEqual(bResult_Safe, true);

            Assert.AreEqual(pTest.iValue, pTest_ForCheck.iValue);
            Assert.AreEqual(pTest.uValue, pTest_ForCheck.uValue);
            Assert.AreEqual(pTest.strValue, pTest_ForCheck.strValue);
            Assert.AreEqual(pTest.fValue, pTest_ForCheck.fValue);

            // Error - Not Found Key
            bResult_Safe = PlayerPrefsExtension.GetObject("Test2", ref pTest_ForCheck);
            Assert.AreEqual(bResult_Safe, false);

            SaveTest_UnSafe pTest_UnSafe = new SaveTest_UnSafe();
            pTest_UnSafe.iValue = Random.Range(1, 100);
            pTest_UnSafe.strValue = pTest.iValue.ToString();
            pTest_UnSafe.fValue = Random.Range(-1f, 1f);

            PlayerPrefsExtension.SetObject("Test_UnSafe", pTest_UnSafe);

            SaveTest_UnSafe pTest_ForCheck_UnSafe = new SaveTest_UnSafe();

            Assert.AreNotEqual(pTest_UnSafe.iValue, pTest_ForCheck_UnSafe.iValue);
            Assert.AreNotEqual(pTest_UnSafe.strValue, pTest_ForCheck_UnSafe.strValue);
            Assert.AreNotEqual(pTest_UnSafe.fValue, pTest_ForCheck_UnSafe.fValue);

            bool bResult_UnSafe = PlayerPrefsExtension.GetObject("Test_UnSafe", ref pTest_ForCheck_UnSafe);
            Assert.AreEqual(bResult_UnSafe, false);

            Assert.AreNotEqual(pTest_UnSafe.iValue, pTest_ForCheck_UnSafe.iValue);
            Assert.AreNotEqual(pTest_UnSafe.strValue, pTest_ForCheck_UnSafe.strValue);
            Assert.AreNotEqual(pTest_UnSafe.fValue, pTest_ForCheck_UnSafe.fValue);
        }

        [Test]
        public void SaveLoad_EncryptTest()
        {
            SaveTest_Safe pTest = new SaveTest_Safe();
            pTest.iValue = Random.Range(1, 100);
            pTest.strValue = pTest.iValue.ToString();
            pTest.fValue = Random.Range(-1f, 1f);

            // 암호화 저장
            PlayerPrefsExtension.SetObject_Encrypt(nameof(SaveLoad_EncryptTest), pTest);

            SaveTest_Safe pTest_ForCheck = new SaveTest_Safe();

            Assert.AreNotEqual(pTest.iValue, pTest_ForCheck.iValue);
            Assert.AreNotEqual(pTest.strValue, pTest_ForCheck.strValue);
            Assert.AreNotEqual(pTest.fValue, pTest_ForCheck.fValue);

            // Encrypt한 오브젝트를 일반 Get으로 하면 에러
            Assert.IsFalse(PlayerPrefsExtension.GetObject(nameof(SaveLoad_EncryptTest), ref pTest_ForCheck));
            Assert.AreNotEqual(pTest.iValue, pTest_ForCheck.iValue);
            Assert.AreNotEqual(pTest.strValue, pTest_ForCheck.strValue);
            Assert.AreNotEqual(pTest.fValue, pTest_ForCheck.fValue);

            // 암호화 불러오기
            Assert.IsTrue(PlayerPrefsExtension.GetObject_Encrypted(nameof(SaveLoad_EncryptTest), ref pTest_ForCheck));
            Assert.AreEqual(pTest.iValue, pTest_ForCheck.iValue);
            Assert.AreEqual(pTest.strValue, pTest_ForCheck.strValue);
            Assert.AreEqual(pTest.fValue, pTest_ForCheck.fValue);
        }
    }
}