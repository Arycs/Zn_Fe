using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ZnFramework
{
    public class GameEntry : MonoBehaviour
    {
        public static GameEntry Instance;

        [FoldoutGroup("ParamsSettings")] [SerializeField]
        private ParamsSettings.DeviceGrade m_CurrDeviceGrade;

        [FoldoutGroup("ParamsSettings")] [SerializeField]
        private ParamsSettings m_ParamesSettings;

        [FoldoutGroup("ParamsSettings")] [SerializeField]
        private ZnLanguage m_CurrLanguage;

        [FoldoutGroup("GameObjectPool")] [Header("游戏物体对象池父物体")]
        public Transform PoolParent;

        [Header("标准分辨率的宽度")] [FoldoutGroup("UIGroup")] [SerializeField]
        public int m_StandardWidth = 1280;

        [Header("标准分辨率的高度")] [FoldoutGroup("UIGroup")] [SerializeField]
        public int m_StandardHeight = 720;

        [Header("UI摄像机")] [FoldoutGroup("UIGroup")] [SerializeField]
        public Camera UICamera;

        [Header("根画布")] [FoldoutGroup("UIGroup")] [SerializeField]
        public Canvas m_UIootCanvas;

        [Header("根画布的缩放")] [FoldoutGroup("UIGroup")] [SerializeField]
        public CanvasScaler UIRootCanvasScaler;

        [Header("UI分组")] [FoldoutGroup("UIGroup")] [SerializeField]
        public UIGroup[] UIGroups;


        /// <summary>
        /// 全局参数设置
        /// </summary>
        public static ParamsSettings ParamsSettings { get; private set; }

        /// <summary>
        /// 当前设备等级
        /// </summary>
        public static ParamsSettings.DeviceGrade CurrDeviceGrade { get; private set; }

        /// <summary>
        /// 游戏物体对象池的分组
        /// </summary>
        [SerializeField] [FoldoutGroup("GameObjectPool")]
        public GameObjectPoolEntity[] GameObjectPoolGroups;

        #region 基础组件

        public static AudioManager Audio { get; private set; }
        public static DataManager Data { get; private set; }
        public static LocalizationManager Localization { get; private set; }
        public static DataTableManager DataTable { get; private set; }
        public static EventManager Event { get; private set; }
        public static FsmManager Fsm { get; private set; }
        public static GameObjManager GameObj { get; private set; }
        public static PoolManager Pool { get; private set; }
        public static ProcedureManager Procedure { get; private set; }
        public static ZnSceneManager Scene { get; private set; }
        public static TimeManager Time { get; private set; }
        public static UIManager UI { get; private set; }
        public static AddressableManager Resource { get; private set; }
        public static TaskManager Task { get; private set; }

        #endregion

        public void Awake()
        {
            Instance = this;
            CurrDeviceGrade = m_CurrDeviceGrade;
            ParamsSettings = m_ParamesSettings;
        }

        private void Start()
        {
            Event = new EventManager();
            Time = new TimeManager();
            Pool = new PoolManager();
            Fsm = new FsmManager();
            Procedure = new ProcedureManager();
            DataTable = new DataTableManager();

            Audio = new AudioManager();
            Data = new DataManager();
            Localization = new LocalizationManager();
            GameObj = new GameObjManager();
            Scene = new ZnSceneManager();
            UI = new UIManager();
            Resource = new AddressableManager();
            Task = new TaskManager();

            Pool.Init();
            Procedure.Init();
            UI.Init();
        }

        private void Update()
        {
            //循环更新组件
            for (LinkedListNode<IUpdateComponent> curr = m_UpdateComponent.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate();
            }

            Time.OnUpdate();
            Pool.OnUpdate();
            Procedure.OnUpdate();
            UI.OnUpdate();
            Scene.OnUpdate();
            Task.OnUpdate();
            Resource.OnUpdate();
        }

        private void OnDestroy()
        {
            Event.Dispose();
            Time.Dispose();
        }

        #region 更新组件管理

        /// <summary>
        /// 更新组件的列表
        /// </summary>
        private static readonly LinkedList<IUpdateComponent> m_UpdateComponent = new LinkedList<IUpdateComponent>();

        #region RegisterUpdateComponent 注册更新组件

        /// <summary>
        /// 注册更新组件
        /// </summary>
        /// <param name="component"></param>
        public static void RegisterUpdateComponent(IUpdateComponent component)
        {
            m_UpdateComponent.AddLast(component);
        }

        #endregion

        #region RemoveUpdateComponent 移除更新组件

        /// <summary>
        /// 移除更新组件
        /// </summary>
        /// <param name="component"></param>
        public static void RemoveUpdateComponent(IUpdateComponent component)
        {
            m_UpdateComponent.Remove(component);
        }

        #endregion

        #endregion

        #region 打印日志信息

        /// <summary>
        /// Editor 打印日志
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInfo(LogCategory category, string message, params object[] args)
        {
            switch (category)
            {
                case LogCategory.Normal:
                    Debug.Log(args.Length == 0 ? message : string.Format(message, args));
                    break;
                case LogCategory.Event:
#if DEBUG_EVENT
                    Debug.Log(
                        $"Event : <color=#DEAB8A>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.Time:
#if DEBUG_TIME
                    Debug.Log(
                        $"Time : <color=#ACE44A>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.Pool:
#if DEBUG_POOL
                    Debug.Log(
                        $"Pool : <color=#ACE44A>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.Fsm:
#if DEBUG_FSM
                    Debug.Log(
                        $"Fsm : <color=#FFD700>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.Procedure:
#if DEBUG_PROCEDURE
                    Debug.Log(
                        $"Procedure : <color=#FFD700>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.UI:
#if DEBUG_UI
                    Debug.Log(
                        $"UI : <color=#FFD700>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.GameLogic:
#if DEBUG_GameLogic
                    Debug.Log(
                        $"GameLogic : <color=#FFD700>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.Scene:
#if DEBUG_Scene
                    Debug.Log(
                        $"Scene : <color=#FFD700>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                case LogCategory.Resource:
#if DEBUG_Resource
                    Debug.Log(
                        $"Resource : <color=#DEAB8A>{(args.Length == 0 ? message : string.Format(message, args))}</color>");
                    break;
#endif
                default:
                    break;
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(string message, params object[] args)
        {
            Debug.LogError("ERROR : ====>" + (args.Length == 0 ? message : string.Format(message, args)));
        }

        #endregion
    }
}