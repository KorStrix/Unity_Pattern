#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-13 오후 4:37:18
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public static class UnityExtension
{
    /// <summary>
    /// 오브젝트가 파괴되었는지 null인지 확실하게 체크, 비용이 무겁습니다
    /// 참고한 링크
    /// https://answers.unity.com/questions/13840/how-to-detect-if-a-gameobject-has-been-destroyed.html
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static bool IsNull(this GameObject gameObject)
    {
        return gameObject == null || ReferenceEquals(gameObject, null);
    }

    /// <summary>
    /// 오브젝트가 파괴되었는지 null인지 확실하게 체크, 비용이 무겁습니다
    /// </summary>
    /// <param name="pComponent"></param>
    /// <returns></returns>
    public static bool IsNull(this Component pComponent)
    {
        return pComponent == null || ReferenceEquals(pComponent, null) || pComponent.gameObject.IsNull();
    }
}