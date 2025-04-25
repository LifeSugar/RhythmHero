using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using rhythmhero.audio;

namespace rhythmhero
{
    public class Ghost : MonoBehaviour
    {
        #region 单例
        public static Ghost instance;
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Duplicate Ghost!!!");
            }
            instance = this;
        }
        #endregion

        /* ----------  音效 / 参数 ---------- */

        [Header("漂浮上来的音效，时长最好为 2s (Loop)")]
        public EventReference flowUpSound;          // 在 FMOD Studio 里把事件设成 Loop
        private EventInstance _flowUpInst;

        [Tooltip("判定移动的平方阈值（m²）")]
        public float moveSqrThreshold = 1e-6f;      // ≈1 mm 位移

        [Header("移动速度（m/s）")]
        public float moveSpeed = 4f;

        /* ----------  私有缓存 ---------- */

        private Vector3 _lastPos;
        private bool _isMoving;                     // 当前帧是否在移动
        private bool _wasMoving;                    // 上一帧是否在移动

        /* ----------  生命周期 ---------- */

        void Start()
        {
            _lastPos    = transform.position;
            
            _flowUpInst = AudioManager.instance.CreatEventInstance(flowUpSound);
        }

        void Update()
        {
            DetectMovement();
            HandleMoveSound();
        }

        void OnDisable()      // 或 OnDestroy
        {
            _flowUpInst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _flowUpInst.release();
        }

        /* ----------  移动检测 & 音效 ---------- */

        /// <summary>计算 _isMoving/_wasMoving</summary>
        void DetectMovement()
        {
            Vector3 curr = transform.position;
            _isMoving    = (curr - _lastPos).sqrMagnitude > moveSqrThreshold;
            _lastPos     = curr;
        }

        /// <summary>根据移动状态播放 / 停止音效</summary>
        void HandleMoveSound()
        {
            // 更新 3D 位置，让声音跟着 Ghost 走
            _flowUpInst.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            if (_isMoving && !_wasMoving)           // 刚开始移动
            {
                _flowUpInst.start();                // 事件本身是 Loop，所以持续播放
            }
            else if (!_isMoving && _wasMoving)      // 刚停下
            {
                _flowUpInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            _wasMoving = _isMoving;                 // 记录给下一帧用
        }


        /// <summary>小幽灵从井里飘起来</summary>
        public void FlowUpGhost()
        {
            float targetY  = transform.position.y + 3.1f;
            transform.DOMoveY(targetY, 2f)
                     .OnComplete(() => DialogueManager.instance.StartDialogue());
        }

        /// <summary>移动到目标点并朝向玩家</summary>
        public void MoveToTargetAndLookAt(Transform destination)
        {
            float distance = Vector3.Distance(transform.position, destination.position);
            float duration = distance / moveSpeed;

            transform.DOMove(destination.position, duration)
                     .SetEase(Ease.Linear)
                     .OnComplete(() =>
                     {
                         if (PlayerState.instance != null)
                         {
                             Vector3 dir = (PlayerState.instance.transform.position -
                                            transform.position).normalized;
                             if (dir != Vector3.zero)
                             {
                                 Quaternion rot = Quaternion.LookRotation(dir);
                                 transform.DORotateQuaternion(rot, 0.5f)
                                          .OnComplete(() => destination.gameObject.SetActive(true));
                             }
                         }
                     });
        }
    }
}
