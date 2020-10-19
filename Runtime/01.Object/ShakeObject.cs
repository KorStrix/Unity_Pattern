#region Header
/*	============================================
 *	Author 			    	: Require PlayerPref Key : "Author"
 *	Initial Creation Date 	: 2020-10-13
 *	Summary 		        : 
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity_Pattern
{

    /// <summary>
    /// 
    /// </summary>
    public class ShakeObject : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EShakePos
        {
            All,
            X,
            XY,
            XZ,
            Y,
            YZ,
            Z
        }
        /* public - Field declaration               */

        [SerializeField] [Header("흔들리기 적용위치")] 
        private EShakePos _eShakePosType = EShakePos.All;

        [SerializeField] [Header("기본 흔드는 힘")]
        float _fDefaultShakePow = 1f;

        [SerializeField] [Header("흔드는 힘을 깎는 양")]
        private float _fShakeMinusDelta = 0.1f;

        /* protected & private - Field declaration  */

        private Vector3 _vecOriginPos;
        private float _fRemainShakePow;

        bool _bIsShaking;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoShakeObject()
        {
            DoShakeObject(_fDefaultShakePow);
        }

        public void DoShakeObject(float fShakePow)
        {
            if (_bIsShaking)
                transform.localPosition = _vecOriginPos;

            _bIsShaking = true;
            _fRemainShakePow = fShakePow;
            _vecOriginPos = transform.localPosition;
        }


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void Awake()
        {
            _vecOriginPos = Vector3.one * float.MaxValue; // 더미값
        }

        private void Update()
        {
            if (_bIsShaking == false)
                return;

            if (_fRemainShakePow > 0f)
            {
                Vector3 vecShakePos = RandomRange(AddFloat(_vecOriginPos, -_fRemainShakePow), AddFloat(_vecOriginPos, _fRemainShakePow));
                if (_eShakePosType != EShakePos.All)
                {
                    if (_eShakePosType == EShakePos.Y || _eShakePosType == EShakePos.YZ || _eShakePosType == EShakePos.Z)
                        vecShakePos.x = _vecOriginPos.x;

                    if (_eShakePosType == EShakePos.X || _eShakePosType == EShakePos.XZ || _eShakePosType == EShakePos.Z)
                        vecShakePos.y = _vecOriginPos.y;

                    if (_eShakePosType == EShakePos.X || _eShakePosType == EShakePos.XY || _eShakePosType == EShakePos.Y)
                        vecShakePos.z = _vecOriginPos.z;
                }

                transform.localPosition = vecShakePos;
                _fRemainShakePow -= _fShakeMinusDelta;
            }
            else
            {
                transform.localPosition = _vecOriginPos;
                _bIsShaking = false;
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        public static Vector3 RandomRange(Vector3 vecMinRange, Vector3 vecMaxRange)
        {
            float fRandX = Random.Range(vecMinRange.x, vecMaxRange.x);
            float fRandY = Random.Range(vecMinRange.y, vecMaxRange.y);
            float fRandZ = Random.Range(vecMinRange.z, vecMaxRange.z);

            return new Vector3(fRandX, fRandY, fRandZ);
        }

        public static Vector3 AddFloat(Vector3 vecOrigin, float fAddValue)
        {
            vecOrigin.x += fAddValue;
            vecOrigin.y += fAddValue;
            vecOrigin.z += fAddValue;

            return vecOrigin;
        }

        #endregion Private
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ShakeObject))]
    public class ShakeObject_Inspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShakeObject ptarget = target as ShakeObject;

            if (GUILayout.Button("Test Shake"))
            {
                ptarget.DoShakeObject();
            }
        }
    }
#endif
}
