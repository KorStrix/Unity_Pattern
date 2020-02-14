#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-12
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity_Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class CombinationManager_Example : CSingletonDynamicMonoBase<CombinationDataManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum ERecipeKey
        {
            Stew,
            Cookie,

            RoastMeat,
        }

        public enum EItemKey
        {
            Dough,
            Water,

            Fork,
            Beef,
            Fire,
        }

        [System.Serializable]
        public class RequireCombinationMaterialData : IRequireCombinationMaterialData
        {
            public string IRequireMaterialKey => eItemKey.ToString();

            public int iRequireCount => iItemCount;

            public EItemKey eItemKey;
            public int iItemCount;
        }

        [System.Serializable]
        public class RecipeData_Example : ICombinationRecipe
        {
            public string strCombinationRecipeKey => eRecipeName.ToString();

            public string strRecipeDescription { get; private set; }

            IEnumerable<IRequireCombinationMaterialData> ICombinationRecipe.arrRequireMaterialData => arrRecipeDecrease;

            public ERecipeKey eRecipeName;
            public RequireCombinationMaterialData[] arrRecipeDecrease;

            public bool ICombinationRecipe_IsRequireMaterial(ICombinationMaterial pMaterial)
            {
                return true;
            }

            public bool ICombinationRecipe_IsPossibleCombination(IEnumerable<ICombinationMaterial> arrMaterial)
            {
                return true;
            }

            public bool ICombinationRecipe_Combination(IEnumerable<ICombinationMaterial> arrMaterial)
            {
                return true;
            }
        }


        [System.Serializable]
        public class CombinationItem_Example : ICombinationMaterial
        {
            public string strCombinationMaterialKey => eItemKey.ToString();

            public int iMaterialCount { get => iItemCount; set => iItemCount = value; }

            public EItemKey eItemKey;
            public int iItemCount;
        }

        /* public - Field declaration               */

        [Header("테스트할 레시피")]
        public ERecipeKey eRecipeName_ForTest;

        [Header("조합 레시피"), Space(10)]
        public List<RecipeData_Example> listRecipeData = new List<RecipeData_Example>();

        [Header("조합용 아이템")]
        public List<CombinationItem_Example> listCombinationItem = new List<CombinationItem_Example>();

        /* protected & private - Field declaration  */

        CombinationDataManager _pDataManager;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoCheck_Combination()
        {
            ICombinationRecipe[] arrRecipe;
            if(_pDataManager.DoGet_Possible_CombinationRecipeArray(listCombinationItem, out arrRecipe))
            {
                for(int i = 0; i < arrRecipe.Length; i++)
                {
                    Debug.Log("Combination : " + arrRecipe[i].strCombinationRecipeKey);
                }
            }
        }

        public void DoCombinationRecipe(ERecipeKey eRecipeKey)
        {
            RecipeData_Example pRecipeData = listRecipeData.Where(p => p.eRecipeName == eRecipeKey).FirstOrDefault();
            if (pRecipeData == null)
            {
                Debug.LogError($"{nameof(DoCombinationRecipe)} Not Found RecipeKey : {eRecipeKey}");
                return;
            }

            if(_pDataManager.DoCombinationRecipe(pRecipeData, listCombinationItem))
            {
                Debug.Log("Combination Success Recipe - " + pRecipeData.strCombinationRecipeKey);
            }
            else
            {
                Debug.Log("Combination Fail Recipe - " + pRecipeData.strCombinationRecipeKey);
            }
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pDataManager = GetComponent<CombinationDataManager>();
            _pDataManager.DoInit_CombinationData(listRecipeData.ToArray());
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CombinationManager_Example))]
    public class CombinationManager_Example_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CombinationManager_Example pTarget = target as CombinationManager_Example;

            if (GUILayout.Button("Check Combination"))
            {
                pTarget.DoAwake_Force();
                pTarget.DoCheck_Combination();
                Debug.Log("Check Combination");
            }

            if (GUILayout.Button("Try Combination"))
            {
                pTarget.DoAwake_Force();
                pTarget.DoCombinationRecipe(pTarget.eRecipeName_ForTest);
                Debug.Log("Try Combination");
            }
        }
    }

#endif
}