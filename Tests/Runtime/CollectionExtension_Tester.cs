#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-03-18
 *	Summary 		        : 
 *	테스트 지침 링크
 *	https://github.com/KorStrix/Unity_DevelopmentDocs/tree/master/Test
 *
 *  Template 		        : Test For Unity Editor V1
   ============================================ */
#endregion Header

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CollectionExtension_Tester
    {
        [Test]
        public void 딕셔너리_AddSafe는_Exception이_나지않습니다()
        {
            // Arrange (데이터 정렬)
            Dictionary<int, int> mapTestTarget = new Dictionary<int, int>();
            mapTestTarget.Add(1, 1);
            bool bIsError_Normal = false;
            bool bIsError_Safe = false;

            // Action (기능 실행)
            try
            {
                mapTestTarget.Add(1, 1);
            }
            catch
            {
                // 중복 Key Add로 인한 Exception
                bIsError_Normal = true;
            }

            try
            {
                mapTestTarget.Add_Safe(1, 1);
                mapTestTarget = null;
                mapTestTarget.Add_Safe(1, 1);
            }
            catch
            {
                bIsError_Safe = true;
            }

            // Assert (맞는지 체크)
            Assert.IsTrue(bIsError_Normal);
            Assert.IsFalse(bIsError_Safe);
        }

        [Test]
        public void 딕셔너리_ContainsKeySafe는_Exception이_나지않습니다()
        {
            // Arrange (데이터 정렬)
            Dictionary<string, int> mapTestTarget = new Dictionary<string, int>();
            mapTestTarget.Add("Test", 1);
            bool bIsError_Normal = false;
            bool bIsError_Safe = false;

            // Action (기능 실행)
            try
            {
                mapTestTarget.ContainsKey(null);
            }
            catch
            {
                // ContainsKey is Null Exception
                bIsError_Normal = true;
            }

            try
            {
                mapTestTarget.ContainsKey_Safe(null);
                mapTestTarget = null;
                mapTestTarget.ContainsKey_Safe(null);
            }
            catch
            {
                bIsError_Safe = true;
            }

            // Assert (맞는지 체크)
            Assert.IsTrue(bIsError_Normal);
            Assert.IsFalse(bIsError_Safe);
        }

        [Test]
        public void 딕셔너리_GetValueSafe는_Exception이_나지않습니다()
        {
            // Arrange (데이터 정렬)
            Dictionary<string, int> mapTestTarget = new Dictionary<string, int>();
            mapTestTarget.Add("Test", 1);
            bool bIsError_Normal = false;
            bool bIsError_Safe = false;

            // Action (기능 실행)
            try
            {
                int iValue = mapTestTarget["Test2"];
            }
            catch
            {
                // ContainsKey is Null Exception
                bIsError_Normal = true;
            }

            try
            {
                int iValue = mapTestTarget.GetValue_OrDefault("Test2");
            }
            catch
            {
                bIsError_Safe = true;
            }

            // Assert (맞는지 체크)
            Assert.IsTrue(bIsError_Normal);
            Assert.IsFalse(bIsError_Safe);
        }
    }
}
