#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-13 오후 4:37:18
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 
/// </summary>
public static class UnityExtension
{
    public static void DoDelayInvoke(this MonoBehaviour pMono, System.Action Function, float fDelay)
    {
        pMono.StartCoroutine(OnDelayInvoke(Function, fDelay));
    }

    public static Coroutine StartCoroutine_OnlyOnePlay(this MonoBehaviour pMono, IEnumerator pCoroutineFunc)
    {
        // 잘 될지 모르겠다; 테스트 필요
        pMono.StopCoroutine(pCoroutineFunc);
        return pMono.StartCoroutine(pCoroutineFunc);
    }

    static IEnumerator OnDelayInvoke(System.Action Function, float fDelay)
    {
        yield return new WaitForSeconds(fDelay);

        Function?.Invoke();
    }


    /// <summary>
    /// 오브젝트가 파괴되었는지 null인지 확실하게 체크, 비용이 무겁습니다
    /// 참고한 링크
    /// https://answers.unity.com/questions/13840/how-to-detect-if-a-gameobject-has-been-destroyed.html
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static bool IsNull(this GameObject gameObject)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        return gameObject == null || ReferenceEquals(gameObject, null);
    }

    /// <summary>
    /// 오브젝트가 파괴되었는지 null인지 확실하게 체크, 비용이 무겁습니다
    /// </summary>
    /// <param name="pComponent"></param>
    /// <returns></returns>
    public static bool IsNullComponent(this Component pComponent)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        return pComponent == null || ReferenceEquals(pComponent, null) || pComponent.gameObject.IsNull();
    }

    public static T Get_Or_AddComponent<T>(this GameObject pObject)
        where T : Component
    {
        T pComponentReturn = pObject.GetComponent<T>();
        if (pComponentReturn == null)
            pComponentReturn = pObject.AddComponent<T>();

        return pComponentReturn;
    }


    public static T Get_Or_AddComponent<T>(this Component pComponent)
        where T : Component
    {
        T pComponentReturn = pComponent.GetComponent<T>();
        if (pComponentReturn == null)
            pComponentReturn = pComponent.gameObject.AddComponent<T>();

        return pComponentReturn;
    }

    public static void SetActive(this Component pComponent, bool bActive)
    {
        // Debug.Log($"{pComponent.name}.{nameof(SetActive)}({bActive})", pComponent);

        if (pComponent.IsNullComponent())
        {
            Debug.LogError($"{nameof(SetActive)} pComponent == null");
            return;
        }

        pComponent.gameObject.SetActive(bActive);
    }

    public static void SetLayer(this Component pComponent, int iLayer, bool bRecursive = true)
    {
        pComponent.gameObject.layer = iLayer;
        if (bRecursive)
            pComponent.gameObject.GetComponentsInChildren<Transform>().
                Where(p => p.gameObject != pComponent.gameObject).
                ForEachCustom(p => p.SetLayer(iLayer));
    }

    
    public static Coroutine CombineCoroutine(this Coroutine pCoroutine, MonoBehaviour pCoroutineExecuter, YieldInstruction OnNextCoroutine)
    {
        return pCoroutineExecuter.StartCoroutine(WaitCoroutine(pCoroutine, OnNextCoroutine));
    }

    static IEnumerator WaitCoroutine(Coroutine pCoroutine, YieldInstruction OnNextCoroutine)
    {
        yield return pCoroutine;
        yield return OnNextCoroutine;
    }

    public static Coroutine OnFinish(this Coroutine pCoroutine, MonoBehaviour pCoroutineExecuter, System.Action OnFinish)
    {
        return pCoroutineExecuter.StartCoroutine(WaitCoroutine(pCoroutine, OnFinish));
    }

    public static Coroutine OnFinish(this Coroutine pCoroutine, MonoBehaviour pCoroutineExecuter, IEnumerator OnFinishCoroutine)
    {
        return pCoroutineExecuter.StartCoroutine(WaitCoroutine(pCoroutine, OnFinishCoroutine));
    }

    static IEnumerator WaitCoroutine(Coroutine pCoroutine, System.Action OnFinish)
    {
        yield return pCoroutine;

        OnFinish?.Invoke();
    }

    static IEnumerator WaitCoroutine(Coroutine pCoroutine, IEnumerator OnFinishCoroutine)
    {
        yield return pCoroutine;

        yield return OnFinishCoroutine;
    }

}