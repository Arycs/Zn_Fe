using System;
using Sirenix.OdinInspector;
using UnityEngine;
using ZnFramework;

[CreateAssetMenu]
public class ParamsSettings : ScriptableObject
{
    [BoxGroup("InitUrl")] public string WebAccountUrl;

    [BoxGroup("InitUrl")] public string TestWebAccountUrl;

    [BoxGroup("InitUrl")] public bool IsTest;

    [BoxGroup("GeneralParams")] [TableList(ShowIndexLabels = true, AlwaysExpanded = true)] [HideLabel]
    public GeneralParamData[] GeneralParams;

    [BoxGroup("GradeParams")] [TableList(ShowIndexLabels = true, AlwaysExpanded = true)] [HideLabel]
    public GradeParamData[] GradeParamDatas;
    
    private int m_LenGradeParams = 0;

    /// <summary>
    /// 根据Key和设备等级获取参数
    /// </summary>
    /// <param name="key"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    public int GetGradeParamData(string key, DeviceGrade grade)
    {
        m_LenGradeParams = GradeParamDatas.Length;
        for (var i = 0; i < m_LenGradeParams; i++)
        {
            var gradeParamData = GradeParamDatas[i];
            if (gradeParamData.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
            {
                return gradeParamData.GetValueByGrade(grade);
            }
        }

        GameEntry.LogError($"GetGradeParamData Fail Key = {key}");
        return 0;
    }

    /// <summary>
    /// 常规参数
    /// </summary>
    [Serializable]
    public class GeneralParamData
    {
        [TableColumnWidth(160, Resizable = false)]
        public string Key;

        public int Value;
    }

    /// <summary>
    /// 设备等级
    /// </summary>
    public enum DeviceGrade
    {
        Low = 0,
        Middle = 1,
        High = 2
    }

    /// <summary>
    /// 等级参数数据
    /// </summary>
    [Serializable]
    public class GradeParamData
    {
        [TableColumnWidth(160, Resizable = false)]
        public string Key;

        public int LowValue;

        public int MiddleValue;

        public int HighValue;

        public int GetValueByGrade(DeviceGrade grade)
        {
            switch (grade)
            {
                case DeviceGrade.Middle:
                    return MiddleValue;
                case DeviceGrade.High:
                    return HighValue;
                default:
                    return LowValue;
            }
        }
    }
}