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
        /// <summary>
        /// ���� ������ �Ŵ����� ��� ���̽� �׽�Ʈ�Դϴ�.
        /// </summary>
        /// <example>
        /// 
        /// �׽�Ʈ ����
        /// 
        /// 1. ����Ʈ �Ŵ��� ����
        /// 2. �Ŵ����� ����Ʈ ������ ���
        /// 3. �Ŵ����� ����Ʈ ���൵ ������ ���
        /// 
        /// 4. ��ϵ� ������ �������� �Ŵ����� ����Ʈ ���൵ �׽�Ʈ
        /// 5. ����Ʈ ���� �� �Ŵ����� ����Ʈ ���൵ �׽�Ʈ
        /// 
        /// 6. �Ϸ�� ����Ʈ�� ����Ʈ �Ŵ������� ���� �� �׽�Ʈ
        /// 7. ���� ����Ʈ�� ����Ʈ ���� �׽�Ʈ
        /// 
        /// </example>
        [UnityTest]
        public IEnumerator AchievementDataManager_UseCaseTest()
        {
            /// 1. ����Ʈ �Ŵ��� ����
            GameObject pObjectManager = new GameObject(nameof(QuestManager_Example));
            pObjectManager.AddComponent<AchievementDataManager>();
            QuestManager_Example pManagerExample = pObjectManager.AddComponent<QuestManager_Example>();

            const int iTestMinCount = 5;
            int iRandom = Random.Range(iTestMinCount, 10);
            int iRandom_2 = Random.Range(iTestMinCount, 10);


            /// 2. �Ŵ����� ����Ʈ ������ ���
            pManagerExample.listQuestData.Add(new QuestManager_Example.QuestDataExample(QuestManager_Example.EQuestKey_Example.Kill_Orc, "��ũ ���̱�", iRandom));
            pManagerExample.listQuestData.Add(new QuestManager_Example.QuestDataExample(QuestManager_Example.EQuestKey_Example.Get_Wood, "���� ���", iRandom_2));


            /// 3. �Ŵ����� ����Ʈ ���൵ ������ ���
            pManagerExample.listQuestProgressData.Add(new QuestManager_Example.QuestProgressData_Example(QuestManager_Example.EQuestKey_Example.Kill_Orc, 0));
            pManagerExample.listQuestProgressData.Add(new QuestManager_Example.QuestProgressData_Example(QuestManager_Example.EQuestKey_Example.Get_Wood, 1));


            /// 4. �Ŵ����� ����Ʈ ���൵�� �°� ���ִ��� �׽�Ʈ
            pManagerExample.DoAwake_Force();
            var pQuestData_KillOrc = pManagerExample.pQuestDataManager.DoGet_AchievementData(QuestManager_Example.EQuestKey_Example.Kill_Orc.ToString());
            Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.None);

            var pQuestData_GetWood = pManagerExample.pQuestDataManager.DoGet_AchievementData(QuestManager_Example.EQuestKey_Example.Get_Wood.ToString());
            Assert.AreEqual(pQuestData_GetWood.eProgress, EAchieveProgress.In_Progress);


            /// 5. ����Ʈ ���� �� �Ŵ����� ����Ʈ ���൵ �׽�Ʈ
            for (int i = 0; i < iRandom - 1; i++)
            {
                pManagerExample.DoKillMonster(QuestManager_Example.EQuestMonsterKey_Example.Orc);
                Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.In_Progress);
            }

            pManagerExample.DoKillMonster(QuestManager_Example.EQuestMonsterKey_Example.Orc);
            Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.Success);


            /// 6. �Ϸ�� ����Ʈ�� ����Ʈ �Ŵ������� ���� �� �׽�Ʈ
            pManagerExample.pQuestDataManager.DoRemove_AchievementProgress(QuestManager_Example.EQuestKey_Example.Kill_Orc.ToString());
            Assert.AreEqual(pQuestData_KillOrc.eProgress, EAchieveProgress.None);


            /// 7. ���� ����Ʈ�� ����Ʈ ���� �׽�Ʈ
            Assert.AreEqual(pQuestData_GetWood.eProgress, EAchieveProgress.In_Progress);
            pManagerExample.DoGiveUpQuest(QuestManager_Example.EQuestKey_Example.Get_Wood);
            Assert.AreEqual(pQuestData_GetWood.eProgress, EAchieveProgress.GiveUp);

            yield return null;
        }
    }
}
