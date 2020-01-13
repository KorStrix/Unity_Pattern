using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    [Category("StrixLibrary")]
    public class UnityExtension_Tester
    {
        [UnityTest]
        public IEnumerator GameObject_IsNull_Test()
        {
            GameObject pObjectTest = new GameObject();
            Assert.IsTrue(pObjectTest != null);

            // GameObject를 파괴 후 바로 테스트
            GameObject.Destroy(pObjectTest);
            bool bOperator_EqualNull, bOperator_IsNull, bReferenceEqualNull;
            Calculate_IsDestory(pObjectTest, out bOperator_EqualNull, out bOperator_IsNull, out bReferenceEqualNull);

            // 하나라도 False면 IsNull는 True
            Assert.AreEqual((bOperator_EqualNull || bOperator_IsNull || bReferenceEqualNull ), pObjectTest.IsNull());



            // GameObject를 파괴 후 한프레임 대기 후 테스트
            yield return null;

            Calculate_IsDestory(pObjectTest, out bOperator_EqualNull, out bOperator_IsNull, out bReferenceEqualNull);
            Assert.AreEqual((bOperator_EqualNull || bOperator_IsNull || bReferenceEqualNull), pObjectTest.IsNull());



            // Null을 대입한 후 테스트
            pObjectTest = null;

            Calculate_IsDestory(pObjectTest, out bOperator_EqualNull, out bOperator_IsNull, out bReferenceEqualNull);
            Assert.AreEqual((bOperator_EqualNull || bOperator_IsNull || bReferenceEqualNull), pObjectTest.IsNull());
        }

        private static void Calculate_IsDestory(GameObject pObjectTest, out bool bOperator_Equal, out bool bOperator_Is, out bool bReferenceEqual)
        {
            bOperator_Equal = pObjectTest == null;
            bOperator_Is = pObjectTest is null;
            bReferenceEqual = ReferenceEquals(pObjectTest, null);
        }
    }

}
