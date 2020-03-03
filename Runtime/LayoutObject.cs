#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-18
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>

#if !UNITY_2018 // ����Ƽ 2018���������� �̰� ������ Playmode�� �� �ش� ������â�� ������ �ʽ��ϴ�. (�۾��ϱ� ����)
    [ExecuteInEditMode]
#endif
    public class LayoutObject : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */
        public enum ECircleOption
        {
            None,
            Rotate_Circle,
            Rotate_Circle_Inverse_Y,
            Rotate_Circle_Inverse_Z,
        }

        public enum ERowColumnOption
        {
            None,
            Row,
            Column
        }

        /* public - Field declaration               */

        [Header("�׻� ����")]
        public bool bSortWhenUpdate = true;
        [Header("��Ȱ��ȭ�� ������Ʈ�� ���ľ���")]
        public bool bIsNotUpdate_DeActiveObject = false;

        [Header("RectTransform�϶� ����� ��������")]
        public bool bCalculate_RectTransformSize = false;

        [Header("������ ������")]
        public Vector3 vecLocalPosOffset = Vector3.zero;

        [Header("�࿭ �ɼ�")]
        public ERowColumnOption eRowColumnOption = ERowColumnOption.None;
        [Header("���� ����")]
        public int iRowCount = 0;
        [Header("���� ����")]
        public int iColumnCount = 0;

        [Header("���� �࿭���� ��")]
        public float fNextRowColumnOffset = 0f;


        [Space(10)]
        [Header("���� �ɼ�")]
        public ECircleOption eCircleOption = ECircleOption.None;
        [Header("������ �� ȸ�� ��")]
        public Vector3 vecRotate_OnCircle;
        [Header("������ �� ��ġ ��")]
        public Vector3 vecPos_OnCircle;

        [Space(10)]
        [Header("�ǹ��� �߾����� ����")]
        public bool bPivotIsCenter = false;

        /* protected & private - Field declaration  */

        private bool _bIsEnable_RowColumnOption { get { return eRowColumnOption != ERowColumnOption.None; } }
        private bool _bIsEnable_RowColumn_IsRow { get { return eRowColumnOption == ERowColumnOption.Row; } }
        private bool _bIsEnable_RowColumn_IsColumn { get { return eRowColumnOption == ERowColumnOption.Column; } }
        private bool _bIsEnable_CircleOption { get { return eCircleOption != ECircleOption.None; } }

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoSetGrid()
        {
            Vector3 vecOffset = Vector3.zero;
            if (bPivotIsCenter)
                vecOffset = ((vecLocalPosOffset * transform.childCount) / 2f) - (vecLocalPosOffset / 2f);

            int iIndex = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform pTransformChild = transform.GetChild(i);
                if (bIsNotUpdate_DeActiveObject && pTransformChild.gameObject.activeInHierarchy == false)
                    continue;

                iIndex++;
                RectTransform pTransformRect = pTransformChild as RectTransform;

                Vector3 CalculatedPosition = (vecLocalPosOffset * iIndex) - vecOffset;
                if (pTransformRect)
                    pTransformRect.localPosition = CalculatedPosition;
                else
                    pTransformChild.localPosition = CalculatedPosition;

                if (_bIsEnable_RowColumnOption)
                {
                    Vector3 vecRowColumnOffset = Calculate_RowColumnPosition(iIndex);
                    if (pTransformRect)
                        pTransformRect.localPosition += vecRowColumnOffset;
                    else
                        pTransformChild.localPosition += vecRowColumnOffset;
                }

                if (_bIsEnable_CircleOption)
                    Calculate_CircleOption(iIndex, pTransformChild, pTransformRect);
            }

            if(bCalculate_RectTransformSize)
                CalculateSizeDelta_OnRectTransform();
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void Update()
        {
            if (bSortWhenUpdate)
                DoSetGrid();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

#region Private

        private void Calculate_CircleOption(int iIndex, Transform pTransformChild, RectTransform pTransformRect)
        {
            pTransformChild.localRotation = Quaternion.Euler(vecRotate_OnCircle * iIndex);
            Vector3 vecCurrentLocalPos = pTransformChild.localPosition;

            vecCurrentLocalPos += pTransformChild.forward * vecPos_OnCircle.z;
            vecCurrentLocalPos += pTransformChild.up * vecPos_OnCircle.y;
            vecCurrentLocalPos += pTransformChild.right * vecPos_OnCircle.x;
            pTransformChild.localPosition = vecCurrentLocalPos;
            if (pTransformRect)
                pTransformRect.localPosition = vecCurrentLocalPos;

            if (eCircleOption == ECircleOption.Rotate_Circle_Inverse_Y)
            {
                Vector3 vecDirection = transform.position - pTransformChild.position;
                pTransformChild.up = vecDirection.normalized;
            }
            else if (eCircleOption == ECircleOption.Rotate_Circle_Inverse_Z)
            {
                Vector3 vecDirection = transform.position - pTransformChild.position;
                pTransformChild.forward = vecDirection.normalized;
            }
        }

        private Vector3 Calculate_RowColumnPosition(int iIndex)
        {
            Vector3 vecRowColumnPosition = Vector3.zero;
            if (_bIsEnable_RowColumn_IsRow)
            {
                vecRowColumnPosition.x = (iIndex / iRowCount) * fNextRowColumnOffset;
                vecRowColumnPosition.y = -(vecLocalPosOffset.y * ((iIndex / iRowCount) * iRowCount));
            }
            else if (_bIsEnable_RowColumn_IsColumn)
            {
                vecRowColumnPosition.x = -(vecLocalPosOffset.x * ((iIndex / iColumnCount) * iColumnCount));
                vecRowColumnPosition.y = (iIndex / iColumnCount) * fNextRowColumnOffset;
            }

            return vecRowColumnPosition;
        }

        private void CalculateSizeDelta_OnRectTransform()
        {
            RectTransform pRectTransform = GetComponent<RectTransform>();
            if (pRectTransform == null)
                return;

            Vector2 vecSizeDelta = pRectTransform.sizeDelta;
            vecSizeDelta = vecLocalPosOffset * transform.childCount;

            pRectTransform.sizeDelta = vecSizeDelta;
        }

#endregion Private
    }
}