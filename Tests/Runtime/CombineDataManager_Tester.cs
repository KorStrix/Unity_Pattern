using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        /// 6. ���� �õ� - ����
        /// 
        /// </example>
        [UnityTest]
        public IEnumerator CombineDataManager_UseCaseTest()
        {
            /// 1. ���� ���� �Ŵ���<see cref="CombineManager_Example"/> ����
            GameObject pObjectManager = new GameObject(nameof(CombineManager_Example));
            CombineDataManager pDataManager = pObjectManager.AddComponent<CombineDataManager>();
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

            pDataManager.DoInit_CombineData(pManagerExample.listRecipeData.ToArray());



            /// 3. ���� ������ ������ ��� ��� - ����
            pManagerExample.listCombinationItem.Clear();
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Beef, 1));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Water, 1));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Fire, 1));

            ICombineRecipe[] arrCombineRecipe = pManagerExample.DoCheck_PossibleCombineRecipe();
            Assert.AreEqual(arrCombineRecipe.Length, 0);



            /// 4. ���� ������ ������ ��� ��� - ����
            pManagerExample.listCombinationItem.Clear();
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Beef, 5));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Water, 5));
            pManagerExample.listCombinationItem.Add(new CombineManager_Example.CombinationItem_Example(CombineManager_Example.EItemKey.Fire, 5));

            arrCombineRecipe = pManagerExample.DoCheck_PossibleCombineRecipe();
            Assert.AreEqual(arrCombineRecipe.Length, 2);



            /// 5. ���� �õ� - ����
            bool bCombieResult = pManagerExample.DoCombine(CombineManager_Example.ERecipeKey.Cookie);
            Assert.IsFalse(bCombieResult);



            /// 6. ���� �õ� - ���� - �Ѱ��� ������ ���̽�
            bCombieResult = pManagerExample.DoCombine(CombineManager_Example.ERecipeKey.Stew);
            Assert.IsTrue(bCombieResult);


            yield return null;
        }
    }
}
