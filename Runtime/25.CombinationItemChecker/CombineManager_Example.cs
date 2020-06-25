#region Header
/*	============================================
 *	Author   			    : Strix
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
    public class CombineManager_Example : CSingletonDynamicMonoBase<CombineManager_Example>
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
        public class RequireCombineMaterialData : IRequireCombineMaterialData
        {
            public string IRequireMaterialKey => eItemKey.ToString();

            public int iRequireCount => iItemCount;

            public EItemKey eItemKey;
            public int iItemCount;

            public RequireCombineMaterialData(EItemKey eItemKey, int iItemCount)
            {
                this.eItemKey = eItemKey; this.iItemCount = iItemCount;
            }
        }

        [System.Serializable]
        public class RecipeData_Example : ICombineRecipe
        {
            public string strCombineRecipeKey => eRecipeName.ToString();

            IEnumerable<IRequireCombineMaterialData> ICombineRecipe.arrRequireMaterialData => arrRecipeDecrease;

            public ERecipeKey eRecipeName;
            public RequireCombineMaterialData[] arrRecipeDecrease;

            public RecipeData_Example(ERecipeKey eRecipeName, params RequireCombineMaterialData[] arrRecipeDecrease)
            {
                this.eRecipeName = eRecipeName; this.arrRecipeDecrease = arrRecipeDecrease;
            }

            public bool ICombineRecipe_IsRequireMaterial(ICombineMaterial pMaterial)
            {
                return true;
            }

            public bool ICombineRecipe_IsPossibleCombine(IEnumerable<ICombineMaterial> arrMaterial)
            {
                return true;
            }

            public bool ICombineRecipe_Combine(IEnumerable<ICombineMaterial> arrMaterial)
            {
                Debug.Log($"조합중.. {eRecipeName} - ");

                return true;
            }
        }


        [System.Serializable]
        public class CombinationItem_Example : ICombineMaterial
        {
            public string strCombineMaterialKey => eItemKey.ToString();

            public int iMaterialCount { get => iItemCount; set => iItemCount = value; }

            public EItemKey eItemKey;
            public int iItemCount;

            public CombinationItem_Example(EItemKey eItemKey, int iItemCount)
            {
                this.eItemKey = eItemKey; this.iItemCount = iItemCount;
            }
        }

        /* public - Field declaration               */

        public CombineDataManager pDataManager { get; private set; } = new CombineDataManager();

        [Header("테스트할 레시피")]
        public ERecipeKey eRecipeName_ForTest;

        [Header("조합 레시피"), Space(10)]
        public List<RecipeData_Example> listRecipeData = new List<RecipeData_Example>();

        [Header("조합용 아이템")]
        public List<CombinationItem_Example> listCombinationItem = new List<CombinationItem_Example>();

        /* protected & private - Field declaration  */

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public ICombineRecipe[] DoCheck_PossibleCombineRecipe()
        {
            ICombineRecipe[] arrRecipe;
            pDataManager.DoGet_Possible_CombineRecipeArray(listCombinationItem, out arrRecipe);

            return arrRecipe;
        }

        public bool DoCombine(ERecipeKey eRecipeKey)
        {
            RecipeData_Example pRecipeData = listRecipeData.Where(p => p.eRecipeName == eRecipeKey).FirstOrDefault();
            if (pRecipeData == null)
            {
                Debug.LogError($"{nameof(DoCombine)} Not Found RecipeKey : {eRecipeKey}");
                return false;
            }

            return pDataManager.DoCombineRecipe(pRecipeData, listCombinationItem);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            pDataManager.DoInit_CombineData(listRecipeData.ToArray());
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CombineManager_Example))]
    public class CombineManager_Example_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CombineManager_Example pTarget = target as CombineManager_Example;

            if (GUILayout.Button("Check Combine"))
            {
                pTarget.DoAwake_Force();
                pTarget.DoCheck_PossibleCombineRecipe();
                Debug.Log("Check Combine");
            }

            if (GUILayout.Button("Try Combine"))
            {
                pTarget.DoAwake_Force();
                pTarget.DoCombine(pTarget.eRecipeName_ForTest);
                Debug.Log("Try Combination");
            }
        }
    }

#endif
}