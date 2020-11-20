#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-23 오후 4:29:13
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface ILogic_Check_IsTarget
{
    bool Check_IsTarget(ITargetAble pTarget);
}

public static class ILogicHelper
{
    public static bool Check_IsTarget(this IEnumerable<ILogic_Check_IsTarget> listLogic, ITargetAble pTarget)
    {
        foreach (var pLogic in listLogic)
        {
            if (pLogic.Check_IsTarget(pTarget) == false)
                return false;
        }

        return true;
    }
}


public class Check_IsTarget_IsEnemy : ILogic_Check_IsTarget
{
    ETargetFlag _eTarget;

    public Check_IsTarget_IsEnemy(ETargetFlag eTarget)
    {
        _eTarget = eTarget;
    }

    public bool Check_IsTarget(ITargetAble pTargetAble)
    {
        return _eTarget.HasFlag_Custom(pTargetAble.eFlag);
    }
}

public class Check_IsTarget_IsThis_TheTarget : ILogic_Check_IsTarget
{
    ITargetAble _pCharacter_Target;

    public Check_IsTarget_IsThis_TheTarget(ITargetAble pTarget)
    {
        _pCharacter_Target = pTarget;
    }

    public bool Check_IsTarget(ITargetAble pTarget)
    {
        return _pCharacter_Target == pTarget;
    }
}


public class Check_IsTarget_IsNotMe : ILogic_Check_IsTarget
{
    ITargetAble pCharacter_Me;

    public Check_IsTarget_IsNotMe(ITargetAble pTarget)
    {
        this.pCharacter_Me = pTarget;
    }

    public bool Check_IsTarget(ITargetAble pTarget)
    {
        return pCharacter_Me != pTarget;
    }
}
