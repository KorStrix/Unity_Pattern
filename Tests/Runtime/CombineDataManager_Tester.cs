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
        /// ���� ������ �Ŵ���<see cref="CombineDataManager"/>�� ���� ���̽� �׽�Ʈ�Դϴ�.
        /// </summary>
        /// <example>
        /// 
        /// �׽�Ʈ ����
        /// 
        /// 1. ���� ���� �Ŵ���<see cref="CombineManager_Example"/> ����
        /// 2. �Ŵ����� ������ ������ ���
        /// 
        /// 3. ���� ������ ������ ��� ��� - ����
        /// 4. ���� ������ ������ ��� ��� - ����
        ///
        /// 5. ���� �õ� - ����
        /// 6. ���� �õ� - ���� - �Ѱ��� ������ ���̽�
        /// 7. ���� �õ� - ���� - ������ ������ ���̽�
        /// 
        /// </example>
        [UnityTest]
        public IEnumerator CombineDataManager_UseCaseTest()
        {
            /// 1. ���� ���� �Ŵ���<see cref="CombineManager_Example"/> ����
            GameObject pObjectManager = new GameObject(nameof(CombineManager_Example));
            CombineManager_Example pManagerExample = pObjectManager.AddComponent<CombineManager_Example>();



            /// 2. �Ŵ����� ������ ������ ���
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



            /// 3. ���� ������ ������ ��� ��� - ����
            pManagerExample.listCombinationItem.Clear();
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Beef, 1));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Water, 1));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Fire, 1));

            ICombineRecipe[] arrCombineRecipe = pManagerExample.DoCheck_PossibleCombineRecipe();
            Assert.AreEqual(arrCombineRecipe.Length, 0);



            /// 4. ���� ������ ������ ��� ��� - ����
            const int const_iHasMaterialCount = 5;

            pManagerExample.listCombinationItem.Clear();
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Beef, const_iHasMaterialCount));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Water, const_iHasMaterialCount));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Fire, const_iHasMaterialCount));

            arrCombineRecipe = pManagerExample.DoCheck_PossibleCombineRecipe();
            Assert.AreEqual(arrCombineRecipe.Length, 2);



            /// 5. ���� �õ� - ����
            bool bCombieResult = pManagerExample.DoCombine(CombineManager_Example.ERecipeKey.Cookie);
            Assert.IsFalse(bCombieResult);



            /// 6. ���� �õ� - ���� - �Ѱ��� ������ ���̽�
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



            /// 7. ���� �õ� - ���� - ������ ������ ���̽�
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
