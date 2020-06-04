using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    [Category("StrixLibrary")]
    public class Singleton_Tester
    {
        public class Singleton_Dynamic_1 : CSingletonNotMonoBase<Singleton_Dynamic_1>
        {
        }

        Singleton_Dynamic_1 _pManagerMember_Instance = Singleton_Dynamic_1.instance;

        [Test]
        public void NotMono싱글톤은_멤버에서_선언해도_단일인스턴스입니다()
        {
            var pManagerLocal_Instance = Singleton_Dynamic_1.instance;

            Assert.AreEqual(_pManagerMember_Instance, Singleton_Dynamic_1.instance);
        }



        #region Monostate

        public class CMonostateBody : IMonoState
        {
            public void IMonoState_OnMakeInstance(out bool bIsGenerateGameObject_Default_Is_False)
            {
                bIsGenerateGameObject_Default_Is_False = true;
            }

            public void IMonoState_OnReleaseInstance()
            {
            }

            public void IMonoState_OnMakeGameObject(GameObject pObject, CMonostate pMono)
            {
            }

            public void IMonoState_OnDestroyGameObject(GameObject pObject)
            {
            }
        }

        private CMonostateWrapper<CMonostateBody> _pMonostateMember_Instance = new CMonostateWrapper<CMonostateBody>();

        [Test]
        public void Monostate는_멤버에서_선언해도_바디는_단일인스턴스입니다()
        {
            CMonostateWrapper<CMonostateBody> _pMonostate_New = new CMonostateWrapper<CMonostateBody>();
            CMonostateBody _pMonostate_Instance = CMonostateWrapper<CMonostateBody>.instance;

            Assert.AreEqual(_pMonostateMember_Instance.Instance, _pMonostate_New.Instance);
            Assert.AreEqual(_pMonostateMember_Instance.Instance, CMonostateWrapper<CMonostateBody>.instance);
        }

        [UnityTest]
        public IEnumerator Monostate는_전용게임오브젝트를_얻을수있습니다()
        {
            CMonostateWrapper<CMonostateBody>.DoReleaseSingleton();
            Assert.IsNull(CMonostateWrapper<CMonostateBody>.gameObject);
            Assert.IsNull(_pMonostateMember_Instance.Instance.gameObjectGetter());

            yield return null;
            yield return null;

            Assert.IsNotNull(CMonostateWrapper<CMonostateBody>.gameObject);
            Assert.IsNotNull(_pMonostateMember_Instance.Instance.gameObjectGetter());
        }

        #endregion
    }
}