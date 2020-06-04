using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2020-06-04 오후 3:54:23
   Description : Monostate 패턴은 Singleton과 비슷합니다.


    Singleton와 같은점 : 인스턴스의 단일을 보장합니다.
    Singleton와 다른점 : Wrapper를 통해 인스턴스에 접근합니다.


    Singleton대비 장점
    - 컴파일 중에 인스턴스를 생성할 수 있습니다.
    - 유니티에 종속되지 않는 순수 클래스를 매니져로 만들 수 있습니다.

    Singleton대비 단점
    - Singleton보다 인스턴스 접근에 직관적이지 않습니다.


   Edit Log    : 
   ============================================ */

/// <summary>
/// <see cref="CMonostate<>"/>에서 Coroutine을 사용하기 위한 핼퍼 클래스
/// </summary>
public class CMonostate : Unity_Pattern.CObjectBase
{
    public delegate void delOnDestroy(GameObject pObjectDestroyed, bool bApplication_IsQuit);

    public event System.Action<GameObject> p_Event_OnDisable;
    public event delOnDestroy p_Event_OnDestroy;

    public static bool g_bApplication_IsQuit = false;

    private static GameObject _pObjectManager;
    private static List<System.Action> _listAction = new List<Action>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnSceneLoaded()
    {
        _pObjectManager = new GameObject($"{nameof(CMonostate)}_Manager");

        CMonostate pManager = _pObjectManager.AddComponent<CMonostate>();
        pManager.StartCoroutine(pManager.ManagerCoroutine());
    }

    public static void DoAdd_UnityCallBackListener(System.Action OnAction)
    {
        _listAction.Add(OnAction);
    }

    public static void DoRemove_UnityCallBackListener(System.Action OnAction)
    {
        _listAction.Remove(OnAction);
    }

    IEnumerator ManagerCoroutine()
    {
        g_bApplication_IsQuit = false;

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
        p_Event_OnDestroy?.Invoke(gameObject, g_bApplication_IsQuit);
    }

    private void OnApplicationQuit()
    {
        g_bApplication_IsQuit = true;
    }
}

public interface IMonoState
{
    void IMonoState_OnMakeInstance(out bool bIsGenerateGameObject_Default_Is_False);
    void IMonoState_OnReleaseInstance();

    void IMonoState_OnMakeGameObject(GameObject pObject, CMonostate pMono);
    void IMonoState_OnDestroyGameObject(GameObject pObject);
}

public static class MonoStateHelper
{
    public static GameObject gameObjectGetter<T>(this T pMonostate)
        where T : class, IMonoState, new()
    {
        return CMonostateWrapper<T>.gameObject;
    }

    public static Transform transformGetter<T>(this T pMonostate)
        where T : class, IMonoState, new()
    {
        return CMonostateWrapper<T>.transform;
    }

    public static CMonostate monoGetter<T>(this T pMonostate)
        where T : class, IMonoState, new()
    {
        return CMonostateWrapper<T>.pMono;
    }
}

/// <summary>
/// 제네릭 클래스의 경우 AddComponent를 할 수 없으므로 그를 위한 클래스
/// </summary>
public class CMonostateWrapper<TCLASS_MONOSTATE>
    where TCLASS_MONOSTATE : class, IMonoState, new()
{
    public static GameObject gameObject
    {
        get
        {
            GetInstance(); return _gameObject;
        }
    }

    public static Transform transform
    {
        get
        {
            GetInstance(); return _transform;
        }
    }

    public static CMonostate pMono
    {
        get
        {
            GetInstance(); return _pMono;
        }
    }

    private static GameObject _gameObject;
    private static Transform _transform;

    private static CMonostate _pMono;

    static TCLASS_MONOSTATE _instance;
    static bool _bIsGenerateGameObject = false;
    static bool _bIsRequireInit = false;
    static bool _bApplication_IsQuit = false;

    // ========================== [ Division ] ========================== //

    public CMonostateWrapper()
    {
        if (_bApplication_IsQuit)
            return;

        GetInstance();
    }

    public static TCLASS_MONOSTATE instance => GetInstance();
    public TCLASS_MONOSTATE Instance => GetInstance();

    public static TCLASS_MONOSTATE GetInstance()
    {
        if (_instance == null)
        {
            _instance = new TCLASS_MONOSTATE();
            _bIsRequireInit = true;

            _instance.IMonoState_OnMakeInstance(out _bIsGenerateGameObject);
        }

        if (_gameObject.IsNull() && _bIsGenerateGameObject)
            CMonostate.DoAdd_UnityCallBackListener(OnSceneLoaded);

        return _instance;
    }

    public static void DoReleaseSingleton()
	{
        CMonostate.DoRemove_UnityCallBackListener(OnSceneLoaded);

        _instance?.IMonoState_OnReleaseInstance();

        if (gameObject.IsNull() == false)
            GameObject.Destroy(gameObject);

        _instance = null;
        _transform = null;
        _gameObject = null;
	}

    // ========================== [ Division ] ========================== //


    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] 이게 호출안됨;
    public static void OnSceneLoaded()
    {
        if (_bIsRequireInit == false)
            return;
        _bIsRequireInit = false;
        _bApplication_IsQuit = CMonostate.g_bApplication_IsQuit;

        if (_bIsGenerateGameObject && gameObject.IsNull())
        {
            _gameObject = new GameObject(typeof(TCLASS_MONOSTATE).Name);
            _transform = gameObject.transform;
            _pMono = gameObject.AddComponent<CMonostate>();
            _pMono.p_Event_OnDestroy += OnDestroy;

            _instance.IMonoState_OnMakeGameObject(gameObject, pMono);
        }
    }

    // ========================== [ Division ] ========================== //

    private static void OnDestroy(GameObject pObject, bool bApplication_IsQuit)
    {
        _bApplication_IsQuit = bApplication_IsQuit;
        if (bApplication_IsQuit)
            return;

        _instance?.IMonoState_OnDestroyGameObject(pObject);
        DoReleaseSingleton();
    }
}