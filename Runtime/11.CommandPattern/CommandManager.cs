#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-02-17
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity_Pattern
{
    public enum ECommandState
    {
        /// <summary>
        /// 일반적인 커맨드 재생
        /// </summary>
        None,

        /// <summary>
        /// 실행 취소로 커맨드 재생
        /// </summary>
        Undo,

        /// <summary>
        /// 다시 되돌리기로 커맨드 재생
        /// </summary>
        Redo,
    }

    public enum EWhere
    {
        OnInit,
        OnExecute_Command,
        OnExecute_UndoCommand,
        OnExecute_RedoCommand,
    }

    public interface ICommandAble
    {
        Task DoInitCommand_Async(IReadOnlyList<CancellationToken> listCancelToken);
        Task DoExecuteCommand_Async(ECommandState eCommandState, IReadOnlyList<CancellationToken> listCancelToken);
        void OnError(EWhere eWhere, Exception pException, out bool bIsPossible_NextStep);
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommandManager<TCommand>
        where TCommand : ICommandAble
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EState
        {
            OnInit,
            OnExecuteCommand,
            OnException,
            OnCancel,
            OnFinish,
        }

        /* public - Field declaration               */

        public struct OnChangeStateMsg
        {
            public EState eState;
            public IReadOnlyList<TCommand> listCommands;

            public OnChangeStateMsg(EState eState)
            {
                this.eState = eState;
                listCommands = null;
            }

            public OnChangeStateMsg(EState eState, IReadOnlyList<TCommand> listCommands)
            {
                this.eState = eState;
                this.listCommands = listCommands;
            }
        }


        public ObservableCollection<OnChangeStateMsg> OnChangeState { get; private set; } = new ObservableCollection<OnChangeStateMsg>();

        /* protected & private - Field declaration  */

        private List<TCommand> _listExecuteCommand = new List<TCommand>();
        List<CancellationToken> _listCancelToken = new List<CancellationToken>();
        CancellationTokenSource _pToken_OnCancel;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public async Task DoExecuteCommand(params TCommand[] arrCommand)
        {
            InitCancelToken();
            _listExecuteCommand.Clear();
            _listExecuteCommand.AddRange(arrCommand);

            await ExecuteCommands();
        }

        public async Task DoExecuteCommand(IEnumerable<TCommand> arrCommand)
        {
            InitCancelToken();
            _listExecuteCommand.Clear();
            _listExecuteCommand.AddRange(arrCommand);

            await ExecuteCommands();
        }

        /// <summary>
        /// 현재 실행 중/예정인 모든 커맨드를 취소합니다.
        /// </summary>
        public void DoCancel_AllCommand()
        {
            _pToken_OnCancel.Cancel();
        }


        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private async Task<bool> OnInitCommands()
        {
            for (int i = 0; i < _listExecuteCommand.Count; i++)
            {
                TCommand pCommand = _listExecuteCommand[i];

                // 커맨드 실행 할 때마다 Exception 핸들링
                try
                {
                    await pCommand.DoInitCommand_Async(_listCancelToken);
                }
                catch (OperationCanceledException OnCancel_Command)
                {
                    // 로직 Init 중에 Cancel이 오면 그냥 return
                    return false;
                }
                catch (Exception OnError)
                {
                    // Error를 Command에 맡기고, 다음 스텝이 실행 가능한지 체크 후
                    bool bPossible_NextStep = false;
                    pCommand.OnError(EWhere.OnInit, OnError, out bPossible_NextStep);


                    // 다음 스텝 실행이 불가능하면
                    if (bPossible_NextStep == false)
                        return false;
                }
            }

            return true;
        }

        private async Task ExecuteCommands()
        {
            if (await OnInitCommands() == false)
                return;


            try
            {
                for (int i = 0; i < _listExecuteCommand.Count; i++)
                {
                    await _listExecuteCommand[i].DoExecuteCommand_Async(ECommandState.None, _listCancelToken);
                }
            }
            catch (OperationCanceledException OnCancel_Command)
            {
            }
            catch (Exception OnError)
            {
            }
        }

        private void InitCancelToken()
        {
            _pToken_OnCancel = new CancellationTokenSource();
            _listCancelToken.Clear();
            _listCancelToken.Add(_pToken_OnCancel.Token);
        }

        #endregion Private
    }
}