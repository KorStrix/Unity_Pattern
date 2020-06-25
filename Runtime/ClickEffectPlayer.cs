#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-03-18
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity_Pattern;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class ClickEffectPlayer : MonoBehaviour
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration               */


	/* protected & private - Field declaration  */

	IEnumerable<EffectWrapper> _arrTouchEffect;
	System.Func<bool> _OnCheckIsPlay = Check_IsPlayDefault;

	Camera _pCamera;

	// ========================================================================== //

	/* public - [Do~Something] Function 	        */

	public Camera DoFindMainCamera()
	{
		return FindObjectsOfType<Camera>().OrderByDescending(p => p.depth).FirstOrDefault();
	}

	public void DoInit(params EffectWrapper[] arrTouchEffect)
	{
		_arrTouchEffect = arrTouchEffect.Where(p => p.IsNullComponent() == false);
	}

	public void DoSet_CheckIsPlayEffect(System.Func<bool> OnCheckIsPlay)
	{
		_OnCheckIsPlay = OnCheckIsPlay;
	}

	public void DoSet_Camera(Camera pCamera)
	{
		_pCamera = pCamera;
	}

	// ========================================================================== //

	/* protected - [Override & Unity API]       */

	private void Awake()
	{
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		DontDestroyOnLoad(gameObject);
	}

	private void SceneManager_sceneLoaded(Scene sScene, LoadSceneMode eSceneMode)
	{
		_pCamera = DoFindMainCamera();
	}

	private void Update()
	{
		if (_arrTouchEffect.IsNullOrEmpty() || _pCamera == null)
			return;

		if (_OnCheckIsPlay() == false)
			return;

		Play_TouchEffect();
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
	}

	/* protected - [abstract & virtual]         */

	// ========================================================================== //

	#region Private
	static bool Check_IsPlayDefault() { return Input.GetMouseButton(0); }

	private void Play_TouchEffect()
	{
		Vector3 vecPos = _pCamera.ScreenToWorldPoint(Input.mousePosition);
		vecPos.z += 1000f;

		EffectManager.DoPlayEffect(_arrTouchEffect.GetRandomItem(), vecPos);
	}

	#endregion Private
}