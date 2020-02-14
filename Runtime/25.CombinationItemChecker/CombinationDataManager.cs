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

namespace Unity_Pattern
{
    public enum ECombinationResult
    {
        None,

        Fail,
        Success,
    }

    public interface IRequireCombinationMaterialData
    {
        string IRequireMaterialKey { get; }
        int iRequireCount { get; }
    }

    public interface ICombinationRecipe
    {
        string strCombinationRecipeKey { get; }
        string strRecipeDescription { get; }

        IEnumerable<IRequireCombinationMaterialData> arrRequireMaterialData { get; }

        bool ICombinationRecipe_IsRequireMaterial(ICombinationMaterial pMaterial);
        bool ICombinationRecipe_IsPossibleCombination(IEnumerable<ICombinationMaterial> arrMaterial);
        bool ICombinationRecipe_Combination(IEnumerable<ICombinationMaterial> arrMaterial);

    }

    public interface ICombinationMaterial
    {
        string strCombinationMaterialKey { get; }
        int iMaterialCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CombinationDataManager : CSingletonDynamicMonoBase<CombinationDataManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        /* protected & private - Field declaration  */

        Dictionary<string, List<ICombinationRecipe>> _mapRecipe = new Dictionary<string, List<ICombinationRecipe>>();
        Dictionary<Dictionary<string, IRequireCombinationMaterialData>, ICombinationRecipe> _mapRecipe_KeyIs_RequireMaterial = new Dictionary<Dictionary<string, IRequireCombinationMaterialData>, ICombinationRecipe>();

        HashSet<ICombinationRecipe> _setRecipeTemp_Return = new HashSet<ICombinationRecipe>();
        HashSet<ICombinationRecipe> _setRecipeTemp_Remove = new HashSet<ICombinationRecipe>();

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        public void DoInit_CombinationData<TCombinationRecipe>(TCombinationRecipe[] arrSourceData)
            where TCombinationRecipe : ICombinationRecipe
        {
            var pSourceData_Group = arrSourceData.GroupBy(p => p.strCombinationRecipeKey);
            _mapRecipe = pSourceData_Group.ToDictionary(p => p.Key, x => x.Select(y => (ICombinationRecipe)y).ToList());
            _mapRecipe_KeyIs_RequireMaterial = 
                arrSourceData.ToDictionary(
                    x => x.arrRequireMaterialData.ToDictionary(y => y.IRequireMaterialKey, y => y),
                    z => (ICombinationRecipe)z);
        }

        public bool DoGet_Possible_CombinationRecipeArray(IEnumerable<ICombinationMaterial> arrMaterial, out ICombinationRecipe[] arrRecipe)
        {
            _setRecipeTemp_Return.Clear();
            _setRecipeTemp_Remove.Clear();

            foreach (ICombinationMaterial pMaterial in arrMaterial)
            {
                foreach (var pSet in _mapRecipe_KeyIs_RequireMaterial)
                {
                    IRequireCombinationMaterialData pRequireCombinationMaterial;
                    if (pSet.Key.TryGetValue(pMaterial.strCombinationMaterialKey, out pRequireCombinationMaterial) == false)
                        continue;

                    if (pMaterial.iMaterialCount < pRequireCombinationMaterial.iRequireCount)
                        continue;

                    if (pSet.Value.ICombinationRecipe_IsRequireMaterial(pMaterial))
                        _setRecipeTemp_Return.Add(pSet.Value);
                }
            }

            foreach (var pRecipe in _setRecipeTemp_Return)
            {
                if (pRecipe.ICombinationRecipe_IsPossibleCombination(arrMaterial) == false)
                    _setRecipeTemp_Remove.Add(pRecipe);
            }

            foreach(var pRecipeRemove in _setRecipeTemp_Remove)
                _setRecipeTemp_Return.Remove(pRecipeRemove);

            arrRecipe = _setRecipeTemp_Return.ToArray();
            return arrRecipe.Length != 0;
        }

        public bool DoCombinationRecipe(ICombinationRecipe pRecipe, IEnumerable<ICombinationMaterial> arrMaterial)
        {
            if (pRecipe.ICombinationRecipe_IsPossibleCombination(arrMaterial) == false)
                return false;

            foreach (ICombinationMaterial pMaterial in arrMaterial)
            {
                IRequireCombinationMaterialData pRequireCombinationMaterial = pRecipe.arrRequireMaterialData.Where(p => p.IRequireMaterialKey == pMaterial.strCombinationMaterialKey).FirstOrDefault();
                if (pRequireCombinationMaterial == null)
                    continue;

                if (pMaterial.iMaterialCount < pRequireCombinationMaterial.iRequireCount)
                    continue;

                pMaterial.iMaterialCount -= pRequireCombinationMaterial.iRequireCount;
            }

            return pRecipe.ICombinationRecipe_Combination(arrMaterial);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            DontDestroyOnLoad(gameObject);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}