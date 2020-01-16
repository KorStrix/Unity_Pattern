using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-04 오후 5:54:23
   Description : 
   Edit Log    : 
   ============================================ */

/// <summary>
/// <see cref="CSingletonNotMonoBase<>"/>에서 Coroutine을 사용하기 위한 핼퍼 클래스
/// </summary>
public class CSingletonNotMono : CObjectBase
{
    public event System.Action<GameObject> p_Event_OnDisable;
    public event System.Action<GameObject> p_Event_OnDestroy;

    private void OnDisable()
    {
        if (p_Event_OnDisable != null)
            p_Event_OnDisable(gameObject);
    }

    private void OnDestroy()
    {
        if (p_Event_OnDestroy != null)
            p_Event_OnDestroy(gameObject);
    }
}

/// <summary>
/// 제네릭 클래스의 경우 AddComponent를 할 수 없으므로 그를 위한 클래스
/// </summary>
public class CSingletonNotMonoBase<CLASS_DERIVED>
    where CLASS_DERIVED : CSingletonNotMonoBase<CLASS_DERIVED>, new()
{
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }

    protected CSingletonNotMono _pMono { get; private set; }

    static CLASS_DERIVED _instance;
    static bool _bIsGenearteGameObject = false;

    // ========================== [ Division ] ========================== //

    public CSingletonNotMonoBase()
    {
        _instance = this as CLASS_DERIVED;
        _instance.OnMakeSingleton(out _bIsGenearteGameObject);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    static public CLASS_DERIVED instance
	{
		get
		{
            if (_instance == null)
                DoCreateInstance_Force();

			return _instance;
		}
	}

    static public void DoCreateInstance_Force()
    {
        new CLASS_DERIVED();
    }

    static public void DoReleaseSingleton()
	{
		if(_instance != null)
        {
            _instance.OnReleaseSingleton();

            if (_instance.gameObject.IsNull() == false)
                GameObject.Destroy(_instance.gameObject);
        }

        _instance = null;
	}

    // ========================== [ Division ] ========================== //

    virtual protected void OnMakeSingleton(out bool bIsGenearteGameObject_Default_Is_False) { bIsGenearteGameObject_Default_Is_False = false; }
    virtual protected void OnReleaseSingleton() { }

    virtual protected void OnMakeGameObject(GameObject pObject, CSingletonNotMono pMono) { }
    virtual protected void OnDestroyGameObject(GameObject pObject) { }


    private void SceneManager_sceneLoaded(Scene pScene, LoadSceneMode eLoadSceneMode)
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

        if (UnityEngine.Object.ReferenceEquals(_instance, null))
            return;

        if (_bIsGenearteGameObject && gameObject.IsNull())
        {
            System.Type pTypeDERIVED = typeof(CLASS_DERIVED);
            _instance.gameObject = new GameObject(pTypeDERIVED.Name);
            _instance.transform = _instance.gameObject.transform;
            _instance._pMono = instance.gameObject.AddComponent<CSingletonNotMono>();

            _instance._pMono.p_Event_OnDestroy += _instance.OnDestroy;
            SceneManager.sceneUnloaded += _instance.OnSceneUnloaded;

            _instance.OnMakeGameObject(_instance.gameObject, _instance._pMono);
        }
    }


    virtual protected void OnSceneUnloaded(Scene pScene) { }


    // ========================== [ Division ] ========================== //

    protected void EventSetInstance(CLASS_DERIVED pInstanceSet)
	{
		_instance = pInstanceSet;
	}

    private void OnDestroy(GameObject pObject)
    {
        OnDestroyGameObject(pObject);
        DoReleaseSingleton();
    }
}