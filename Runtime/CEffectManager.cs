#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-11-06 오후 5:38:49
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectHandle
{
    public GameObject pObjectEffect { get; private set; }

}


/// <summary>
/// 이펙트 매니져
/// </summary>
public class CEffectManager : CSingletonDynamicMonoBase<CEffectManager>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class EffectWrapper
    {
        public string strEffectName;
    }

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    static Dictionary<string, EffectWrapper> _mapEffect = new Dictionary<string, EffectWrapper>();

    System.Func<IEnumerator> _OnRequireInstance;

    static MonoBehaviour _pMono_CoroutineExecuter;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public void DoInit(MonoBehaviour pMono_CoroutineExecuter, System.Func<IEnumerator> OnRequire_Instance)
    {
        _pMono_CoroutineExecuter = pMono_CoroutineExecuter;
        
        // pMono_CoroutineExecuter.StartCoroutine(CoWaitFor_CreateInstance(OnRequire_Instance()));
    }

    static public GameObject DoPlayEffect(string strEffectName, Vector3 vecPlayPos)
    {
        GameObject pObjectEffect = null;
        if(_mapEffect.ContainsKey(strEffectName) == false)
        {

        }

        _pMono_CoroutineExecuter.StartCoroutine(CoPlayEffect(strEffectName));

        return pObjectEffect;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private static IEnumerator CoWaitFor_CreateInstance()
    {
        yield break;
    }

    private static IEnumerator CoPlayEffect(string strEffectName)
    {
        yield break;
    }

    #endregion Private
}