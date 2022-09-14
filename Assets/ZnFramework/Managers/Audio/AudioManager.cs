using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnFramework
{
    public class AudioManager
    {
        #region BGM

        /// <summary>
        /// 当前BGM的名称
        /// </summary>
        private string m_CurrBGMAudio;

        /// <summary>
        /// 当前BGM音量
        /// </summary>
        private float m_CurrBGMVolume;

        /// <summary>
        /// 当前BGM最大音量
        /// </summary>
        private float m_CurrBGMMaxVolume;
        
        #endregion
        
        public AudioManager()
        {
        }

        public void Init()
        {
            
        }
    }
}