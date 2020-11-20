#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-11-02
 *	Summary 		        : 
 *	테스트 지침 링크
 *	https://github.com/KorStrix/Unity_DevelopmentDocs/tree/master/Test
 *
 *  Template 		        : New MVP - Model - Model Tester For ReSharper
   ============================================ */

#endregion Header

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity_Pattern;

namespace UnityPattern_Test
{
    public class CommandManager_Tester
    {
        public class DummyCommand : ICommandAble
        {
            public static int g_iExecutedIndex{ get; private set; }

            public static void DoReset_ExecutedIndex()
            {
                g_iExecutedIndex = 0;
            }

            public int iCommandOrder { get; private set; }
            public int iExecutedIndex { get; private set; }

            public float fWaitSecond { get; private set; }

            public DummyCommand(int iCommandOrder)
            {
                this.iCommandOrder = iCommandOrder;
            }

            public Task DoInitCommand_Async(IReadOnlyList<CancellationToken> arrCancelToken)
            {
                this.iExecutedIndex = 0;

                return Task.CompletedTask;
            }

            public Task DoExecuteCommand_Async(ECommandState eCommandState, IReadOnlyList<CancellationToken> arrCancelToken)
            {
                switch (eCommandState)
                {
                    case ECommandState.None:
                        iExecutedIndex = ++g_iExecutedIndex;
                        break;

                    case ECommandState.Undo:
                        break;
                    
                    case ECommandState.Redo:
                        break;
                }

                if (fWaitSecond == 0f)
                    return Task.CompletedTask;


                return Task.Delay((int)(fWaitSecond * 1000));
            }

            public void DoSet_WaitForSecond(float fWaitSecond)
            {
                this.fWaitSecond = fWaitSecond;
            }

            public void OnError(EWhere eWhere, Exception pException, out bool bIsPossible_NextStep)
            {
                bIsPossible_NextStep = false;
            }
        }

        [Test]
        public void 커맨드를_순차적으로_실행하는지()
        {
            // Arrange (데이터 정렬)
            var listCommand = Create_DummyCommands();
            CommandManager<DummyCommand> pCommandManager = new CommandManager<DummyCommand>();


            // Act (기능 실행)
            _ = pCommandManager.DoExecuteCommand(listCommand);


            // Assert (맞는지 체크)
            for (int i = 0; i < listCommand.Count; i++)
                Assert.AreEqual(listCommand[i].iExecutedIndex, i + 1);
        }

        [Test]
        public void 커맨드를_하나만_실행할수있는지()
        {
            // Arrange (데이터 정렬)
            var listCommand = Create_DummyCommands();
            CommandManager<DummyCommand> pCommandManager = new CommandManager<DummyCommand>();


            for (int i = 0; i < listCommand.Count; i++)
            {
                DummyCommand pCommand = listCommand[i];
                Assert.AreEqual(pCommand.iExecutedIndex, 0);

                // Act (기능 실행)
                _ = pCommandManager.DoExecuteCommand(pCommand);


                // Assert (맞는지 체크)
                Assert.AreEqual(pCommand.iExecutedIndex, i + 1);
            }
        }

        [Test]
        public void 커맨드_일정시간_기다리기_테스트()
        {
            // Arrange (데이터 정렬)
            var listCommand = Create_DummyCommands();
            CommandManager<DummyCommand> pCommandManager = new CommandManager<DummyCommand>();
            DummyCommand pRandomCommand = listCommand[0];

            float fRandomWait = UnityEngine.Random.Range(0.1f, 0.3f);
            pRandomCommand.DoSet_WaitForSecond(fRandomWait);

            Stopwatch pTimer = new Stopwatch();


            // Act (기능 실행)
            pTimer.Start();
            Task.Run(async () =>
            {
                // https://forum.unity.com/threads/async-await-in-unittests.513857/
                await pCommandManager.DoExecuteCommand(pRandomCommand);
            }).GetAwaiter().GetResult();
            pTimer.Stop();


            // Assert (맞는지 체크)
            Assert.AreNotEqual(pRandomCommand.iExecutedIndex, 0);
            Assert.IsTrue(Math.Abs(pTimer.Elapsed.TotalSeconds - fRandomWait) < 0.02f);
        }

        private static List<DummyCommand> Create_DummyCommands()
        {
            DummyCommand.DoReset_ExecutedIndex();

            System.Collections.Generic.List<DummyCommand> listCommand = new System.Collections.Generic.List<DummyCommand>();
            for (int i = 0; i < 10; i++)
                listCommand.Add(new DummyCommand(i));

            listCommand.ForEach(p => Assert.AreEqual(p.iExecutedIndex, 0));
            return listCommand;
        }
    }
}