#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-11 오전 10:30:58
 *	개요 : 
 *	
 *	원본 소스코드 링크 : https://wergia.tistory.com/104
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class EnumFlagHelper
{
    public static bool ContainEnumFlag<T>(this T eEnumFlag, params T[] arrEnum)
        where T : struct, System.IConvertible, System.IComparable, System.IFormattable
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
}

/// <summary>
/// 유니티 인스펙터에 <see cref="System.FlagsAttribute"/>를 그리기 위한 <see cref="System.Attribute"/>입니다.
/// </summary>
public class UnityEnumFlagAttribute : PropertyAttribute
{
    public UnityEnumFlagAttribute() { }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UnityEnumFlagAttribute))]
public class EnumFlagAttribute_Inspector : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}
#endif