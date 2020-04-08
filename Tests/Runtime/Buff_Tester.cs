#region Header
/*	============================================
 *	Author 			    	: #AUTHOR#
 *	Initial Creation Date 	: 2020-04-07
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

using Unity_Pattern;
using System.Linq;

namespace Tests
{
    public enum EBuffName
    {
        버프_공격력증가,

        디버프_둔화,
    }

    public enum EBuffAttribute
    {
        공격력증가,

        이동속도_감소,
        공격속도_감소,
    }

    public class TestBuffContainer : IBuffContainer
    {
        public string IBuffContainer_strBuffKey => _strBuffKey;

        public float IBuffContainer_fDurationSec => _fDurationSec;

        public IEnumerable<IBuffAttribute> IBuffContainer_arrAttribute => _listBuffAttribute;


        List<IBuffAttribute> _listBuffAttribute;
        string _strBuffKey;
        float _fDurationSec;

        public TestBuffContainer(EBuffName eBuffName, float fDurationSec, params IBuffAttribute[] arrBuffAttribute)
        {
            _strBuffKey = eBuffName.ToString();
            _fDurationSec = fDurationSec;
            _listBuffAttribute = arrBuffAttribute.ToList();
        }

        public void IBuffContainer_OnPlay()
        {
        }

        public void IBuffContainer_OnReset()
        {
        }
    }

    public class TestBuffAttribute : IBuffAttribute
    {
        public string IBuffAttribute_strBuffAttributeKey => _strBuffAttributeKey;

        public float fPower { get; private set; }

        string _strBuffAttributeKey;

        public TestBuffAttribute(EBuffAttribute eAttribute, float fPower)
        {
            this._strBuffAttributeKey = eAttribute.ToString(); this.fPower = fPower;
        }


        public void IBuffAttribute_OnPlay()
        {
        }

        public void IBuffAttribute_OnReset()
        {
        }
    }

    public class Character_BuffTester
    {
        public const float const_fDamage_Init = 1f;
        public const float const_fMoveSpeed_Init = 2f;
        public const float const_fAttackSpeed_Init = 1f;


        public string strCharacterName;
        public float fDamage = const_fDamage_Init;
        public float fMoveSpeed = const_fMoveSpeed_Init;
        public float fAttackSpeed = const_fAttackSpeed_Init;

        public Character_BuffTester(string strCharacterName)
        {
            this.strCharacterName = strCharacterName;
        }

        public string GetStatString()
        {
            return $"{strCharacterName} - fDamage : {fDamage} / fMoveSpeed : {fMoveSpeed} / fAttackSpeed : {fAttackSpeed}";
        }
    }

    public class Buff_Tester
    {
        [UnityTest]
        public IEnumerator 단일버프_동작테스트()
        {
            // Arrange (데이터 정렬)
            string strCharacterName = nameof(여러버프_동작테스트);
            float fDurationSec = 1f;
            float fBuffPower = 2f;

            Character_BuffTester pCharacterTester = new Character_BuffTester(strCharacterName);
            TestBuffContainer pBuff = new TestBuffContainer(EBuffName.버프_공격력증가, fDurationSec, arrBuffAttribute: new TestBuffAttribute(EBuffAttribute.공격력증가, fBuffPower));



            // Act (기능 실행)
            Assert.AreEqual(pCharacterTester.strCharacterName, strCharacterName);
            Assert.AreEqual(pCharacterTester.fDamage * fBuffPower, Character_BuffTester.const_fDamage_Init * fBuffPower);
            Assert.AreEqual(pCharacterTester.fAttackSpeed, Character_BuffTester.const_fAttackSpeed_Init);
            Assert.AreEqual(pCharacterTester.fMoveSpeed, Character_BuffTester.const_fMoveSpeed_Init);



            // Assert (맞는지 체크)
            Assert.AreEqual(pCharacterTester.strCharacterName, strCharacterName);
            Assert.AreEqual(pCharacterTester.fDamage, Character_BuffTester.const_fDamage_Init);
            Assert.AreEqual(pCharacterTester.fAttackSpeed, Character_BuffTester.const_fAttackSpeed_Init);
            Assert.AreEqual(pCharacterTester.fMoveSpeed, Character_BuffTester.const_fMoveSpeed_Init);


            yield break;
        }

        [UnityTest]
        public IEnumerator 여러버프_동작테스트()
        {
            // Arrange (데이터 정렬)
            string strCharacterName = nameof(여러버프_동작테스트);
            float fDurationSec = 0.5f;
            float fBuffPower_AttackSpeed = 0.1f;
            float fBuffPower_MoveSpeed = 0.5f;

            Character_BuffTester pCharacterTester = new Character_BuffTester(strCharacterName);
            TestBuffContainer pBuff = new TestBuffContainer(EBuffName.디버프_둔화, fDurationSec, new TestBuffAttribute(EBuffAttribute.공격속도_감소, fBuffPower_AttackSpeed), new TestBuffAttribute(EBuffAttribute.이동속도_감소, fBuffPower_MoveSpeed));



            // Act (기능 실행)
            Assert.AreEqual(pCharacterTester.strCharacterName, strCharacterName);
            Assert.AreEqual(pCharacterTester.fDamage, Character_BuffTester.const_fDamage_Init);
            Assert.AreEqual(pCharacterTester.fAttackSpeed * fBuffPower_AttackSpeed, Character_BuffTester.const_fAttackSpeed_Init * fBuffPower_AttackSpeed);
            Assert.AreEqual(pCharacterTester.fMoveSpeed * fBuffPower_MoveSpeed, Character_BuffTester.const_fMoveSpeed_Init * fBuffPower_MoveSpeed);



            // Assert (맞는지 체크)
            Assert.AreEqual(pCharacterTester.strCharacterName, strCharacterName);
            Assert.AreEqual(pCharacterTester.fDamage, Character_BuffTester.const_fDamage_Init);
            Assert.AreEqual(pCharacterTester.fAttackSpeed, Character_BuffTester.const_fAttackSpeed_Init);
            Assert.AreEqual(pCharacterTester.fMoveSpeed, Character_BuffTester.const_fMoveSpeed_Init);

            yield break;
        }
    }
}
