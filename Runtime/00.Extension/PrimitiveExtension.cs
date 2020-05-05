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
    public static int Calculate_LevelRateValue(int iInitValue, float fMagnificationRate, int iCurrentLevel)
    {
        return (int)(iInitValue * Mathf.Pow(fMagnificationRate, iCurrentLevel - 1));
    }

    public static float Calculate_LevelRateValue(float fInitValue, float fMagnificationRate, float fCurrentLevel)
    {
        return fInitValue * Mathf.Pow(fMagnificationRate, fCurrentLevel - 1);
    }

    
    public static float Normailize(this float fCurrent_0_1, float fMax, float fMin = 0f)
    {
        return (fCurrent_0_1 * (fMax - fMin)) + fMin;
    }

    public static T ConvertEnum<T>(this string strText)
        where T : struct
    {
        T pEnum;
        if(System.Enum.TryParse(strText, out pEnum) == false)
        {
            Debug.LogError($"Enum Parsing Fail - ({pEnum.GetType()}){strText}");
        }

        return pEnum;
    }

    public static bool TryConvertEnum<T>(this string strText, out T pEnum)
        where T : struct
    {
        return System.Enum.TryParse(strText, out pEnum);
    }

    public static bool TryConvertVector(this string strText, out Vector3 vecRot, char chSplit = ',')
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

    public static string CutLength(this string strText, int iCutLength)
    {
        if (string.IsNullOrEmpty(strText))
            return strText;

        int iStringLength = strText.Length;
        iCutLength = Mathf.Clamp(iCutLength, 0, iStringLength);

        return strText.Substring(0, iStringLength - iCutLength);
    }
}
