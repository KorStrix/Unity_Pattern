using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CSingletonMonoBase<CLASS_DERIVED> : Unity_Pattern.CObjectBase
    where CLASS_DERIVED : CSingletonMonoBase<CLASS_DERIVED>
{
    public static CLASS_DERIVED instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CLASS_DERIVED>();
                if (_instance == null)
                {
                    if (_bIsQuitApplication)
                    {
                        return null;
                    }
                    else
                    {
                        for (int i = 0; i < SceneManager.sceneCount; i++)
                        {
                            Scene pScene = SceneManager.GetSceneAt(i);
                            GameObject[] arrObject = pScene.GetRootGameObjects();
                            for(int j = 0; j < arrObject.Length; j++)
                            {
                                _instance = arrObject[j].GetComponentInChildren<CLASS_DERIVED>();
                                if (_instance != null)
                                    break;
                            }
                        }
                    }
                }

                if (_instance != null && _instance.bIsExecute_Awake == false)
                    _instance.Awake();
            }

            return _instance;
        }
    }

    private static CLASS_DERIVED _instance;
    private static bool _bIsQuitApplication = false;

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        if (bIsExecute_Awake == false)
        {
            if (_instance == null)
                _instance = FindObjectOfType<CLASS_DERIVED>();
        }

        base.OnAwake();
    }

    void OnDestroy()
    {
        _instance = null;
        bIsExecute_Awake = false;
    }

    private void OnApplicationQuit()
    {
        _bIsQuitApplication = true;
    }

    // ========================== [ Division ] ========================== //

    public static CLASS_DERIVED EventMakeSingleton()
    {
        if (_bIsQuitApplication) return null;
        if (_instance != null) return instance;

        GameObject pObjectNewManager = new GameObject(typeof(CLASS_DERIVED).ToString());
        return pObjectNewManager.AddComponent<CLASS_DERIVED>();
    }
}
