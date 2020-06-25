#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-17 오후 3:51:25
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class Scatter
{
    const float const_MinistDelay = 0.02f;

    public delegate IEnumerator delMovePositionCoroutine<T>(T pObject, Vector3 vecPos_BeforeRandom, Vector3 vecPos_AfterRandom);

    static List<Component> g_listComponent = new List<Component>();

    /// <summary>
    /// 해당 위치에 흩뿌리기
    /// </summary>
    public static Coroutine DoScattterCoroutine<T>(MonoBehaviour pCoroutineExecuter, T[] arrScatterObject, Vector3 vecPosition, float fRange, float fDuration, delMovePositionCoroutine<T> OnScatteredObject_Coroutine, System.Action<T[]> OnFinishScatter)
        where T : Component
    {
        return pCoroutineExecuter.StartCoroutine(ScatterCoroutine(pCoroutineExecuter, arrScatterObject, vecPosition, fRange, fDuration, OnScatteredObject_Coroutine, OnFinishScatter, false));
    }

    /// <summary>
    /// 해당 위치에 흩뿌리기
    /// </summary>
    public static Coroutine DoScattterCoroutine_2D<T>(MonoBehaviour pCoroutineExecuter, T[] arrScatterObject, Vector3 vecPosition, float fRange, float fDuration, delMovePositionCoroutine<T> OnScatteredObject_Coroutine, System.Action<T[]> OnFinishScatter)
        where T : Component
    {
        return pCoroutineExecuter.StartCoroutine(ScatterCoroutine(pCoroutineExecuter, arrScatterObject, vecPosition, fRange, fDuration, OnScatteredObject_Coroutine, OnFinishScatter, true));
    }

    /// <summary>
    /// 해당 위치에 흩뿌리기
    /// </summary>
    public static Coroutine DoScattterCoroutine_2D<T>(MonoBehaviour pCoroutineExecuter, T pScatterObject, Vector3 vecPosition, float fRange, float fDuration, delMovePositionCoroutine<T> OnScatteredObject_Coroutine, System.Action<T[]> OnFinishScatter)
        where T : Component
    {
        return pCoroutineExecuter.StartCoroutine(ScatterCoroutine(pCoroutineExecuter, new T[] { pScatterObject }, vecPosition, fRange, fDuration, OnScatteredObject_Coroutine, OnFinishScatter, true));
    }

    /// <summary>
    /// 현재 위치에서 목적지로 이동 후 완료를 보고
    /// </summary>
    public static Coroutine DoStart_MoveToPosCoroutine_Duration<T>(MonoBehaviour pCoroutineExecuter, T pTarget, Transform pDest, float fDuration, System.Func<T, IEnumerator> OnFinishMoveCoroutine)
        where T : Component
    {
        return pCoroutineExecuter.StartCoroutine(MoveToPosition(pCoroutineExecuter, pTarget, () => pDest.position, CalculateProgress_Duration, fDuration, OnFinishMoveCoroutine, false));
    }


    /// <summary>
    /// 현재 위치에서 목적지로 이동 후 완료를 보고
    /// </summary>
    public static Coroutine DoStart_MoveToPosCoroutine_Duration<T>(MonoBehaviour pCoroutineExecuter, T pTarget, Vector3 vecDestWorldPos, float fDuration, System.Func<T, IEnumerator> OnFinishMoveCoroutine)
        where T : Component
    {
        return pCoroutineExecuter.StartCoroutine(MoveToPosition(pCoroutineExecuter, pTarget, () => { return vecDestWorldPos; }, CalculateProgress_Duration, fDuration, OnFinishMoveCoroutine, false));
    }

    /// <summary>
    /// 현재 위치에서 목적지로 이동 후 완료를 보고
    /// </summary>
    public static Coroutine DoStart_MoveToPosCoroutine_2D_Duration<T>(MonoBehaviour pCoroutineExecuter, T pTarget, Vector2 vecDestWorldPos, float fDuration, System.Func<T, IEnumerator> OnFinishMoveCoroutine)
        where T : Component
    {
        return pCoroutineExecuter.StartCoroutine(MoveToPosition(pCoroutineExecuter, pTarget, () => { return vecDestWorldPos; }, CalculateProgress_Duration, fDuration, OnFinishMoveCoroutine, true));
    }

    /// <summary>
    /// 현재 위치에서 목적지로 이동 후 완료를 보고
    /// </summary>
    public static Coroutine DoStart_MoveToPosCoroutine_2D_Speed<T>(MonoBehaviour pCoroutineExecuter, T pTarget, Vector2 vecDestWorldPos, float fSpeed, System.Func<T, IEnumerator> OnFinishMoveCoroutine)
        where T : Component
    {
        return pCoroutineExecuter.StartCoroutine(MoveToPosition(pCoroutineExecuter, pTarget, () => { return vecDestWorldPos; }, CalculateProgress_Speed, fSpeed, OnFinishMoveCoroutine, true));
    }

    // ========================================================================================================================

    private static IEnumerator ScatterCoroutine<T>(MonoBehaviour pCoroutineExecuter, T[] arrScatterObject, Vector3 vecPos, float fRange, float fDuration, delMovePositionCoroutine<T> OnScatteredObject_Coroutine, System.Action<T[]> OnFinishScatter, bool bUse2D)
        where T : Component
    {
        float fHalfRange = fRange * 0.5f;
        int iCount = arrScatterObject.Length;
        float fDelayAverage = fDuration / iCount;
        if (fDelayAverage < const_MinistDelay)
            fDelayAverage = const_MinistDelay;

        int iSpawnCount_PerFrame = Mathf.Clamp((int)(fDelayAverage * iCount), 1, iCount);
        System.Func<Vector3, float, Vector3> OnGetRandomPos = bUse2D ? (System.Func<Vector3, float, Vector3>)GetRandomPos_2D : GetRandomPos;

        if (OnScatteredObject_Coroutine == null)
            OnScatteredObject_Coroutine = OnScatteredObject_Coroutine_Default;

        for (int i = 0; i < iCount;)
        {
            for(int j = 0; j < iSpawnCount_PerFrame; j++)
            {
                int iIndex = i + j;
                if (iIndex >= iCount)
                    break;

                T pObject = arrScatterObject[iIndex];
                Vector3 vecRandomPos = OnGetRandomPos(vecPos, fHalfRange);
                pCoroutineExecuter.StartCoroutine(OnScatteredObject_Coroutine(pObject, vecPos, vecRandomPos));
            }

            i += iSpawnCount_PerFrame;
            yield return new WaitForSeconds(fDelayAverage);
        }

        OnFinishScatter?.Invoke(arrScatterObject);
    }

    private static IEnumerator MoveToPosition<T>(MonoBehaviour pCoroutineExecuter, T pTarget, System.Func<Vector3> OnGetWorldPos, System.Func<float, float> OnCalculateProgress, float fValue, System.Func<T, IEnumerator> OnFinishMoveCoroutine, bool bUse2D)
        where T : Component
    {
        Transform pTransform = pTarget.transform;
        Vector3 vecStartPosition = pTransform.position;

        float fProgress_0_1 = 0f;
        if(bUse2D)
        {
            float fPosZ = vecStartPosition.z;
            while (fProgress_0_1 < 1f && pTransform != null)
            {
                Vector3 vecPos = Vector2.Lerp(vecStartPosition, OnGetWorldPos(), fProgress_0_1);
                vecPos.z = fPosZ;
                pTransform.position = vecPos;
                fProgress_0_1 += OnCalculateProgress(fValue);

                yield return null;
            }

            if(pTransform)
            {
                Vector3 vecPosFinal = OnGetWorldPos();
                vecPosFinal.z = fPosZ;
                pTransform.position = vecPosFinal;
            }
        }
        else
        {
            while (fProgress_0_1 < 1f && pTransform != null)
            {
                pTransform.position = Vector3.Lerp(vecStartPosition, OnGetWorldPos(), fProgress_0_1);
                fProgress_0_1 += OnCalculateProgress(fValue);

                yield return null;
            }

            if(pTransform)
                pTransform.position = OnGetWorldPos();
        }

        if(OnFinishMoveCoroutine != null)
            yield return OnFinishMoveCoroutine(pTarget);
    }


    // ========================================================================================================================

    static IEnumerator OnScatteredObject_Coroutine_Default<T>(T pObject, Vector3 vecPos, Vector3 vecRandomPos)
        where T : Component
    {
        pObject.transform.position = vecRandomPos;

        yield break;
    }

    private static Vector3 GetRandomPos(Vector3 vecPos, float fHalfRange)
    {
        float fPosition_X = Random.Range(vecPos.x - fHalfRange, vecPos.x + fHalfRange);
        float fPosition_Y = Random.Range(vecPos.y - fHalfRange, vecPos.y + fHalfRange);
        float fPosition_Z = Random.Range(vecPos.z - fHalfRange, vecPos.z + fHalfRange);

        return new Vector3(fPosition_X, fPosition_Y, fPosition_Z);
    }

    private static Vector3 GetRandomPos_2D(Vector3 vecPos, float fHalfRange)
    {
        float fPosition_X = Random.Range(vecPos.x - fHalfRange, vecPos.x + fHalfRange);
        float fPosition_Y = Random.Range(vecPos.y - fHalfRange, vecPos.y + fHalfRange);

        return new Vector3(fPosition_X, fPosition_Y, vecPos.z);
    }

    private static float CalculateProgress_Duration(float fDuration)
    {
        return Time.deltaTime / fDuration;
    }

    private static float CalculateProgress_Speed(float fSpeed)
    {
        return fSpeed * Time.deltaTime;
    }
}