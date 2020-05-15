#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-06 오후 4:59:56
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CObjectBase : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public bool bIsExecute_Awake { get; protected set; } = false;

        /* protected & private - Field declaration         */
        protected static bool _bIsQuit_Application { get; private set; } = false;
        protected static bool _bIsEditor_Compiling { get; private set; } = false;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            _bIsEditor_Compiling = true;
            var arrObject = FindObjectsOfType<CObjectBase>();
            if (arrObject.Length != 0)
            {
                Debug.Log($"{nameof(CObjectBase)} OnScriptsReloaded Listen Count : {arrObject.Length}");
                arrObject.ForEachCustom(p => p.OnEditorCompile());
            }

            _bIsEditor_Compiling = false;
        }
#endif

        public void DoAwake()
        {
            Awake();
        }

        public void DoAwake_Force()
        {
            bIsExecute_Awake = false;
            Awake();
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected void Awake()
        {
            if (bIsExecute_Awake)
                return;
            bIsExecute_Awake = true;

            OnAwake();
        }

        protected void OnEnable()
        {
            OnEnableObject();
        }

        private void OnDisable()
        {
            OnDisableObject(_bIsQuit_Application);
        }

        private void OnApplicationQuit()
        {
            _bIsQuit_Application = true;
        }

        private void OnDestroy()
        {
            OnDestroyObject(_bIsQuit_Application);
        }

        
        /* protected - [abstract & virtual]         */

        protected virtual void OnAwake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
#endif

            if(gameObject.activeInHierarchy)
            {
                StopCoroutine(nameof(OnAwakeCoroutine));
                StartCoroutine(nameof(OnAwakeCoroutine));
            }
        }

        protected virtual void OnEnableObject()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
#endif

            if (gameObject.activeInHierarchy)
            {
                StopCoroutine(nameof(OnEnableCoroutine));
                StartCoroutine(nameof(OnEnableCoroutine));
            }
        }

        protected virtual void OnDisableObject(bool bIsQuit_Application) { }
        protected virtual void OnDestroyObject(bool bIsQuit_Application) { }

        protected virtual IEnumerator OnAwakeCoroutine() { yield break; }
        protected virtual IEnumerator OnEnableCoroutine() { yield break; }


        protected virtual void OnEditorCompile()
        {
        }

        // ========================================================================== //

        #region Private

        #endregion Private
    }
}