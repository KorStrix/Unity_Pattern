using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity_Pattern;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityPattern_Test
{
    [Category("UnityPattern")]
    public class PoolingManager_Tester
    {
        public class PoolingObjectTest
        {
            public string strText;
        }

        /// <summary>
        /// 일반 클래스 풀링 테스트
        /// </summary>
        [Test]
        public void NormalClass_PoolTest()
        {
            PoolingObjectTest pPoolingOrigin = new PoolingObjectTest();
            pPoolingOrigin.strText = "원본 클래스";
            Assert.AreEqual(pPoolingOrigin.strText, "원본 클래스");

            List<PoolingObjectTest> listPooling = new List<PoolingObjectTest>();
            for (int i = 0; i < 10; i++)
            {
                PoolingObjectTest pPoolObject = PoolingManager_NormalClass<PoolingObjectTest>.instance.DoPop(pPoolingOrigin);
                pPoolObject.strText = i.ToString(); // 인스턴스가 각자 다르기 때문에 다른 값이 세팅

                listPooling.Add(pPoolObject);
            }

            for (int i = 0; i < listPooling.Count; i++)
            {
                PoolingObjectTest pPoolObject = listPooling[i];
                Assert.AreEqual(pPoolObject.strText, i.ToString()); // 인스턴스가 각자 다른지 확인
            }

            PoolingManager_NormalClass<PoolingObjectTest>.instance.DoPushAll();

            // 10번 생성 후 모두 리턴을 5번씩
            for (int i = 0; i < 5; i++)
            {
                listPooling.Clear();
                for (int j = 0; j < 10; j++)
                {
                    PoolingObjectTest pPoolObject = PoolingManager_NormalClass<PoolingObjectTest>.instance.DoPop(pPoolingOrigin);
                    listPooling.Add(pPoolObject);
                }

                PoolingManager_NormalClass<PoolingObjectTest>.instance.DoPushAll();
            }

            Assert.AreEqual(PoolingManager_NormalClass<PoolingObjectTest>.instance.iInstanceCount, 10); // 최대 생성 수는 10번이다.
            Assert.AreEqual(pPoolingOrigin.strText, "원본 클래스"); // 원본 오브젝트는 변함이 없다.
        }
    }

}
#endif
