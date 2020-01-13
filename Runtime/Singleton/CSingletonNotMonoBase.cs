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

public class CSingletonNotMonoBase<CLASS_DERIVED> : UnityEngine.Object
    where CLASS_DERIVED : CSingletonNotMonoBase<CLASS_DERIVED>, new()
{
	static private CLASS_DERIVED _instance;

    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }

	// ========================== [ Division ] ========================== //

	static public CLASS_DERIVED instance
	{
		get
		{
			if (UnityEngine.Object.Equals(_instance, null))
                DoCreateInstance_Force(new CLASS_DERIVED());

			return _instance;
		}
	}

    static public void DoCreateInstance_Force(CLASS_DERIVED pSingletonInstance)
    {
        _instance = pSingletonInstance;

        bool bIsGenearteGameObject;
        _instance.OnMakeSingleton(out bIsGenearteGameObject);
        if (bIsGenearteGameObject)
        {
            System.Type pTypeDERIVED = typeof(CLASS_DERIVED);
            _instance.gameObject = new GameObject(pTypeDERIVED.Name);
            _instance.transform = _instance.gameObject.transform;

            EventTrigger_OnDisable pOnDisable = _instance.gameObject.AddComponent<EventTrigger_OnDisable>();
            pOnDisable.p_Event_OnDisable += _instance.OnDisable_p_Event_OnDisable;
            SceneManager.sceneUnloaded += _instance.OnSceneUnloaded;

            _instance.OnMakeGameObject(_instance.gameObject);
        }
    }

    static public void DoReleaseSingleton()
	{
		if(UnityEngine.Object.Equals(_instance, null) == false)
			_instance.OnReleaseSingleton();

		_instance = null;
	}

    // ========================== [ Division ] ========================== //

    virtual protected void OnMakeSingleton(out bool bIsGenearteGameObject_Default_Is_False) { bIsGenearteGameObject_Default_Is_False = false; }
    virtual protected void OnReleaseSingleton() { }

    virtual protected void OnMakeGameObject(GameObject pObject) { }
    virtual protected void OnDestroyGameObject(GameObject pObject) { }

    virtual protected void OnSceneUnloaded(Scene pScene) { }


    // ========================== [ Division ] ========================== //

    protected void EventSetInstance(CLASS_DERIVED pInstanceSet)
	{
		_instance = pInstanceSet;
	}

    private void OnDisable_p_Event_OnDisable(GameObject pObject)
    {
        OnDestroyGameObject(pObject);
        DoReleaseSingleton();
    }
}