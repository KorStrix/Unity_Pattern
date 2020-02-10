#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-09 오후 11:17:32
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity_Pattern
{
    public interface ILanguageData
    {
        string strLanguageKey { get; }
        string GetLocalText(SystemLanguage eSystemLanguage);
    }


    /// <summary>
    /// 
    /// </summary>
    public class LanguageManager : CSingletonDynamicMonoBase<LanguageManager>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public ObservableCollection<SystemLanguage> OnSetLanguage { get; private set; } = new ObservableCollection<SystemLanguage>();

        public SystemLanguage eLanguage_Current { get; private set; } = SystemLanguage.Korean;

        /* protected & private - Field declaration         */

        Dictionary<string, ILanguageData> _mapLanguageData = new Dictionary<string, ILanguageData>();

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoSetLanguage(SystemLanguage eLanguage)
        {
            eLanguage_Current = eLanguage;
            OnSetLanguage.DoNotify(eLanguage);
        }

        public void DoInitData<T>(IEnumerable<T> arrLanguageData)
            where T : ILanguageData
        {
            _mapLanguageData = arrLanguageData.ToDictionary(p => p.strLanguageKey, p => (ILanguageData)p);
        }

        public string GetText(string strLanguageKey)
        {
            ILanguageData pData;
            if(_mapLanguageData.TryGetValue(strLanguageKey, out pData) == false)
            {
                Debug.LogError($"Not Found LangaugeKey : {strLanguageKey}");
                return "Not Found";
            }

            return pData.GetLocalText(eLanguage_Current);
        }

        public string GetText_Format(string strLanguageKey, params object[] arrParam)
        {
            return string.Format(GetText(strLanguageKey), arrParam);
        }


        public string GetText_Random(string strLanguageKey_StartWidth)
        {
            IEnumerable<string> arrMatchKey = _mapLanguageData.Keys.Where(p => p.StartsWith(strLanguageKey_StartWidth));
            int iRandomIndex = Random.Range(0, arrMatchKey.Count());

            return _mapLanguageData[arrMatchKey.ElementAt(iRandomIndex)].GetLocalText(eLanguage_Current);
        }

        public string GetText_Format_Random(string strLanguageKey_StartWidth, params object[] arrParam)
        {
            return string.Format(GetText_Random(strLanguageKey_StartWidth), arrParam);
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void OnAwake()
        {
            base.OnAwake();

            DontDestroyOnLoad(gameObject);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private

    }
}