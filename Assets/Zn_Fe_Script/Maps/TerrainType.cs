using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Zn_Fe.Maps
{
    /// <summary>
    /// 对应FE8 中 地形类型, 后续不断添加
    /// </summary>
    [Serializable]
    public enum TerrainType : byte
    {
        [LabelText("未定义")]
        None,
        [LabelText("平地")]
        Plain,
        [LabelText("湿地(沼泽)")]
        [Obsolete("Game Not Used", true)]
        Swamp,
        [LabelText("道路")]
        Road,
        [LabelText("村庄")]
        Village,
    }
}