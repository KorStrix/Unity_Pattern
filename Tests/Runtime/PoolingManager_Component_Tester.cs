using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity_Pattern;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityPattern_Test
{
    public class PoolingManager_Component_Tester
    {
        public enum ETestPoolingObjectName
        {
            Test1,
            Test2,

            Max,
        }

        public class TestPoolingObject : MonoBehaviour
        {
            protected static Dictionary<ETestPoolingObjectName, int> g_mapActiveCount;
            public ETestPoolingObjectName eTestType;

            public static void ResetActiveCount()
            {
                g_mapActiveCount = new Dictionary<ETestPoolingObjectName, int>() { { ETestPoolingObjectName.Test1, 0 }, { ETestPoolingObjectName.Test2, 0 } };
            }

            public static int GetActiveCount(ETestPoolingObjectName eTestPoolingObjectName)
            {
                return g_mapActiveCount[eTestPoolingObjectName];
            }

            private void OnEnable() { g_mapActiveCount[eTestType]++; }
            private void OnDisable() { g_mapActiveCount[eTestType]--; }
        }

        [Test]
        public void WorkingTest()
        {
            // Assert
            PoolingManager_Component<TestPoolingObject> pPoolingManager = PoolingManager_Component<TestPoolingObject>.instance;
            pPoolingManager.DoDestroyAll();
            Dictionary<ETestPoolingObjectName, TestPoolingObject> mapObjectInstance = InitTest();

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));


            // Test1
            List<TestPoolingObject> listObjectPooling = new List<TestPoolingObject>();
            for (int i = 0; i < 10; i++)
                listObjectPooling.Add(pPoolingManager.DoPop(mapObjectInstance[ETestPoolingObjectName.Test1]));

            Assert.AreEqual(10, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));

            for (int i = 0; i < listObjectPooling.Count; i++)
                pPoolingManager.DoPush(listObjectPooling[i]);

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));


            // Test2
            listObjectPooling.Clear();
            for (int i = 0; i < 5; i++)
                listObjectPooling.Add(pPoolingManager.DoPop(mapObjectInstance[ETestPoolingObjectName.Test2]));

            // Active Check
            for (int i = 0; i < listObjectPooling.Count; i++)
                Assert.AreEqual(true, listObjectPooling[i].gameObject.activeSelf);

            Assert.AreEqual(5, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));

            for (int i = 0; i < listObjectPooling.Count; i++)
                pPoolingManager.DoPush(listObjectPooling[i]);

            // Active Check - 리턴했기 때문에 False
            for (int i = 0; i < listObjectPooling.Count; i++)
                Assert.AreEqual(false, listObjectPooling[i].gameObject.activeSelf);

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));
        }


        [Test]
        public void 미리생성된_오브젝트를_풀에넣고_테스트()
        {
            // Assert
            PoolingManager_Component<TestPoolingObject> pPoolingManager = PoolingManager_Component<TestPoolingObject>.instance;
            pPoolingManager.DoDestroyAll();
            Dictionary<ETestPoolingObjectName, TestPoolingObject> mapObjectInstance = InitTest();

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));


            // Act
            // 게임 오브젝트를 Instantiate를 통해 생성합니다.
            List<TestPoolingObject> listObject = new List<TestPoolingObject>();
            for (int i = 0; i < 10; i++)
                listObject.Add(GameObject.Instantiate(mapObjectInstance[ETestPoolingObjectName.Test1]));


            // 생성한 것을 풀에 넣습니다
            pPoolingManager.DoAdd_PoolObject(mapObjectInstance[ETestPoolingObjectName.Test1], listObject);

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(listObject.Count, pPoolingManager.iInstanceCount);


            // Assert
            for (int i = 0; i < listObject.Count; i++)
                pPoolingManager.DoPop(mapObjectInstance[ETestPoolingObjectName.Test1]);

            Assert.AreEqual(listObject.Count, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(listObject.Count, pPoolingManager.iUseCount);
            Assert.AreEqual(listObject.Count, pPoolingManager.iInstanceCount);

            pPoolingManager.DoPushAll();

            Assert.AreEqual(pPoolingManager.iUseCount, 0);
            Assert.AreEqual(TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1), 0);


            // 풀에 있는 인스턴스보다 더 많이 얻어와봄
            {
                int iCount = listObject.Count * 2;
                for (int i = 0; i < iCount; i++)
                    pPoolingManager.DoPop(mapObjectInstance[ETestPoolingObjectName.Test1]);

                Assert.AreEqual(iCount, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
                Assert.AreEqual(iCount, pPoolingManager.iUseCount);
                Assert.AreEqual(iCount, pPoolingManager.iInstanceCount);
            }
        }

        private Dictionary<ETestPoolingObjectName, TestPoolingObject> InitTest()
        {
            TestPoolingObject.ResetActiveCount();

            Dictionary<ETestPoolingObjectName, TestPoolingObject> mapObjectPooling = new Dictionary<ETestPoolingObjectName, TestPoolingObject>();
            for (int i = 0; i < (int)ETestPoolingObjectName.Max; i++)
            {
                ETestPoolingObjectName eTest = (ETestPoolingObjectName)i;
                GameObject pObjectOrigin_Test = new GameObject(eTest.ToString());
                pObjectOrigin_Test.gameObject.SetActive(false);

                TestPoolingObject pTestPoolingObject = pObjectOrigin_Test.AddComponent<TestPoolingObject>();
                pTestPoolingObject.eTestType = eTest;
                mapObjectPooling.Add(eTest, pTestPoolingObject);
            }

            return mapObjectPooling;
        }
    }
}
#endif
