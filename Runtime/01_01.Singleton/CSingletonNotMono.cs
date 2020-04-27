using System.Threading;
using UnityEngine;
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
public class CSingletonNotMono : Unity_Pattern.CObjectBase
{
    public delegate void delOnDestroy(GameObject pObjectDestroyed, bool bApplication_IsQuit);

    public event System.Action<GameObject> p_Event_OnDisable;
    public event delOnDestroy p_Event_OnDestroy;

    bool _bApplication_IsQuit = false;


    public static Thread pUnityThread { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnSceneLoaded()
    {
        pUnityThread = Thread.CurrentThread;
    }

    private void OnDisable()
    {
        if (p_Event_OnDisable != null)
            p_Event_OnDisable(gameObject);
    }

    private void OnDestroy()
    {
        if (p_Event_OnDestroy != null)
            p_Event_OnDestroy(gameObject, _bApplication_IsQuit);

        pUnityThread = null;
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
    static bool _bIsGenearteGameObject = false;

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

                // 실행한 스레드가 Unity 스레드일경우 바로 실행
                if (CSingletonNotMono.pUnityThread == Thread.CurrentThread)
                {
                    OnSceneLoaded();
                }
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

    protected virtual void OnMakeSingleton(out bool bIsGenearteGameObject_Default_Is_False) { bIsGenearteGameObject_Default_Is_False = false; }
    protected virtual void OnReleaseSingleton() { }

    protected virtual void OnMakeGameObject(GameObject pObject, CSingletonNotMono pMono) { }
    protected virtual void OnDestroyGameObject(GameObject pObject) { }


    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnSceneLoaded()
    {
        if (_bIsRequireInit == false)
            return;
        _bIsRequireInit = false;
        _bApplication_IsQuit = false;

        _instance.OnMakeSingleton(out _bIsGenearteGameObject);
        if (_bIsGenearteGameObject && _instance.gameObject.IsNull())
        {
            System.Type pTypeDERIVED = typeof(CLASS_DERIVED);
            _instance.gameObject = new GameObject(pTypeDERIVED.Name);
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