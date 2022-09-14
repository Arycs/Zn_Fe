using System;
using UnityEngine;

namespace ZnFramework
{
    public enum ZnLanguage
    {
        /// <summary>
        /// 中文
        /// </summary>
        Chinese = 0,
        /// <summary>
        /// 英文
        /// </summary>
        English = 1
    }

    public class LocalizationManager : IDisposable
    {
        private ZnLanguage m_CurrLanguage;

        /// <summary>
        /// 当前语言(要和本地化表的语言字段一致)
        /// </summary>
        public ZnLanguage CurrLanguage => m_CurrLanguage;

        private LocalizationManager m_LocalizationManager;

        public LocalizationManager()
        {
            switch (Application.systemLanguage)
            {
                default:
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    m_CurrLanguage = ZnLanguage.Chinese;
                    break;
                case SystemLanguage.English:
                    m_CurrLanguage = ZnLanguage.English;
                    break;
            }
        }

        /// <summary>
        /// 获取本地化文本内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetString(string key,params object[] args)
        {
            return GameEntry.DataTable.LocalizationDBModel.LocalizationDic.TryGetValue(key,out var value) ? string.Format(value,args) : null;
        }

        public void Init()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}