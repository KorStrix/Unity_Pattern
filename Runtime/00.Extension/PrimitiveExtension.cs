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
using System.Text;
using System.Linq;

/// <summary>
/// 
/// </summary>
public static class PrimitiveExtension
{
    static public float Normailize(this float fCurrent_0_1, float fMax, float fMin = 0f)
    {
        return (fCurrent_0_1 * (fMax - fMin)) + fMin;
    }

    static public T ConvertEnum<T>(this string strText)
        where T : struct
    {
        T pEnum;
        if(System.Enum.TryParse(strText, out pEnum) == false)
        {
            Debug.LogError($"Enum Parsing Fail - ({pEnum.GetType()}){strText}");
        }

        return pEnum;
    }

    static public bool TryConvertEnum<T>(this string strText, out T pEnum)
        where T : struct
    {
        return System.Enum.TryParse(strText, out pEnum);
    }

    static public bool TryConvertVector(this string strText, out Vector3 vecRot, char chSplit = ',')
    {
        vecRot = Vector3.zero;
        if (string.IsNullOrEmpty(strText))
            return false;

        string[] arrString = strText.Split(chSplit);
        try
        {
            vecRot.x = float.Parse(arrString[0]);
            vecRot.y = float.Parse(arrString[1]);
            vecRot.z = float.Parse(arrString[2]);
        }
        catch
        {
            return false;
        }

        return true;
    }

}
