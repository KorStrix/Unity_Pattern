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
    public class PoolingManager_NormalClass<CLASS_POOL_TARGET> : PoolingManagerBase<PoolingManager_NormalClass<CLASS_POOL_TARGET>, CLASS_POOL_TARGET>
        where CLASS_POOL_TARGET : class, new()
    {
        protected override CLASS_POOL_TARGET OnCreateClass_WhenEmptyPool(CLASS_POOL_TARGET pObjectCopyTarget, int iID)
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

        public class DictionaryComparer<T> : EqualityComparer<T>
        {
            public override bool Equals(T x, T y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        /* public - Field declaration            */

        public bool p_bIsDebug = false;

        public int p_iUseCount => _setUsedObject.Count;
        public int p_iInstanceCount => _mapAllInstance.Count;

        public CLASS_POOL_TARGET[] arrAllObject => _mapAllInstance.Keys.ToArray();
        public CLASS_POOL_TARGET[] arrUsedObject => _setUsedObject.ToArray();

        /* protected & private - Field declaration         */

        protected Dictionary<CLASS_POOL_TARGET, int> _mapAllInstance = new Dictionary<CLASS_POOL_TARGET, int>();

        // 본래 LinkedList를 사용했으나, C#에선 LinkedList가 오히려 더 느리다..
        // https://stackoverflow.com/questions/5983059/why-is-a-linkedlist-generally-slower-than-a-list

        protected Dictionary<int, HashSet<CLASS_POOL_TARGET>> _mapUsed = new Dictionary<int, HashSet<CLASS_POOL_TARGET>>(new DictionaryComparer<int>());
        protected Dictionary<int, List<CLASS_POOL_TARGET>> _mapUnUsed = new Dictionary<int, List<CLASS_POOL_TARGET>>(new DictionaryComparer<int>());

        HashSet<CLASS_POOL_TARGET> _setUsedObject = new HashSet<CLASS_POOL_TARGET>();

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoPrePooling(CLASS_POOL_TARGET pObjectCopyTarget, int iCount)
        {
            if (pObjectCopyTarget == null)
                return;

            int iID = pObjectCopyTarget.GetHashCode();
            Add_NewObjectType(pObjectCopyTarget, iID);

            int iTotalCount = _mapUnUsed[iID].Count + _mapUnUsed[iID].Count;
            if (iTotalCount > iCount)
                return;

            LinkedList<CLASS_POOL_TARGET> listTemp = new LinkedList<CLASS_POOL_TARGET>();
            int iPoolingCount = iCount - iTotalCount;
            for (int i = 0; i < iPoolingCount; i++)
                listTemp.AddLast(DoPop(pObjectCopyTarget));

            foreach (var pPrePoolingObject in listTemp)
                DoPush(pPrePoolingObject);
        }

        public CLASS_POOL_TARGET DoPop(CLASS_POOL_TARGET pObjectCopyTarget)
        {
            if (pObjectCopyTarget == null)
                return null;

            int iID = pObjectCopyTarget.GetHashCode();
            Add_NewObjectType(pObjectCopyTarget, iID);

            CLASS_POOL_TARGET pUnUsed = Get_UnusedObject(pObjectCopyTarget, iID);
            if (p_bIsDebug)
                Debug.Log("Pooling Simple Pop - " + pUnUsed.ToString());

            OnPopObject(pUnUsed);

            return pUnUsed;
        }

        public void DoPush(GameObject pObjectReturn)
        {
            DoPush(pObjectReturn.GetComponent<CLASS_POOL_TARGET>());
        }

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

        public void DoPushAll()
        {
            foreach (var pObject in _mapAllInstance.Keys)
                DoPush(pObject);
        }

        virtual public void DoDestroyAll()
        {
            _mapAllInstance.Clear();
            _mapUsed.Clear();
            _setUsedObject.Clear();
            _mapUnUsed.Clear();
        }

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

        protected override void OnMakeSingleton(out bool bIsGenearteGameObject_Default_Is_False)
        {
            base.OnMakeSingleton(out bIsGenearteGameObject_Default_Is_False);

            bIsGenearteGameObject_Default_Is_False = true;
            strTypeName = typeof(CLASS_POOL_TARGET).Name;
        }

        protected override void OnMakeGameObject(GameObject pObject, CSingletonNotMono pMono)
        {
            base.OnMakeGameObject(pObject, pMono);

#if UNITY_EDITOR // 하이어라키뷰에 실시간 풀링 상황 모니터링을 위한 Update
            pMono.StartCoroutine(CoUpdate());
#endif
            GameObject.DontDestroyOnLoad(gameObject);
        }

        string strTypeName;

#if UNITY_EDITOR // 하이어라키뷰에 실시간 풀링 상황 모니터링을 위한 Update
        private IEnumerator CoUpdate()
        {
            while (true)
            {
                gameObject.name = string.Format($"풀링<{strTypeName}> -{_setUsedObject.Count}/{_mapAllInstance.Count}개 사용중");

                yield return new WaitForSeconds(1f);
            }
        }
#endif

        /* protected - [abstract & virtual]         */

        abstract protected CLASS_POOL_TARGET OnCreateClass_WhenEmptyPool(CLASS_POOL_TARGET pObjectCopyTarget, int iID);

        virtual protected void OnPopObject(CLASS_POOL_TARGET pClassType) { }
        virtual protected void OnPushObject(CLASS_POOL_TARGET pClassType) { }

        // ========================================================================== //

        #region Private

        private void Add_NewObjectType(CLASS_POOL_TARGET pObjectCopyTarget, int iID)
        {
            if (_mapUnUsed.ContainsKey(iID))
                return;

            _mapUsed.Add(iID, new HashSet<CLASS_POOL_TARGET>());
            _mapUnUsed.Add(iID, new List<CLASS_POOL_TARGET>());
        }

        private CLASS_POOL_TARGET Get_UnusedObject(CLASS_POOL_TARGET pObjectCopyTarget, int iID)
        {
            CLASS_POOL_TARGET pComponentUnUsed;
            if (_mapUnUsed[iID].Count != 0)
            {
                int iIndexLast = _mapUnUsed[iID].Count - 1;
                pComponentUnUsed = _mapUnUsed[iID][iIndexLast];
                _mapUnUsed[iID].RemoveAt(iIndexLast);
            }
            else
            {
                pComponentUnUsed = OnCreateClass_WhenEmptyPool(pObjectCopyTarget, iID);
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

                if (_mapUnUsed.ContainsKey(iID))
                    _mapUnUsed[iID].Add(pObjectReturn);
            }
        }

        #endregion Private
    }
}