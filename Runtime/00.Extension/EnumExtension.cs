#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-13 오후 4:37:18
 *	개요 : 
   ============================================ */
#endregion Header

using System.Linq;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public static class EnumExtension
{
    public static TEnum ConvertEnum<TEnum>(this string strText)
        where TEnum : struct
    {
        if (System.Enum.TryParse(strText, out TEnum pEnum) == false)
        {
            Debug.LogError($"Enum Parsing Fail - ({pEnum.GetType()}){strText}");
        }

        return pEnum;
    }

    public static bool TryConvertEnum<TEnum>(this string strText, out TEnum pEnum)
        where TEnum : struct
    {
        return System.Enum.TryParse(strText, out pEnum);
    }

    public static bool ContainEnumFlag<TEnum>(this TEnum eEnumFlag, params TEnum[] arrEnum)
        where TEnum : struct, System.IConvertible, System.IComparable, System.IFormattable
    {
        bool bIsContain = false;

        int iEnumFlag = eEnumFlag.GetHashCode();
        foreach (var pEnum in arrEnum)
        {
            int iEnum = pEnum.GetHashCode();
            bIsContain = (iEnumFlag & iEnum) != 0;
            if (bIsContain)
                break;
        }

        return bIsContain;
    }

    public static TEnum GetPrevEnum<TEnum>(this TEnum eEnum, System.Action<string> OnError = null)
        where TEnum : struct, System.IConvertible, System.IComparable, System.IFormattable
    {
        var arrEnumValues = System.Enum.GetValues(eEnum.GetType()).OfType<TEnum>().ToArray();

        int iFindIndex = -1;
        for (int i = 0; i < arrEnumValues.Length; i++)
        {
            if (arrEnumValues[i].Equals(eEnum))
            {
                iFindIndex = i - 1;
                break;
            }
        }

        if (iFindIndex < 0)
        {
            OnError?.Invoke($"{eEnum} {nameof(GetPrevEnum)} Not founded");
            return eEnum;
        }

        return arrEnumValues[iFindIndex];
    }
}
