using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-04 오후 5:54:23
   Description : 
   Edit Log    : 
   ============================================ */

/// <summary>
/// <see cref="CSingletonNotMonoBase<>"/>에서 Coroutine을 사용하기 위한 핼퍼 클래스
/// </summary>
public class CSingletonNotMono : Unity_Pattern.CObjectBase
{
    public delegate void delOnDestroy(GameObject pObjectDestroyed, bool bApplication_IsQuit);

    public event System.Action<GameObject> p_Event_OnDisable;
    public event delOnDestroy p_Event_OnDestroy;

    private static GameObject _pObjectManager;
    private static List<System.Action> _listAction = new List<Action>();
    bool _bApplication_IsQuit = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnSceneLoaded()
    {
        _pObjectManager = new GameObject($"{nameof(CSingletonNotMono)}_Manager");

        CSingletonNotMono pManager = _pObjectManager.AddComponent<CSingletonNotMono>();
        pManager.StartCoroutine(pManager.ManagerCoroutine());
    }

    public static void DoAdd_UnityCallBackListener(System.Action OnAction)
    {
        _listAction.Add(OnAction);
    }

    IEnumerator ManagerCoroutine()
    {
        while (true)
        {
            for(int i = 0; i < _listAction.Count; i++)
                _listAction[i]?.Invoke();
            _listAction.Clear();

            yield return null;
        }
    }

    private void OnDisable()
    {
        p_Event_OnDisable?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        p_Event_OnDestroy?.Invoke(gameObject, _bApplication_IsQuit);
    }

    private void OnApplicationQuit()
    {
        _bApplication_IsQuit = true;
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
    static bool _bIsGenerateGameObject = false;
    static bool _bIsRequireInit = false;
    static bool _bApplication_IsQuit = false;

    // ========================== [ Division ] ========================== //

    public static CLASS_DERIVED instance
	{
		get
		{
            if (_instance == null)
            {
                if(_bApplication_IsQuit)
                {
                    return null;
                }

                _instance = new CLASS_DERIVED();
                _bIsRequireInit = true;

                CSingletonNotMono.DoAdd_UnityCallBackListener(OnSceneLoaded);

                // CSingletonNotMono.pUnitySynchronizationContext.Send(_ => OnSceneLoaded(true), null);
                // 실행한 스레드가 Unity 스레드일경우 바로 실행 // 이거 안됨;
                //if (CSingletonNotMono.pUnityThread == Thread.CurrentThread)
                //{
                //    OnSceneLoaded();
                //}
                //else // 아닌 경우 Unity 스레드로
                //{
                //    SynchronizationContext_Unity.Send(_ => OnSceneLoaded(), null);
                //}
            }

            return _instance;
		}
	}

    public static void DoReleaseSingleton()
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

    protected virtual void OnMakeSingleton(out bool bIsGenerateGameObject_Default_Is_False) { bIsGenerateGameObject_Default_Is_False = false; }
    protected virtual void OnReleaseSingleton() { }

    protected virtual void OnMakeGameObject(GameObject pObject, CSingletonNotMono pMono) { }
    protected virtual void OnDestroyGameObject(GameObject pObject) { }


    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] 이게 호출안됨;
    public static void OnSceneLoaded()
    {
        if (_bIsRequireInit == false)
            return;
        _bIsRequireInit = false;
        _bApplication_IsQuit = false;

        _instance.OnMakeSingleton(out _bIsGenerateGameObject);
        if (_bIsGenerateGameObject && _instance.gameObject.IsNull())
        {
            _instance.gameObject = new GameObject(typeof(CLASS_DERIVED).Name);
            _instance.transform = _instance.gameObject.transform;
            _instance._pMono = instance.gameObject.AddComponent<CSingletonNotMono>();
            _instance._pMono.p_Event_OnDestroy += _instance.OnDestroy;

            _instance.OnMakeGameObject(_instance.gameObject, _instance._pMono);
        }
    }

    // ========================== [ Division ] ========================== //

    private void OnDestroy(GameObject pObject, bool bApplication_IsQuit)
    {
        _bApplication_IsQuit = bApplication_IsQuit;
        if (bApplication_IsQuit)
            return;

        OnDestroyGameObject(pObject);
        DoReleaseSingleton();
    }
}