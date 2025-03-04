using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace rhythmhero.audio
{
    public class BGMManager : MonoBehaviour
    {
        // 单例模式，方便全局访问
        public static BGMManager instance { get; private set; }

        // 音乐事件引用列表
        public List<EventReference> bgms = new List<EventReference>();

        // FMOD 实例列表，对应 bgms
        public List<EventInstance> bgmInstances = new List<EventInstance>();
        
        //BGM Position
        public List<Transform> bgmTransforms = new List<Transform>();

        // 计时器
        public float timer;

        // 上一个触发 0.5s 和 2s 节拍的计数，用于防止重复触发
        private int lastHalfBeat = -1;
        private int lastFullBeat = -1;

        // 定义两个节拍事件，外部脚本可以订阅这些事件
        public event System.Action OneBeat;  // 每 0.5 秒触发
        public event System.Action FourBeat; // 每 2 秒触发
        
        //定义两个float辅助矫正动画
        public float checkerhalf;
        private float checkerfour;

        void Awake()
        {
            // 单例模式检查，防止场景中出现多个 BGMManager 实例
            if (instance != null)
            {
                Debug.LogWarning("More than one BGM manager in scene.");
                return;
            }
            instance = this;
        }

        void Start()
        {
            // 初始化 BGM 实例
            AudioManager.instance.InitializeBGMInstance();
        }

        void Update()
        {
            // 每帧检查节拍并触发对应的事件
            BeatsChecker();
            
        }

        /// <summary>
        /// 检查当前播放位置并触发节拍事件。
        /// 每 0.5 秒触发 OneBeat，每 2 秒触发 FourBeat。
        /// </summary>
        void BeatsChecker()
        {
            // 确保有 BGM 实例在播放
            if (bgmInstances.Count == 0) return;

            // 获取当前 BGM 的时间线位置（毫秒）
            bgmInstances[0].getTimelinePosition(out int currentPositionMs);

            //计算每拍进程（秒）
            checkerhalf = (currentPositionMs / 1000f) % 0.5f;
            checkerfour = (currentPositionMs / 1000f) % 2.0f;

            // 计算当前的 0.5 秒和 2 秒节拍计数
            int currentHalfBeat = currentPositionMs / 500;  // 每 0.5 秒一个节拍
            int currentFullBeat = currentPositionMs / 2000; // 每 2 秒一个节拍

            // 检查 0.5 秒节拍（OneBeat）
            if (currentHalfBeat != lastHalfBeat)
            {
                lastHalfBeat = currentHalfBeat;
                OneBeat?.Invoke(); // 触发 OneBeat 事件
            }

            // 检查 2 秒节拍（FourBeat）
            if (currentFullBeat != lastFullBeat)
            {
                lastFullBeat = currentFullBeat;
                FourBeat?.Invoke(); // 触发 FourBeat 事件
            }
        }
        
        //校正动画的方法
        public void SyncIdleAnimation(Animator animator, string idleStateName)
        {
            // if (checkerhalf >= 0f)
            // {
            //     // 计算动画起始时间
            //     float normalizedTime = checkerhalf;
            //
            //     // 播放 Idle 动画，并从 normalizedTime 开始
            //     animator.Play(idleStateName, 0, normalizedTime);
            //
            //     Debug.Log($"同步动画到 checkerhalf 时间点，起始时间为 {checkerhalf} 秒");
            // }
        }

    }
}
