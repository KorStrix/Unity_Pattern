#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-12-21 오전 10:13:20
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
    /// <summary>
    /// <see cref="UnityEngine.Component"/>를 상속받지 않는 NormalClass용 제네릭 풀링 매니져.
    /// <para>사용 방법</para>
    /// <para>풀링에 요청 : <see cref="PoolingManager_NormalClass.DoPop(class pOriginalObject)"/></para>
    /// <para>풀링에 리턴 : 매니져에 관계없이 해당 오브젝트 Disable때 자동 리턴</para>
    /// </summary>
    /// <typeparam name="CLASS_POOL_TARGET">풀링 리턴받을 타입</typeparam>
    public class PoolingManager_NormalClass<CLASS_POOL_TARGET> : PoolingManagerBase<PoolingManager_NormalClass<CLASS_POOL_TARGET>, CLASS_POOL_TARGET>
        where CLASS_POOL_TARGET : class, new()
    {
        protected override CLASS_POOL_TARGET OnCreateClass_When_EmptyPool(CLASS_POOL_TARGET pObjectCopyTarget, int iID)
        {
            return new CLASS_POOL_TARGET();
        }
    }

    /// <summary>
    /// 풀링의 추상 베이스 클래스, 기본적인 풀링 로직이 들어있습니다.
    /// <para>상속받아 사용.</para>
    /// </summary>
    /// <typeparam name="CLASS_DERIVED"></typeparam>
    /// <typeparam name="CLASS_POOL_TARGET"></typeparam>
    public abstract class PoolingManagerBase<CLASS_DERIVED, CLASS_POOL_TARGET> : CSingletonNotMonoBase<CLASS_DERIVED>
        where CLASS_DERIVED : PoolingManagerBase<CLASS_DERIVED, CLASS_POOL_TARGET>, new()
        where CLASS_POOL_TARGET : class
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public class DictionaryComparer : EqualityComparer<int>
        {
            public override bool Equals(int x, int y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(int obj)
            {
                return obj.GetHashCode();
            }
        }

        /* public - Field declaration            */

        public bool p_bIsDebug = false;

        /// <summary>
        /// 현재 풀에서 사용되고 있는 인스턴스 개수
        /// </summary>
        public int iUseCount => _setUsedObject.Count;

        /// <summary>
        /// 현재 풀에서 관리하는 모든 인스턴스 개수
        /// </summary>
        public int iInstanceCount => _mapAllInstance.Count;

        /// <summary>
        /// 풀에서 관리하는 모든 오브젝트
        /// </summary>
        public CLASS_POOL_TARGET[] arrAllObject => _mapAllInstance.Keys.ToArray();

        /// <summary>
        /// 풀에서 사용되고 있는 모든 오브젝트
        /// </summary>
        public CLASS_POOL_TARGET[] arrUsedObject => _setUsedObject.ToArray();

        /* protected & private - Field declaration         */

        protected Dictionary<CLASS_POOL_TARGET, int> _mapAllInstance = new Dictionary<CLASS_POOL_TARGET, int>();

        // 본래 LinkedList를 사용했으나, C#에선 LinkedList가 오히려 더 느리다.
        // -> Hashset으로 변경되면서 현재 사용하지 않지만, 그냥 교양용으로 남겨둠
        // https://stackoverflow.com/questions/5983059/why-is-a-linkedlist-generally-slower-than-a-list

        // Dictionary에서 Key를 Struct로 하면 GC가 쌓입니다
        // https://libsora.so/posts/csharp-dictionary-enum-key-without-gc/

        protected Dictionary<int, HashSet<CLASS_POOL_TARGET>> _mapUsed = new Dictionary<int, HashSet<CLASS_POOL_TARGET>>(new DictionaryComparer());
        protected Dictionary<int, HashSet<CLASS_POOL_TARGET>> _mapUnUsed = new Dictionary<int, HashSet<CLASS_POOL_TARGET>>(new DictionaryComparer());

        HashSet<CLASS_POOL_TARGET> _setUsedObject = new HashSet<CLASS_POOL_TARGET>();

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 지정된 갯수만큼 풀에 미리 생성해놓습니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">미리 생성할 원본 오브젝트</param>
        /// <param name="iCount">미리 생성할 갯수</param>
        public void DoPrePooling(CLASS_POOL_TARGET pObjectCopyTarget, int iCount)
        {
            if (pObjectCopyTarget == null)
                return;

            int iID = pObjectCopyTarget.GetHashCode();
            Add_NewObjectID(iID);

            int iTotalCount = _mapUnUsed[iID].Count + _mapUnUsed[iID].Count;
            //if (iTotalCount > iCount)
            //    return;

            LinkedList<CLASS_POOL_TARGET> listTemp = new LinkedList<CLASS_POOL_TARGET>();
            int iPoolingCount = iCount - iTotalCount;
            for (int i = 0; i < iPoolingCount; i++)
                listTemp.AddLast(DoPop(pObjectCopyTarget));

            foreach (var pPrePoolingObject in listTemp)
                DoPush(pPrePoolingObject);
        }

        /// <summary>
        /// 이미 생성된 오브젝트를 풀에 넣습니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">미리 생성할 원본 오브젝트</param>
        public void DoAdd_PoolObject(CLASS_POOL_TARGET pObjectCopyTarget, CLASS_POOL_TARGET pPoolingObject)
        {
            if (pObjectCopyTarget == null)
                return;

            int iID = pObjectCopyTarget.GetHashCode();
            Add_NewObjectID(iID);

            if (_mapAllInstance.ContainsKey(pPoolingObject))
                return;

            OnInitClass(pObjectCopyTarget, pPoolingObject, iID);
            _mapAllInstance.Add(pPoolingObject, iID);

            DoPush(pPoolingObject);
        }

        /// <summary>
        /// 이미 생성된 오브젝트를 풀에 넣습니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">미리 생성할 원본 오브젝트</param>
        public void DoAdd_PoolObject(CLASS_POOL_TARGET pObjectCopyTarget, IEnumerable<CLASS_POOL_TARGET> arrPoolingObject)
        {
            if (pObjectCopyTarget == null)
                return;

            int iID = pObjectCopyTarget.GetHashCode();
            Add_NewObjectID(iID);

            foreach (var pObject in arrPoolingObject)
            {
                if (_mapAllInstance.ContainsKey(pObject))
                    continue;

                OnInitClass(pObjectCopyTarget, pObject, iID);
                _mapAllInstance.Add(pObject, iID);

                DoPush(pObject);
            }
        }

        /// <summary>
        /// 풀에서 원본을 인자로 카피본을 리턴받습니다.
        /// </summary>
        /// <param name="pObjectCopyTarget">카피할 원본</param>
        public CLASS_POOL_TARGET DoPop(CLASS_POOL_TARGET pObjectCopyTarget)
        {
            if (pObjectCopyTarget == null)
                return null;

            int iID = pObjectCopyTarget.GetHashCode();
            Add_NewObjectID(iID);

            CLASS_POOL_TARGET pUnUsed = Get_UnusedObject(pObjectCopyTarget, iID);
            if (p_bIsDebug)
                Debug.Log("Pooling Simple Pop - " + pUnUsed);

            OnPopObject(pUnUsed);

            return pUnUsed;
        }

        /// <summary>
        /// 사용 후 풀에 반환합니다.
        /// </summary>
        /// <param name="pObjectReturn">리턴할 오브젝트</param>
        public void DoPush(GameObject pObjectReturn)
        {
            DoPush(pObjectReturn.GetComponent<CLASS_POOL_TARGET>());
        }

        /// <summary>
        /// 사용 후 풀에 반환합니다.
        /// </summary>
        /// <param name="pClassType">리턴할 오브젝트</param>
        public void DoPush(CLASS_POOL_TARGET pClassType)
        {
            if (pClassType == null)
                return;

            if (_mapAllInstance.ContainsKey(pClassType) == false)
                return;

            Remove_UsedList(pClassType);

            if (p_bIsDebug)
                Debug.Log("Pooling Simple Pushed - " + pClassType, gameObject);

            OnPushObject(pClassType);
        }

        /// <summary>
        /// 모든 오브젝트를 리턴합니다.
        /// </summary>
        public void DoPushAll()
        {
            foreach (var pObject in _mapAllInstance.Keys)
                DoPush(pObject);
        }

        /// <summary>
        /// 모든 오브젝트를 파괴합니다.
        /// </summary>
        public virtual void DoDestroyAll()
        {
            _mapAllInstance.Clear();
            _mapUsed.Clear();
            _setUsedObject.Clear();
            _mapUnUsed.Clear();
        }

        /// <summary>
        /// 풀의 관리대상에서 제외됩니다.
        /// </summary>
        /// <param name="pObjectDestroyed">관리대상에서 제외할 오브젝트</param>
        public void Event_RemovePoolObject(GameObject pObjectDestroyed)
        {
            if (pObjectDestroyed == null)
                return;

            CLASS_POOL_TARGET pObjectReturn = pObjectDestroyed.GetComponent<CLASS_POOL_TARGET>();
            if (pObjectReturn == null || _mapAllInstance.ContainsKey(pObjectReturn) == false)
                return;

            int iID = _mapAllInstance[pObjectReturn];
            if (_mapUsed.ContainsKey(iID) && _mapUsed[iID].Contains(pObjectReturn))
            {
                _mapUsed[iID].Remove(pObjectReturn);
                _setUsedObject.Remove(pObjectReturn);
            }

            if (_mapUnUsed.ContainsKey(iID) && _mapUnUsed[iID].Contains(pObjectReturn))
                _mapUnUsed[iID].Remove(pObjectReturn);

            _mapAllInstance.Remove(pObjectReturn);
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnMakeSingleton(out bool bIsGenerateGameObject_Default_Is_False)
        {
            base.OnMakeSingleton(out bIsGenerateGameObject_Default_Is_False);

            bIsGenerateGameObject_Default_Is_False = true;
            _strTypeName = typeof(CLASS_POOL_TARGET).Name;
        }

        protected override void OnMakeGameObject(GameObject pObject, CSingletonNotMono pMono)
        {
            base.OnMakeGameObject(pObject, pMono);

#if UNITY_EDITOR // 하이어라키뷰에 실시간 풀링 상황 모니터링을 위한 Update
            pMono.StartCoroutine(CoUpdate());
#endif
            GameObject.DontDestroyOnLoad(gameObject);
        }

        string _strTypeName;

#if UNITY_EDITOR // 하이어라키뷰에 실시간 풀링 상황 모니터링을 위한 Update
        private IEnumerator CoUpdate()
        {
            while (true)
            {
                gameObject.name = string.Format($"풀링<{_strTypeName}> -{_setUsedObject.Count}/{_mapAllInstance.Count}개 사용중");

                yield return new WaitForSeconds(1f);
            }
        }
#endif

        /* protected - [abstract & virtual]         */

        /// <summary>
        /// 풀에 없을 때 풀 타겟 클래스를 만드는 함수
        /// </summary>
        /// <param name="pObjectCopyTarget"></param>
        /// <param name="iID"></param>
        /// <returns></returns>
        protected abstract CLASS_POOL_TARGET OnCreateClass_When_EmptyPool(CLASS_POOL_TARGET pObjectCopyTarget, int iID);

        /// <summary>
        /// 풀에 처음 넣을 때 Init함수
        /// </summary>
        /// <param name="pObjectCopyTarget"></param>
        /// <param name="iID"></param>
        protected virtual void OnInitClass(CLASS_POOL_TARGET pObject_Original, CLASS_POOL_TARGET pObject_InPool, int iID) { }

        protected virtual void OnPopObject(CLASS_POOL_TARGET pClassType) { }
        protected virtual void OnPushObject(CLASS_POOL_TARGET pClassType) { }

        // ========================================================================== //

        #region Private

        private void Add_NewObjectID(int iID)
        {
            if (_mapUnUsed.ContainsKey(iID))
                return;

            _mapUsed.Add(iID, new HashSet<CLASS_POOL_TARGET>());
            _mapUnUsed.Add(iID, new HashSet<CLASS_POOL_TARGET>());
        }

        private CLASS_POOL_TARGET Get_UnusedObject(CLASS_POOL_TARGET pObjectCopyTarget, int iID)
        {
            CLASS_POOL_TARGET pComponentUnUsed;
            if (_mapUnUsed[iID].Count != 0)
            {
                pComponentUnUsed = _mapUnUsed[iID].First();
                _mapUnUsed[iID].Remove(pComponentUnUsed);
            }
            else
            {
                pComponentUnUsed = OnCreateClass_When_EmptyPool(pObjectCopyTarget, iID);
                OnInitClass(pObjectCopyTarget, pComponentUnUsed, iID);
                _mapAllInstance.Add(pComponentUnUsed, iID);
            }

            _mapUsed[iID].Add(pComponentUnUsed);
            _setUsedObject.Add(pComponentUnUsed);

            return pComponentUnUsed;
        }

        private void Remove_UsedList(CLASS_POOL_TARGET pObjectReturn)
        {
            int iID = _mapAllInstance[pObjectReturn];
            if (_mapUsed.ContainsKey(iID) && _mapUsed[iID].Contains(pObjectReturn))
            {
                _mapUsed[iID].Remove(pObjectReturn);
                _setUsedObject.Remove(pObjectReturn);
            }

            if (_mapUnUsed.ContainsKey(iID))
                _mapUnUsed[iID].Add(pObjectReturn);
        }

        #endregion Private
    }
}
