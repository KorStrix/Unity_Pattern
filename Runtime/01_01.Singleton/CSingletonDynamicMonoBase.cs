#region Header
/* ============================================ 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;

/// <summary>
/// 인스턴스가 없으면 동적으로 게임오브젝트를 생성하는 싱글톤.
/// </summary>
public class CSingletonDynamicMonoBase<CLASS_DERIVED> : Unity_Pattern.CObjectBase
    where CLASS_DERIVED : CSingletonDynamicMonoBase<CLASS_DERIVED>
{
    static public CLASS_DERIVED instance
    {
        get
        {
            if (_instance == null)
            {
                if(_bIsQuitApplication)
                {
                    return null;
                }
                else
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                        return null;
                    // return new CLASS_SingletoneTarget(); // Exception 방지를 위한 코드, 어차피 Editor에서 PlayMode -> EditMode로 돌아가는 과정이라 App 성능에 영향가지 않는다.
#endif

                    _instance = FindObjectOfType<CLASS_DERIVED>();
                    if (_instance == null)
                        Create_And_SetInstance();
                }
            }

            return _instance;
        }
    }

    static private CLASS_DERIVED _instance;
    static protected bool _bIsQuitApplication { get; private set; } = false;

    // ========================== [ Division ] ========================== //

    static public void DoReleaseSingleton()
	{
		if (_instance != null)
		{
			_instance.OnReleaseSingleton();
            _instance.bIsExecute_Awake = false;
            _instance = null;
        }
    }
	
	static public void DoSetParents_ManagerObject( Transform pTransformParents )
	{
        Transform pTransform = instance.transform;
		pTransform.SetParent( pTransformParents );
        pTransform.localScale = Vector3.one;
        pTransform.localRotation = Quaternion.identity;
        pTransform.position = Vector3.zero;
    }

    static public void DoDestroySingleton()
    {
        instance.OnDestroySingleton();
        Destroy(instance.gameObject);

        _instance = null;
    }

    // ========================== [ Division ] ========================== //

    virtual protected void OnMakeSingleton() { }
    virtual protected void OnReleaseSingleton() { }

    virtual protected void OnDestroySingleton() { }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        if (_instance == null)
        {
            _instance = this as CLASS_DERIVED;
            OnMakeSingleton();
        }
    }

    void OnDestroy()
    {
        DoReleaseSingleton();
    }

    private void OnApplicationQuit()
    {
        _bIsQuitApplication = true;
    }

    // ========================== [ Division ] ========================== //

    static public CLASS_DERIVED EventMakeSingleton(bool bIsCreateNew_Force = false)
    {
        if (_bIsQuitApplication)
            return null;

        if (bIsCreateNew_Force == false && _instance != null)
            return instance;

        Create_And_SetInstance();

        return instance;
    }

    private static void Create_And_SetInstance()
    {
        GameObject pObjectDynamicGenerate = new GameObject(typeof(CLASS_DERIVED).Name + "_Generated_OnRunTime");
        _instance = pObjectDynamicGenerate.AddComponent<CLASS_DERIVED>();
        _instance.OnMakeSingleton();

        if (_instance.bIsExecute_Awake == false)
            _instance.Awake();
    }

}
