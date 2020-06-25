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

namespace Unity_Pattern
{
    public enum ECombineResult
    {
        None,

        Fail,
        Success,
    }

    public interface IRequireCombineMaterialData
    {
        string IRequireMaterialKey { get; }
        int iRequireCount { get; }
    }

    public interface ICombineRecipe
    {
        string strCombineRecipeKey { get; }
        IEnumerable<IRequireCombineMaterialData> arrRequireMaterialData { get; }

        bool ICombineRecipe_IsRequireMaterial(ICombineMaterial pMaterial);
        bool ICombineRecipe_IsPossibleCombine(IEnumerable<ICombineMaterial> arrMaterial);
        bool ICombineRecipe_Combine(IEnumerable<ICombineMaterial> arrMaterial);

    }

    public interface ICombineMaterial
    {
        string strCombineMaterialKey { get; }
        int iMaterialCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CombineDataManager
    {
        /* const & readonly declaration             */

        readonly List<ICombineRecipe> _listRecipeDummy = new List<ICombineRecipe>();
        readonly ICombineRecipe[] const_arrEmptyRecipe = new ICombineRecipe[0];

        /* enum & struct declaration                */

        /* public - Field declaration               */

        /* protected & private - Field declaration  */

        Dictionary<string, List<ICombineRecipe>> _mapRecipe = new Dictionary<string, List<ICombineRecipe>>();
        Dictionary<Dictionary<string, IRequireCombineMaterialData>, ICombineRecipe> _mapRecipe_KeyIs_RequireMaterial = new Dictionary<Dictionary<string, IRequireCombineMaterialData>, ICombineRecipe>();

        HashSet<ICombineRecipe> _setRecipeTemp_Return = new HashSet<ICombineRecipe>();
        HashSet<ICombineRecipe> _setRecipeTemp_Remove = new HashSet<ICombineRecipe>();
        HashSet<string> _setMaterialKey = new HashSet<string>();

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        public void DoInit_CombineData<TCombineRecipe>(TCombineRecipe[] arrSourceData)
            where TCombineRecipe : ICombineRecipe
        {
            _mapRecipe.Clear();
            _mapRecipe_KeyIs_RequireMaterial.Clear();

            try
            {
                var pSourceData_Group = arrSourceData.GroupBy(p => p.strCombineRecipeKey);
                _mapRecipe = pSourceData_Group.ToDictionary(p => p.Key, x => x.Select(y => (ICombineRecipe)y).ToList());
                _mapRecipe_KeyIs_RequireMaterial =
                    arrSourceData.ToDictionary(
                        x => x.arrRequireMaterialData.ToDictionary(y => y.IRequireMaterialKey, y => y),
                        z => (ICombineRecipe)z);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{nameof(CombineDataManager)} - {nameof(DoInit_CombineData)} - Error : {e}");
            }
        }

        public List<ICombineRecipe> GetRecipeList(string strRecipeKey)
        {
            List<ICombineRecipe> listRecipe;
            if (_mapRecipe.TryGetValue(strRecipeKey, out listRecipe))
                return listRecipe;
            else
                return _listRecipeDummy;
        }

        public bool DoGet_Possible_CombineRecipeArray(IEnumerable<ICombineMaterial> arrMaterial, out ICombineRecipe[] arrRecipe)
        {
            _setRecipeTemp_Return.Clear();
            _setMaterialKey.Clear();

            foreach (ICombineMaterial pMaterial in arrMaterial)
            {
                foreach (var pSet in _mapRecipe_KeyIs_RequireMaterial)
                {
                    IRequireCombineMaterialData pRequireCombineMaterial;
                    if (pSet.Key.TryGetValue(pMaterial.strCombineMaterialKey, out pRequireCombineMaterial) == false)
                        continue;

                    if (pMaterial.iMaterialCount < pRequireCombineMaterial.iRequireCount)
                        continue;

                    if (pSet.Value.ICombineRecipe_IsRequireMaterial(pMaterial))
                    {
                        _setMaterialKey.Add(pMaterial.strCombineMaterialKey);
                        _setRecipeTemp_Return.Add(pSet.Value);
                    }
                }
            }

            _setRecipeTemp_Remove.Clear();
            foreach (var pRecipe in _setRecipeTemp_Return)
            {
                if (Check_Recipe_IsEnoughMaterial(pRecipe, arrMaterial) == false)
                    _setRecipeTemp_Remove.Add(pRecipe);
            }

            foreach (var pRecipeRemove in _setRecipeTemp_Remove)
                _setRecipeTemp_Return.Remove(pRecipeRemove);

            arrRecipe = _setRecipeTemp_Return.ToArray();
            return arrRecipe.Length != 0;
        }

        public bool DoCombineRandom(IEnumerable<ICombineMaterial> arrMaterial, out ICombineRecipe pRecipe, System.Func<ICombineRecipe, int> OnGetRandomPercent = null)
        {
            pRecipe = null;
            ICombineRecipe[] arrRecipe;
            if (DoGet_Possible_CombineRecipeArray(arrMaterial, out arrRecipe) == false)
                return false;

            pRecipe = OnGetRandomPercent != null ? arrRecipe.GetRandomItem(OnGetRandomPercent) : arrRecipe.GetRandomItem();
            if (pRecipe == null)
                return false;

            return DoCombineRecipe(pRecipe, arrMaterial);
        }

        public bool DoCombineRecipe(ICombineRecipe pRecipe, IEnumerable<ICombineMaterial> arrMaterial)
        {
            if (Check_Recipe_IsEnoughMaterial(pRecipe, arrMaterial) == false)
                return false;

            foreach (ICombineMaterial pMaterial in arrMaterial)
            {
                IRequireCombineMaterialData pRequireCombinationMaterial = pRecipe.arrRequireMaterialData.Where(p => p.IRequireMaterialKey == pMaterial.strCombineMaterialKey).FirstOrDefault();
                if (pRequireCombinationMaterial == null)
                    continue;

                if (pMaterial.iMaterialCount < pRequireCombinationMaterial.iRequireCount)
                    continue;

                pMaterial.iMaterialCount -= pRequireCombinationMaterial.iRequireCount;
            }

            return pRecipe.ICombineRecipe_Combine(arrMaterial);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private bool Check_Recipe_IsEnoughMaterial(ICombineRecipe pRecipe, IEnumerable<ICombineMaterial> arrMaterial)
        {
            bool bIsPossible = pRecipe.arrRequireMaterialData.Select(p => p.IRequireMaterialKey).Intersect(_setMaterialKey).Count() == pRecipe.arrRequireMaterialData.Count();
            if (bIsPossible)
                bIsPossible = pRecipe.ICombineRecipe_IsPossibleCombine(arrMaterial);

            return bIsPossible;
        }

        #endregion Private
    }
}