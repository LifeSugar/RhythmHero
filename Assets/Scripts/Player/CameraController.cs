using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace rhythmhero
{
    public class CameraController : MonoBehaviour
    {
        [Header("Stats")]
        public float followSpeed = 20f;        // 相机跟随目标的速度
        public float mouseSpeed = 2f;          // 鼠标控制相机旋转的速度
        public float turnSmoothing = 0.1f;     // 相机平滑旋转的系数
        public float minAngle = -20f;          // 垂直旋转的最小角度
        public float maxAngle = 35f;           // 垂直旋转的最大角度
        public float defaultDistance;          // 默认距离
        public Vector3 offset = new Vector3(0, 1.3f, 0);
        public float lockOffset = 0.5f;

        [Header("MoveStat")]
        public Vector3 targetDir;              // 目标方向向量
        public float lookAngle;                // 水平旋转角度
        public float tiltAngle;                // 垂直旋转角度

        [HideInInspector] public Transform pivot;    // 相机的俯仰轴 (X 轴转动)
        [HideInInspector] public Transform camTrans; // 主相机的 Transform
        private Transform camRoot;                   // 相机根，用于水平旋转 (Y 轴)
        private PlayerState state;
        private Transform followTarget;              // 跟随目标

        private float smoothX, smoothY;
        private float smoothXvelocity, smoothYvelocity;

        // ========== 新增 Focus 功能 ==========
        [Header("Focus Settings")]
        [Tooltip("焦点目标，摄像机会转向这个 Transform")]
        public Transform focusTarget;
        [Tooltip("在焦点期间需要禁用的脚本（例如玩家输入、移动脚本等）")]
        public MonoBehaviour[] inputScripts;

        // 临时存储视角
        private float cachedLookAngle;
        private float cachedTiltAngle;
        public bool isFocusing = false;

        // 过渡与停顿时长
        private const float tweenDuration = 1f;
        private const float holdDuration  = 1f;
        // ======================================

        // 调试用
        private Vector3 debugCandidatePos;
        private float debugCameraCollisionRadius;
        private Vector3 debugRayStart;
        private Vector3 debugRayDir;
        private float debugRayDistance;

        public static CameraController instance;

        private void Awake()
        {
            if (instance != null && instance != this)
                Debug.LogWarning("More than one instance of CameraController found!");
            instance = this;
        }

        public void Init(PlayerState playerState)
        {
            state = playerState;
            followTarget = playerState.transform;

            // 获取主相机及其层级
            camTrans = Camera.main.transform;
            pivot    = camTrans.parent;        // 相机的父物体：俯仰轴
            camRoot  = pivot.parent;           // 俯仰轴的父物体：水平旋转轴

            defaultDistance = new Vector3(0f, offset.z, 0f).magnitude;
            pivot.localPosition = offset;
        }

        private void Update()
        {
            if (isFocusing) return;

            float dt = Time.deltaTime;
            float h  = Input.GetAxis("Mouse X");
            float v  = Input.GetAxis("Mouse Y");
            HandleRotations(dt, v, h, mouseSpeed);
        }

        private void FixedUpdate()
        {
            HandleCameraCollision(Time.fixedDeltaTime);
        }

        private void HandleRotations(float delta, float v, float h, float speed)
        {
            if (turnSmoothing > 0f)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            // 垂直旋转（俯仰）
            tiltAngle -= smoothY * speed;
            tiltAngle  = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

            // 水平旋转（偏航）
            lookAngle += smoothX * speed;
            camRoot.rotation = Quaternion.Euler(0f, lookAngle, 0f);
        }

        private void HandleCameraCollision(float delta)
        {
            Vector3 follow = followTarget.position + Vector3.up * offset.y;
            Vector3 desiredCamPos = follow - camRoot.forward * defaultDistance;
            Vector3 rayDir = (desiredCamPos - follow).normalized;

            float cameraCollisionRadius = 0.3f;
            float minDistance = 0.15f;
            int layerMask = 1 << 28;
            float finalDistance = defaultDistance;

            if (Physics.Raycast(follow, rayDir, out RaycastHit hit, defaultDistance, layerMask))
                finalDistance = hit.distance;

            Vector3 candidatePos = follow + rayDir * finalDistance;
            debugCandidatePos = candidatePos;
            debugCameraCollisionRadius = cameraCollisionRadius;
            debugRayStart = follow;
            debugRayDir = rayDir;
            debugRayDistance = finalDistance;

            bool colliding = Physics.CheckSphere(candidatePos, cameraCollisionRadius, layerMask);
            while (colliding && finalDistance > minDistance)
            {
                finalDistance = Mathf.Max(finalDistance - 0.05f, minDistance);
                candidatePos = follow + rayDir * finalDistance;
                colliding = Physics.CheckSphere(candidatePos, cameraCollisionRadius, layerMask);
                debugCandidatePos = candidatePos;
                debugRayDistance = finalDistance;
            }

            camTrans.position = Vector3.Lerp(camTrans.position, candidatePos, delta * followSpeed);
        }

        /// <summary>
        /// 公有方法：禁用玩家输入，用 DOTween 在 1s 内平滑转向 focusTarget，停顿 1s 再平滑返回
        /// </summary>
        public void FocusOnTarget()
        {
            if (focusTarget == null || isFocusing) return;
            isFocusing = true;

            // 1. 禁用所有指定的输入脚本
            foreach (var script in inputScripts)
                if (script != null) script.enabled = false;

            // 2. 记录当前视角
            cachedLookAngle = lookAngle;
            cachedTiltAngle = tiltAngle;

            // 3. 计算目标朝向
            Vector3 dir = focusTarget.position - pivot.position;
            Quaternion worldRot = Quaternion.LookRotation(dir);
            float targetY = worldRot.eulerAngles.y;
            float targetX = Mathf.Clamp(
                // 把世界空间的俯仰转换成本地俯仰
                Mathf.DeltaAngle(0, worldRot.eulerAngles.x),
                minAngle, maxAngle
            );

            // 4. 构造 DOTween 序列
            Sequence seq = DOTween.Sequence();
            // 平滑到目标视角
            seq.Append(camRoot.DORotate(new Vector3(0f, targetY, 0f), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join (pivot.DOLocalRotate(new Vector3(targetX, 0f, 0f), tweenDuration).SetEase(Ease.InOutSine));
            // 停顿
            seq.AppendInterval(holdDuration);
            // 平滑回原视角
            seq.Append(camRoot.DORotate(new Vector3(0f, cachedLookAngle, 0f), tweenDuration).SetEase(Ease.InOutSine));
            seq.Join (pivot.DOLocalRotate(new Vector3(cachedTiltAngle, 0f, 0f), tweenDuration).SetEase(Ease.InOutSine));
            // 完成后恢复
            seq.OnComplete(() =>
            {
                foreach (var script in inputScripts)
                    if (script != null) script.enabled = true;
                isFocusing = false;
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(debugCandidatePos, debugCameraCollisionRadius);
            Gizmos.DrawLine(debugRayStart, debugRayStart + debugRayDir * debugRayDistance);
        }
    }
}
