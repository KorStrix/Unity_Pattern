using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    [Category("StrixLibrary")]
    public class Scatter_Tester : MonoBehaviour
    {
        bool _bIsTestRunning;

        [UnityTest]
        public IEnumerator Scatter_Test()
        {
            Scatter_Tester pScatterTester = new GameObject(nameof(Scatter_Tester)).AddComponent<Scatter_Tester>();

            List<Transform> listScatterObject = new List<Transform>();
            int iRandomCount = Random.Range(5, 10);
            float fRandomRange = Random.Range(0f, 5f);
            float fRandomDelay = Random.Range(0f, 1f);

            Debug.Log("Count : " + iRandomCount);

            for (int i = 0; i < iRandomCount; i++)
                listScatterObject.Add(new GameObject(i.ToString()).transform);

            _bIsTestRunning = true;
            Scatter.DoScattterCoroutine(pScatterTester, listScatterObject.ToArray(), Vector3.zero, fRandomRange, fRandomDelay,
                null,
                (Transform[] arrObject) =>
                {   
                    Debug.Log("OnFinish Scatter");
                    _bIsTestRunning = false;
                });

            Assert.IsTrue(_bIsTestRunning);
            while(_bIsTestRunning)
            {
                yield return null;
            }

            Debug.Log(nameof(Scatter_Test) + " is Finish");
        }
    }
}
