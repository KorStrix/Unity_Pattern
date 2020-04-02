#region Header
/*	============================================
 *	Author 			    	: #AUTHOR#
 *	Initial Creation Date 	: 2020-04-02
 *	Summary 		        : 
 *	테스트 지침 링크
 *	https://github.com/KorStrix/Unity_DevelopmentDocs/Test
 *
 *  Template 		        : Test For Unity Editor V1
   ============================================ */
#endregion Header

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FieldWrapper_Tester
    {
        [Test]
        public void 필드래퍼는_초기값을_할당할수_있습니다()
        {
            // Arrange (데이터 정렬)
            int iTestValue = Random.Range(0, 5);
            string strValue = Random.Range(5, 10).ToString();



            // Act (기능 실행)
            // 디폴트 버전
            FieldWrapper<int> pFieldWrapper_Int_0 = new FieldWrapper<int>();
            FieldWrapper<string> pFieldWrapper_String_Null = new FieldWrapper<string>();

            // 초기값 할당 버전
            FieldWrapper<int> pFieldWrapper_Int_1 = new FieldWrapper<int>(iTestValue);
            FieldWrapper<string> pFieldWrapper_String = new FieldWrapper<string>(strValue);



            // Assert (맞는지 체크)
            Assert.AreEqual(pFieldWrapper_Int_0.Value, default(int));
            Assert.AreEqual(pFieldWrapper_String_Null.Value, default(string));

            Assert.AreEqual(pFieldWrapper_Int_1.Value, iTestValue);
            Assert.AreEqual(pFieldWrapper_String.Value, strValue);
        }


        [Test]
        public void 필드래퍼는_래핑한변수로_변환이가능합니다()
        {
            // Arrange (데이터 정렬)
            int iTestValue = Random.Range(0, 5);
            int iTestValue_Add = Random.Range(6, 10);
            FieldWrapper<int> pFieldWrapper_Int = new FieldWrapper<int>(iTestValue);



            // Act (기능 실행)
            pFieldWrapper_Int.Value += iTestValue_Add;
            int iTestValue_Cast = pFieldWrapper_Int;



            // Assert (맞는지 체크)
            Assert.AreEqual(pFieldWrapper_Int.Value, iTestValue + iTestValue_Add);
            Assert.AreEqual(iTestValue_Cast, iTestValue + iTestValue_Add);
            Assert.IsTrue(iTestValue_Cast == iTestValue + iTestValue_Add);
        }


        int iListenCount;

        [Test]
        public void 필드래퍼는_옵저버를추가할때_조건함수를_붙일수있습니다()
        {
            // Arrange (데이터 정렬)
            int iTestValue = 3;
            FieldWrapper<int> pFieldWrapper_Int = new FieldWrapper<int>(iTestValue);
            pFieldWrapper_Int.DoAddObserver_WithCondition(값이_5이상일때, 알람을_받았다_1);
            iListenCount = 0;



            // Act (기능 실행)
            pFieldWrapper_Int.Value += 1;

            // Assert (맞는지 체크)
            Assert.AreEqual(pFieldWrapper_Int.Value, 4);
            Assert.AreEqual(iListenCount, 0);



            // Act (기능 실행)
            pFieldWrapper_Int.Value += 1;

            // Assert (맞는지 체크)
            Assert.AreEqual(pFieldWrapper_Int.Value, 5);
            Assert.AreEqual(iListenCount, 1);
        }



        [Test]
        public void 필드래퍼는_값이변경될때_알려줍니다()
        {
            // Arrange (데이터 정렬)
            int iTestValue = Random.Range(0, 5);
            int iTestValue_Add = Random.Range(6, 10);
            FieldWrapper<int> pFieldWrapper_Int = new FieldWrapper<int>(iTestValue, 알람을_받았다_1);
            iListenCount = 0;



            // Act (기능 실행)
            pFieldWrapper_Int.Value += iTestValue_Add;

            // Assert (맞는지 체크)
            Assert.AreEqual(iListenCount, 1);



            // Act (기능 실행)
            pFieldWrapper_Int.Subscribe += 알람을_받았다_2;
            pFieldWrapper_Int.Value += iTestValue_Add;

            // Assert (맞는지 체크)
            Assert.AreEqual(iListenCount, 3);
        }


        [Test]
        public void 필드래퍼는_한번만알림기능을_지원합니다()
        {
            // Arrange (데이터 정렬)
            int iTestValue = 3;
            FieldWrapper<int> pFieldWrapper_Int = new FieldWrapper<int>(iTestValue);
            pFieldWrapper_Int.DoAddObserver_WithCondition(값이_5이상일때, 알람을_받았다_1, true);
            iListenCount = 0;



            // Act (기능 실행)
            pFieldWrapper_Int.Value += 1;

            // Assert (맞는지 체크)
            Assert.AreEqual(iListenCount, 0);
            Assert.AreEqual(pFieldWrapper_Int.iObserverCount, 1);


            // Act (기능 실행)
            pFieldWrapper_Int.Value += 1;

            // Assert (맞는지 체크)
            Assert.AreEqual(iListenCount, 1);
            Assert.AreEqual(pFieldWrapper_Int.iObserverCount, 0);
        }



        private bool 값이_5이상일때(int iValue)
        {
            return iValue >= 5;
        }

        private void 알람을_받았다_1(int iValue)
        {
            iListenCount++;
        }

        private void 알람을_받았다_2(int iValue)
        {
            iListenCount++;
        }
    }
}
