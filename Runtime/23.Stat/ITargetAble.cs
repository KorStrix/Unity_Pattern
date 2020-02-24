#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-23 오후 4:31:16
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Flags]
public enum ETargetFlag
{
    None = 0,

    Allliance = 1 << 0,
    Monster = 1 << 1,

    Prop = 1 << 2,
}

public interface ITargetAble
{
    ETargetFlag eFlag { get; }
}
