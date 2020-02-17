using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity_Pattern;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class AchievementDataManager_Tester
    {
        [UnityTest]
        public IEnumerator AchievementDataManager_Test()
        {
            GameObject pObjectManager = new GameObject(nameof(QuestManager_Example));
            pObjectManager.AddComponent<AchievementDataManager>();
            QuestManager_Example pManagerExample = pObjectManager.AddComponent<QuestManager_Example>();

            int iRandom = Random.Range(5, 10);
            int iRandom_2 = Random.Range(5, 10);


            pManagerExample.listQuestData.Add(new QuestManager_Example.QuestDataExample(QuestManager_Example.EQuestKey_Example.Kill_Orc, "오크 죽이기", iRandom));
            pManagerExample.listQuestData.Add(new QuestManager_Example.QuestDataExample(QuestManager_Example.EQuestKey_Example.Kill_Goblin, "고블린 죽이기", iRandom_2));

            pManagerExample.listQuestProgressData.Add(new QuestManager_Example.QuestProgressData_Example(QuestManager_Example.EQuestKey_Example.Kill_Orc, iRandom - 5));
            pManagerExample.listQuestProgressData.Add(new QuestManager_Example.QuestProgressData_Example(QuestManager_Example.EQuestKey_Example.Kill_Goblin, iRandom_2 - 5));

            yield return null;
        }
    }
}
