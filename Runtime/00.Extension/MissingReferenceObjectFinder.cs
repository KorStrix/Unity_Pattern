#region Header
/*	============================================
 *	Author 			    : Strix
 *	Initial Creation Date 	: 2020-03-03
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// 
/// </summary>
// http://wiki.unity3d.com/index.php/FindMissingScripts
public class MissingReferenceObjectFinder
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration               */

    static int iGameObjectCount = 0, iComponentCount = 0, iMissing_count = 0;

    [MenuItem("Tools/Find Missing Reference Component", priority = -10000000)]
    [MenuItem("GameObject/Find Missing Reference Component", priority = -10000000)]
    public static void Find_Missing_Reference_Component()
    {
        ClearCount();

        GameObject[] arrObject = Selection.gameObjects;
        if(arrObject.Length == 0)
        {
            var pCurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            arrObject = pCurrentScene.GetRootGameObjects();
        }

        foreach (GameObject pObject in arrObject)
            FindMissing_InGameObject(pObject);

        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", iGameObjectCount, iComponentCount, iMissing_count));
    }

    private static void ClearCount()
    {
        iGameObjectCount = 0;
        iComponentCount = 0;
        iMissing_count = 0;
    }

    /* protected & private - Field declaration  */


    // ========================================================================== //

    /* public - [Do~Something] Function 	        */


    // ========================================================================== //

    /* protected - [Override & Unity API]       */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private
    private static void FindMissing_InGameObject(GameObject pObject)
    {
        iGameObjectCount++;
        Component[] arrComponent = pObject.GetComponents<Component>();
        for (int i = 0; i < arrComponent.Length; i++)
        {
            iComponentCount++;
            Component pComponent = arrComponent[i];
            if (pComponent != null)
                continue;

            iMissing_count++;
            string strObjectName = pObject.name;
            Transform pTransform = pObject.transform;
            while (pTransform.parent != null)
            {
                strObjectName = pTransform.parent.name + "/" + strObjectName;
                pTransform = pTransform.parent;
            }

            Debug.Log(strObjectName + " has an empty script attached in position: " + i, pObject);
            EditorGUIUtility.PingObject(pComponent);
        }


        // Now recurse through each child GO (if there are any):
        foreach (Transform pChildObject in pObject.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindMissing_InGameObject(pChildObject.gameObject);
        }
    }

    #endregion Private
}
#endif