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

/// <summary>
/// 유니티 인스펙터에 <see cref="System.FlagsAttribute"/>를 그리기 위한 <see cref="System.Attribute"/>입니다.
/// <para>주의) 잘 모르겠지만 int값이 0에 해당하는 enum을 선언하면 밀립니다</para>
/// </summary>
public class EnumFlagAttribute : PropertyAttribute
{
    public EnumFlagAttribute() { }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagAttribute_Inspector : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginChangeCheck();
        {
            int iNewValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
            if (EditorGUI.EndChangeCheck())
                _property.intValue = iNewValue;
        }
    }
}
#endif