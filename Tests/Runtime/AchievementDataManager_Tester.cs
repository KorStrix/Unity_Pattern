using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Unity_Pattern;

namespace StrixLibrary_Test
{
    public class AchievementDataManager_Tester
    {
        /// <summary>
        /// 업적 데이터 매니져<see cref="AchievementDataManager"/>의 유스 케이스 테스트입니다.
        /// </summary>
        /// <example>
        /// 
        /// 테스트 절차
        /// 
        /// 1. 예시 퀘스트 매니져<see cref="QuestManager_Example"/> 생성
        /// 2. 매니져에 퀘스트 데이터 등록
        /// 3. 매니져에 퀘스트 진행도 데이터 등록
        /// 
        /// 4. 등록된 데이터 기준으로 매니져의 퀘스트 진행도 테스트
        /// 5. 퀘스트 진행 후 매니져의 퀘스트 진행도 테스트
        /// 
        /// 6. 완료된 퀘스트를 퀘스트 매니져에서 제거 후 테스트
        /// 7. 받은 퀘스트를 퀘스트 포기 테스트
        /// 
        /// </example>
        [UnityTest]
        public IEnumerator AchievementDataManager_UseCaseTest()
        {
            /// 1. 예시 퀘스트 매니져<see cref="QuestManager_Example"/> 생성
            GameObject pObjectManager = new GameObject(nameof(QuestManager_Example));
            pObjectManager.AddComponent<AchievementDataManager>();
            QuestManager_Example pManagerExample = pObjectManager.AddComponent<QuestManager_Example>();

            const int iTestMinCount = 5;
            int iRandom = Random.Range(iTestMinCount, 10);
            int iRandom_2 = Random.Range(iTestMinCount, 10);



            /// 2. 매니져에 퀘스트 데이터 등록
            pManagerExample.listQuestData.Add(new QuestManager_Example.QuestDataExample(QuestManager_Example.EQuestKey_Example.Kill_Orc, "오크 죽이기", iRandom));
            pManagerExample.listQuestData.Add(new QuestManager_Example.QuestDataExample(QuestManager_Example.EQuestKey_Example.Get_Wood, "나무 얻기", iRandom_2));



            /// 3. 매니져에 퀘스트 진행도 데이터 등록
            pManagerExample.listQuestProgressData.Add(new QuestManager_Example.QuestProgressData_Example(QuestManager_Example.EQuestKey_Example.Kill_Orc, 0));
            pManagerExample.listQuestProgressData.Add(new QuestManager_Example.QuestProgressData_Example(QuestManager_Example.EQuestKey_Example.Get_Wood, 1));

            pManagerExample.DoAwake_Force();



            /// 4. 매니져가 퀘스트 진행도에 맞게 되있는지 테스트
            var pQuestData_KillOrc = pManagerExample.pQuestDataManager.DoGet_AchievementData(QuestManager_Example.EQuestKey_Example.Kill_Orc.ToString());
            Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.None);

            var pQuestData_GetWood = pManagerExample.pQuestDataManager.DoGet_AchievementData(QuestManager_Example.EQuestKey_Example.Get_Wood.ToString());
            Assert.AreEqual(pQuestData_GetWood.eProgress, EAchieveProgress.In_Progress);



            /// 5. 퀘스트 진행 후 매니져의 퀘스트 진행도 테스트
            for (int i = 0; i < iRandom - 1; i++)
            {
                pManagerExample.DoKillMonster(QuestManager_Example.EQuestMonsterKey_Example.Orc);
                Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.In_Progress);
            }

            pManagerExample.DoKillMonster(QuestManager_Example.EQuestMonsterKey_Example.Orc);
            Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.Success);



            /// 6. 완료된 퀘스트를 퀘스트 매니져에서 제거 후 테스트
            pManagerExample.pQuestDataManager.DoRemove_AchievementProgress(QuestManager_Example.EQuestKey_Example.Kill_Orc.ToString());
            Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.None);



            /// 7. 받은 퀘스트를 퀘스트 포기 테스트
            Assert.AreEqual(pQuestData_GetWood.eProgress, EAchieveProgress.In_Progress);
            pManagerExample.DoGiveUpQuest(QuestManager_Example.EQuestKey_Example.Get_Wood);
            Assert.AreEqual(pQuestData_GetWood.eProgress, EAchieveProgress.GiveUp);

            yield return null;
        }
    }
}
