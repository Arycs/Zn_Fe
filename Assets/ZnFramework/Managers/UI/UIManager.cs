using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;

namespace ZnFramework
{
    public class UIManager
    {
        private Dictionary<byte, UIGroup> m_UIGroupDic;

        /// <summary>
        /// 标准分辨率比值
        /// </summary>
        private float m_StandardScreen = 0;

        /// <summary>
        /// 当前分辨率比值
        /// </summary>
        private float m_CurrScreen = 0;

        private UILayer m_UILayer;

        private UIPool m_UIPool;

        private float UIClearInterval { get; set; }

        /// <summary>
        /// UI回池后过期时间
        /// </summary>
        public float UIExpire { get; private set; }

        public int UIPoolMaxCount { get; private set; }

        private float m_NextRunTime = 0f;

        /// <summary>
        /// 已经打开的UI链表
        /// </summary>
        private LinkedList<UIFormBase> m_OpenUIFormList;

        /// <summary>
        /// 反切UI栈
        /// </summary>
        private Stack<UIFormBase> m_ReverseChangeUIStack;

        public UIManager()
        {
            m_UILayer = new UILayer();
            m_UILayer.Init((GameEntry.Instance.UIGroups));
            m_OpenUIFormList = new LinkedList<UIFormBase>();
            m_UIPool = new UIPool();
            m_UIGroupDic = new Dictionary<byte, UIGroup>();
            m_ReverseChangeUIStack = new Stack<UIFormBase>();
        }

        public void Init()
        {
            UIPoolMaxCount =
                GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.UI_PoolMaxCount, GameEntry.CurrDeviceGrade);
            UIExpire = GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.UI_Exprie, GameEntry.CurrDeviceGrade);
            UIClearInterval =
                GameEntry.ParamsSettings.GetGradeParamData(ConstDefine.UI_ClearInterval, GameEntry.CurrDeviceGrade);

            m_StandardScreen = GameEntry.Instance.m_StandardWidth / (float) GameEntry.Instance.m_StandardHeight;
            m_CurrScreen = Screen.width / (float) Screen.height;
            NormalFormCanvasScaler();

            var len = GameEntry.Instance.UIGroups.Length;
            for (int i = 0; i < len; i++)
            {
                var group = GameEntry.Instance.UIGroups[i];
                m_UIGroupDic[group.Id] = group;
            }

            m_UILayer.Init(GameEntry.Instance.UIGroups);
        }

        public void OnUpdate()
        {
            if (!(Time.time > m_NextRunTime + UIClearInterval)) return;
            m_NextRunTime = Time.time;
            //释放 UI对象池
            m_UIPool.CheckClear();
        }

        public void Dispose()
        {
            //Debug.Log("UI组件关闭");
        }
        
        #region OpenUIForm 打开UI窗体

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormId"></param>
        /// <param name="userData"></param>
        /// <param name="onOpen"></param>
        public void OpenUIForm(int uiFormId, object userData = null, Action<UIFormBase> onOpen = null)
        {
            //1. 读表
            var entity = GameEntry.DataTable.Sys_UIFormDBModel.Get(uiFormId);
            if (entity == null)
            {
                GameEntry.LogError("表格中没有对应窗体数据 : " + uiFormId);
                return;
            }

            if (!entity.CanMulit && IsExists(uiFormId))
            {
                return;
            }

            var formBase = GameEntry.UI.Dequeue(uiFormId);
            if (formBase == null)
            {
                //TODO : 异步加载UI需要时间 此处需要处理过滤加载中的UI
                var assetPath = string.Empty;
                switch (GameEntry.Localization.CurrLanguage)
                {
                    case ZnLanguage.Chinese:
                        assetPath = entity.AssetPath_Chinese;
                        break;
                    case ZnLanguage.English :
                        assetPath = entity.AssetPath_English;
                        break;
                }

                LoadUIAsset(assetPath, resourceEntity =>
                {
                    var uiObj = Object.Instantiate((Object) resourceEntity.Target) as GameObject;
                    //把克隆出的资源 加入实例资源池
                    GameEntry.Pool.RegisterInstanceResource(uiObj.GetInstanceID(), resourceEntity);
                    
                    uiObj.transform.SetParent(GameEntry.UI.GetUIGroup(entity.UIGroupId).Group,false);
                    uiObj.transform.localPosition = Vector3.zero;
                    uiObj.transform.localScale = Vector3.one;
                    formBase = uiObj.GetComponent<UIFormBase>();
                    formBase.Init(uiFormId, entity.UIGroupId, entity.DisableUILayer == 1, entity.IsLock == 1, userData);
                    m_OpenUIFormList.AddLast(formBase);

                    OpenUI(entity, formBase, onOpen);
                });
            }
            else
            {
                formBase.Open(userData);
                m_OpenUIFormList.AddLast(formBase);
                ShowUI(formBase);
                OpenUI(entity, formBase, onOpen);
            }

            m_UIPool.CheckByOpenUI();
        }

        #endregion

        #region OpenUI 打开UI

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="sys_UIFormEntity"></param>
        /// <param name="formBase"></param>
        /// <param name="onOpen"></param>
        private void OpenUI(DTSys_UIFormEntity sys_UIFormEntity, UIFormBase formBase, Action<UIFormBase> onOpen)
        {
            var uiFormShowMode = (UIFormShowMode) sys_UIFormEntity.ShowMode;
            if (uiFormShowMode == UIFormShowMode.ReverseChange)
            {
                //如果之前栈里有UI
                if (m_ReverseChangeUIStack.Count > 0)
                {
                    var topUIForm = m_ReverseChangeUIStack.Peek();

                    //禁用 冻结
                    HideUI(topUIForm);
                }

                m_ReverseChangeUIStack.Push(formBase);
            }

            onOpen?.Invoke(formBase);
        }

        #endregion

        #region ShowUI 显示/激活一个UI

        /// <summary>
        /// 显示/激活一个UI
        /// </summary>
        /// <param name="uiFormBase"></param>
        private void ShowUI(UIFormBase uiFormBase)
        {
            uiFormBase.IsActive = true;
            uiFormBase.currCanvas.enabled = true;
            uiFormBase.gameObject.layer = 5;
        }

        #endregion

        #region HideUI 隐藏/冻结一个UI

        /// <summary>
        /// 隐藏/冻结一个UI
        /// </summary>
        /// <param name="uiFormBase"></param>
        private void HideUI(UIFormBase uiFormBase)
        {
            uiFormBase.IsActive = false;
            uiFormBase.currCanvas.enabled = false;
            uiFormBase.gameObject.layer = 0;
        }

        #endregion

        #region CloseUIForm 关闭UI窗口

        /// <summary>
        /// 关闭UI窗口
        /// </summary>
        /// <param name="uiFormBase">UI界面</param>
        public void CloseUIForm(UIFormBase uiFormBase)
        {
            m_OpenUIFormList.Remove(uiFormBase);
            uiFormBase.ToClose();

            //判断UI类型
            var entity = GameEntry.DataTable.Sys_UIFormDBModel.Get(uiFormBase.UIFormId);
            if (entity == null)
            {
                GameEntry.LogError(uiFormBase.UIFormId + "对应的UI窗体不存在");
            }

            HideUI(uiFormBase);

            var uiFormShowMode = (UIFormShowMode) entity.ShowMode;
            if (uiFormShowMode != UIFormShowMode.ReverseChange) return;
            m_ReverseChangeUIStack.Pop();
            if (m_ReverseChangeUIStack.Count <= 0) return;
            var topForms = m_ReverseChangeUIStack.Peek();
            ShowUI(topForms);
        }

        /// <summary>
        /// 关闭UI窗口
        /// </summary>
        /// <param name="uiFormId">UI窗口ID</param>
        public void CloseUIForm(int uiFormId)
        {
            for (var curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.UIFormId != uiFormId) continue;
                CloseUIForm(curr.Value);
                break;
            }
        }

        #endregion

        #region CloseUIFormByInstanceID 通过窗口物体实例ID关闭UI

        /// <summary>
        /// 通过窗口物体实例ID关闭UI
        /// </summary>
        /// <param name="instanceID">UI窗口物体的实例ID</param>
        private void CloseUIFormByInstanceID(int instanceID)
        {
            for (var curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.gameObject.GetInstanceID() != instanceID) continue;
                CloseUIForm(curr.Value);
                break;
            }
        }

        #endregion

        #region UI适配

        /// <summary>
        /// LoadingForm适配缩放
        /// </summary>
        public void UILoadingFormCanvasScaler()
        {
            if (m_CurrScreen > m_StandardScreen)
            {
                //分辨率大于标准分辨率 则设置为0
                GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = 0;
            }
            else
            {
                //分辨率小于标准分辨率, 则用标准减去当前分辨率
                GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = m_CurrScreen - m_StandardScreen;
            }
        }

        /// <summary>
        /// FullForm适配缩放为1
        /// </summary>
        public void FullFormCanvasScaler()
        {
            GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = 1;
        }

        private void NormalFormCanvasScaler()
        {
            GameEntry.Instance.UIRootCanvasScaler.matchWidthOrHeight = m_CurrScreen >= m_StandardScreen ? 1 : 0;
        }

        #endregion

        #region GetUIGroup

        /// <summary>
        /// 根据UI分组编号,获取UI分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UIGroup GetUIGroup(byte id)
        {
            m_UIGroupDic.TryGetValue(id, out var group);
            return group;
        }

        #endregion

        #region SetSortingOrder 设置层级

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="uiFormBase"></param>
        /// <param name="isAdd"></param>
        public void SetSortingOrder(UIFormBase uiFormBase, bool isAdd)
        {
            m_UILayer.SetSortingOrder(uiFormBase, isAdd);
        }

        #endregion

        #region Dequeue 从UI对象池中获取UI

        /// <summary>
        /// 从UI对象池中获取UI
        /// </summary>
        /// <param name="uiFormId"></param>
        /// <returns></returns>
        private UIFormBase Dequeue(int uiFormId)
        {
            return m_UIPool.Dequeue(uiFormId);
        }

        #endregion

        #region Enqueue 回到UI对象池

        /// <summary>
        /// 回到UI对象池
        /// </summary>
        /// <param name="uiFormBase"></param>
        public void Enqueue(UIFormBase uiFormBase)
        {
            m_UIPool.Enqueue(uiFormBase);
        }

        #endregion

        #region IsExists 判断UI是否打开

        /// <summary>
        /// 判断UI是否打开
        /// </summary>
        /// <param name="uiFormId"></param>
        /// <returns></returns>
        private bool IsExists(int uiFormId)
        {
            for (var curr = m_OpenUIFormList.First; curr != null; curr = curr.Next)
            {
                if (curr.Value.UIFormId == uiFormId)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
        
        #region LoadUIAsset 加载UI资源

        /// <summary>
        /// 加载UI资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="onComplete"></param>
        private void LoadUIAsset(string assetPath, Action<ResourceEntity> onComplete)
        {
            GameEntry.Resource.ResourceLoaderManager.LoadMainAsset(AssetCategory.UIPrefab,
                $"Assets/Download/UI/UIPrefab/{assetPath}.prefab",
                resourceEntity => { onComplete?.Invoke(resourceEntity); });
        }

        #endregion
    }
}