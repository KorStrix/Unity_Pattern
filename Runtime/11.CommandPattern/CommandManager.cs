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
        /// �Ϲ����� Ŀ�ǵ� ���
        /// </summary>
        None,

        /// <summary>
        /// ���� ��ҷ� Ŀ�ǵ� ���
        /// </summary>
        Undo,

        /// <summary>
        /// �ٽ� �ǵ������ Ŀ�ǵ� ���
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
        /// ���� ���� ��/������ ��� Ŀ�ǵ带 ����մϴ�.
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

                // Ŀ�ǵ� ���� �� ������ Exception �ڵ鸵
                try
                {
                    await pCommand.DoInitCommand_Async(_listCancelToken);
                }
                catch (OperationCanceledException OnCancel_Command)
                {
                    // ���� Init �߿� Cancel�� ���� �׳� return
                    return false;
                }
                catch (Exception OnError)
                {
                    // Error�� Command�� �ñ��, ���� ������ ���� �������� üũ ��
                    bool bPossible_NextStep = false;
                    pCommand.OnError(EWhere.OnInit, OnError, out bPossible_NextStep);


                    // ���� ���� ������ �Ұ����ϸ�
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