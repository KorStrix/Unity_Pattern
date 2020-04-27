#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-15 오후 5:39:53
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

/// <summary>
/// <see cref="Enum"/>에 별도로 <see cref="string"/>을 추가하기 위한 <see cref="Attribute"/>입니다.
/// <para>사용 예시</para>
/// <para>enum Somthing {</para>
/// <para>[<see cref="RegistSubStringAttribute"/>("Custom String")]</para>
/// <para>Somthing</para>
/// <para>}</para>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class RegistSubStringAttribute : UnityEngine.PropertyAttribute
{
    public string strSubString;

    public RegistSubStringAttribute(string strSubString)
    {
        this.strSubString = strSubString;
    }

    public RegistSubStringAttribute(object pObject)
    {
        this.strSubString = pObject.ToString();
    }
}

public static class SCSubStringHelper
{
    public static string ToStringSub(this System.Enum eEnum)
    {
        string strString = eEnum.ToString();
        Type pType = eEnum.GetType();
        FieldInfo pFieldInfo = pType.GetField(strString);
        if(pFieldInfo != null)
        {
            RegistSubStringAttribute pAttribute = pFieldInfo.GetCustomAttribute(typeof(RegistSubStringAttribute), false) as RegistSubStringAttribute;
            if(pAttribute != null)
                strString = pAttribute.strSubString;
        }

        return strString;
    }

    public static string ToStringSub(this object pClass)
    {
        string strString = pClass.ToString();
        RegistSubStringAttribute pAttribute = pClass.GetType().GetCustomAttribute(typeof(RegistSubStringAttribute), false) as RegistSubStringAttribute;
        if (pAttribute != null)
            strString = pAttribute.strSubString;

        return strString;
    }
}
