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

    public interface IFontdata
    {
        SystemLanguage eLanguage { get; }
        Font pFontFile { get; }
    }

    public class FontDataDefault : IFontdata
    {
        public SystemLanguage eLanguage { get; private set; }

        public Font pFontFile { get; private set; }

        public FontDataDefault(SystemLanguage eLanguage, Font pFontFile)
        {
            this.eLanguage = eLanguage; this.pFontFile = pFontFile;
        }
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
        public ObservableCollection<Font> OnSetFont { get; private set; } = new ObservableCollection<Font>();

        public SystemLanguage eLanguage_Current { get; private set; } = SystemLanguage.Korean;

        /* protected & private - Field declaration         */

        Dictionary<string, ILanguageData> _mapLanguageData_KeyIs_LanguageKey = new Dictionary<string, ILanguageData>();
        Dictionary<SystemLanguage, Font> _mapFontData = new Dictionary<SystemLanguage, Font>();


        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoSetLanguage(SystemLanguage eLanguage)
        {
            eLanguage_Current = eLanguage;
            OnSetLanguage.DoNotify(eLanguage);

            if (_mapFontData.ContainsKey(eLanguage))
                OnSetFont.DoNotify(_mapFontData[eLanguage]);
        }

        public void DoInit_LanguageData<T>(T[] arrData)
            where T : ILanguageData
        {
            _mapLanguageData_KeyIs_LanguageKey = arrData.ToDictionary(p => p.strLanguageKey, p => (ILanguageData)p);
        }

        public void DoInit_FontData<T>(T[] arrData)
            where T : IFontdata
        {
            _mapFontData = arrData.ToDictionary(p => p.eLanguage, p => p.pFontFile);
        }

        public string GetText(string strLanguageKey)
        {
            ILanguageData pData;
            if(_mapLanguageData_KeyIs_LanguageKey.TryGetValue(strLanguageKey, out pData) == false)
            {
                Debug.LogError($"Not Found LangaugeKey : \"{strLanguageKey}\"");
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
            IEnumerable<string> arrMatchKey = _mapLanguageData_KeyIs_LanguageKey.Keys.Where(p => p.StartsWith(strLanguageKey_StartWidth));
            int iRandomIndex = Random.Range(0, arrMatchKey.Count());

            return _mapLanguageData_KeyIs_LanguageKey[arrMatchKey.ElementAt(iRandomIndex)].GetLocalText(eLanguage_Current);
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