using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using Unity_Pattern;

namespace StrixLibrary_Test
{
    public class CombineDataManager_Tester
    {
        /// <summary>
        /// 조합 데이터 매니져<see cref="CombineDataManager"/>의 유스 케이스 테스트입니다.
        /// </summary>
        /// <example>
        /// 
        /// 테스트 절차
        /// 
        /// 1. 예시 조합 매니져<see cref="CombineManager_Example"/> 생성
        /// 2. 매니져에 레시피 데이터 등록
        /// 
        /// 3. 조합 가능한 레시피 목록 출력 - 없음
        /// 4. 조합 가능한 레시피 목록 출력 - 있음
        ///
        /// 5. 조합 시도 - 실패
        /// 6. 조합 시도 - 성공 - 한개만 가능한 케이스
        /// 7. 조합 시도 - 성공 - 여러개 가능한 케이스
        /// 
        /// </example>
        [UnityTest]
        public IEnumerator CombineDataManager_UseCaseTest()
        {
            /// 1. 예시 조합 매니져<see cref="CombineManager_Example"/> 생성
            GameObject pObjectManager = new GameObject(nameof(CombineManager_Example));
            CombineManager_Example pManagerExample = pObjectManager.AddComponent<CombineManager_Example>();



            /// 2. 매니져에 레시피 데이터 등록
            pManagerExample.listRecipeData.Add(new CombineManager_Example.RecipeData_Example(CombineManager_Example.ERecipeKey.Cookie,
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Dough, 2),
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Water, 2),
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Fire, 1)));

            pManagerExample.listRecipeData.Add(new CombineManager_Example.RecipeData_Example(CombineManager_Example.ERecipeKey.RoastMeat,
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Beef, 2),
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Fire, 1)));

            pManagerExample.listRecipeData.Add(new CombineManager_Example.RecipeData_Example(CombineManager_Example.ERecipeKey.Stew,
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Beef, 2),
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Water, 2),
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Fire, 1)));

            pManagerExample.listRecipeData.Add(new CombineManager_Example.RecipeData_Example(CombineManager_Example.ERecipeKey.Stew,
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Fork, 2),
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Water, 2),
                new CombineManager_Example.RequireCombineMaterialData(CombineManager_Example.EItemKey.Fire, 1)));

            pManagerExample.pDataManager.DoInit_CombineData(pManagerExample.listRecipeData.ToArray());



            /// 3. 조합 가능한 레시피 목록 출력 - 없음
            pManagerExample.listCombinationItem.Clear();
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Beef, 1));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Water, 1));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Fire, 1));

            ICombineRecipe[] arrCombineRecipe = pManagerExample.DoCheck_PossibleCombineRecipe();
            Assert.AreEqual(arrCombineRecipe.Length, 0);



            /// 4. 조합 가능한 레시피 목록 출력 - 있음
            const int const_iHasMaterialCount = 5;

            pManagerExample.listCombinationItem.Clear();
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Beef, const_iHasMaterialCount));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Water, const_iHasMaterialCount));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Fire, const_iHasMaterialCount));

            arrCombineRecipe = pManagerExample.DoCheck_PossibleCombineRecipe();
            Assert.AreEqual(arrCombineRecipe.Length, 2);



            /// 5. 조합 시도 - 실패
            bool bCombieResult = pManagerExample.DoCombine(CombineManager_Example.ERecipeKey.Cookie);
            Assert.IsFalse(bCombieResult);



            /// 6. 조합 시도 - 성공 - 한개만 가능한 케이스
            bCombieResult = pManagerExample.DoCombine(CombineManager_Example.ERecipeKey.RoastMeat);
            Assert.IsTrue(bCombieResult);

            List<ICombineRecipe> listRecipe = pManagerExample.pDataManager.GetRecipeList(CombineManager_Example.ERecipeKey.RoastMeat.ToString());
            CombineManager_Example.RecipeData_Example pRecipeData = listRecipe[0] as CombineManager_Example.RecipeData_Example;

            int iRequire_BeefCount, iBeefCount;
            GetItemCount(pManagerExample, pRecipeData, CombineManager_Example.EItemKey.Beef, out iRequire_BeefCount, out iBeefCount);
            Assert.AreEqual(const_iHasMaterialCount - iRequire_BeefCount, iBeefCount);

            int iRequire_FireCount, iFireCount;
            GetItemCount(pManagerExample, pRecipeData, CombineManager_Example.EItemKey.Fire, out iRequire_FireCount, out iFireCount);
            Assert.AreEqual(const_iHasMaterialCount - iRequire_FireCount, iFireCount);



            /// 7. 조합 시도 - 성공 - 여러개 가능한 케이스
            pManagerExample.listCombinationItem.Clear();
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Beef, const_iHasMaterialCount));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Water, const_iHasMaterialCount));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Fire, const_iHasMaterialCount));

            bCombieResult = pManagerExample.DoCombine(CombineManager_Example.ERecipeKey.Stew);
            Assert.IsTrue(bCombieResult);

            listRecipe = pManagerExample.pDataManager.GetRecipeList(CombineManager_Example.ERecipeKey.Stew.ToString());
            pRecipeData = listRecipe[0] as CombineManager_Example.RecipeData_Example;

            yield return null;
        }

        private static void GetItemCount(CombineManager_Example pManagerExample, CombineManager_Example.RecipeData_Example pRecipeData, CombineManager_Example.EItemKey eItemKey, out int iRequireCount, out int iItemCount)
        {
            iRequireCount = pRecipeData.arrRecipeDecrease.Where(p => p.IRequireMaterialKey == eItemKey.ToString()).FirstOrDefault().iRequireCount;
            iItemCount = pManagerExample.listCombinationItem.Where(p => p.strCombineMaterialKey == eItemKey.ToString()).FirstOrDefault().iItemCount;
        }
    }
}
