#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-31
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Unity_Pattern
{
    /// <summary>
    /// Ʈ�������� ���� ����ȭ �����ִ� ��ü
    /// </summary>
    public class TransformFollower : CObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        /// <summary>
        /// ���� ��� Ʈ������
        /// </summary>
        public Transform pTransformTarget;

        /// <summary>
        /// X �������� ������?
        /// </summary>
        public bool bIsFollow_PosX;
        public bool bIsFollow_PosY;
        public bool bIsFollow_PosZ;

        /// <summary>
        /// �������� �󸶳� ������ �ΰ� ������
        /// ������� 1, 0, 0���� �ϸ� �׻� 1, 0, 0 ��ŭ ������ ������ ���󰩴ϴ�.
        /// </summary>
        public Vector3 vecPosOffset;

        public bool bIsFollow_RotX;
        public bool bIsFollow_RotY;
        public bool bIsFollow_RotZ;

        public Vector3 vecRotOffset;

        /* protected & private - Field declaration  */

        /// <summary>
        /// �� Ʈ������
        /// </summary>
        Transform _pTransform;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pTransform = transform;
        }

        private void Update()
        {
            if (pTransformTarget == null)
                return;

            Sync_Position();
            Sync_Rotation();
        }

        /// <summary>
        /// ��ġ�� ����ȭ�մϴ�.
        /// </summary>
        private void Sync_Position()
        {
            Vector3 vecCurrentPos = transform.position;
            Vector3 vecTargetPos = pTransformTarget.position + vecPosOffset;
            if (bIsFollow_PosX)
                vecCurrentPos.x = vecTargetPos.x;

            if (bIsFollow_PosY)
                vecCurrentPos.y = vecTargetPos.y;

            if (bIsFollow_PosZ)
                vecCurrentPos.z = vecTargetPos.z;

            transform.position = vecCurrentPos;
        }

        /// <summary>
        /// ȸ������ ����ȭ�մϴ�.
        /// </summary>
        private void Sync_Rotation()
        {
            Vector3 vecCurrentRot = transform.rotation.eulerAngles;
            Vector3 vecTargetRot = pTransformTarget.rotation.eulerAngles + vecRotOffset;
            if (bIsFollow_RotX)
                vecCurrentRot.x = vecTargetRot.x;

            if (bIsFollow_RotY)
                vecCurrentRot.y = vecTargetRot.y;

            if (bIsFollow_RotZ)
                vecCurrentRot.z = vecTargetRot.z;

            transform.rotation = Quaternion.Euler(vecCurrentRot);
        }
        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }

#if UNITY_EDITOR
    public class TransformFollower_Inspector : Editor
    {

    }
#endif
}