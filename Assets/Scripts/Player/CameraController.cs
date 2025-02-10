using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rhythmhero
{
    public class CameraController : MonoBehaviour
    {
        [Header("Stats")] public float followSpeed = 20; // 相机跟随目标的速度
        public float mouseSpeed = 2; // 鼠标控制相机旋转的速度
        public float turnSmoothing = .1f; // 相机平滑旋转的系数
        public float minAngle = -20; // 垂直旋转的最小角度
        public float maxAngle = 35; // 垂直旋转的最大角度
        public float defaultDistance;
        public Vector3 offset = new Vector3(0, 1.3f, 0);
        public float lockOffset = 0.5f;
        
        [Header("MoveStat")] public Vector3 targetDir; // 目标方向向量
        public float lookAngle; // 水平旋转角度
        public float tiltAngle; // 垂直旋转角度
        
        [HideInInspector] public Transform pivot; // 相机的旋转轴
        [HideInInspector] public Transform camTrans; // 相机的Transform
        PlayerState state;
        Transform followTarget; //跟随目标
        
        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;


        bool usedRightAxis;

        bool changeTargetLeft;
        bool changeTargetRight;
        
        // 用于调试的可视化数据
        private Vector3 debugCandidatePos;
        private float debugCameraCollisionRadius;
        private Vector3 debugRayStart;
        private Vector3 debugRayDir;
        private float debugRayDistance;
        
        public static CameraController instance;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("More than one instance of CameraController found!");
            }
            instance = this;
        }


        public void Init(PlayerState playerState)
        {
            state = playerState;
            followTarget = playerState.transform;
            camTrans = Camera.main.transform; // 获取主相机的Transform
            pivot = camTrans.parent; // 设置pivot为相机的父对象
            defaultDistance = new Vector3(0, offset.z, 0).magnitude;
            pivot.localPosition = offset;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            float h = Input.GetAxis("Mouse X"); // 获取水平输入
            float v = Input.GetAxis("Mouse Y"); // 获取垂直输入
            float targetSpeed = mouseSpeed; // 设定相机移动速度
            HandleRotations(dt, v, h, targetSpeed); // 调用旋转处理方法
            
        }


        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            // 垂直旋转
            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            // 水平旋转
            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
        
        void HandleCameraCollision(float d)
        {
            // follow为跟随点，即玩家角色头部(或指定offset位置)
            Vector3 follow = followTarget.position + new Vector3(0, offset.y, 0);

            // 计算期望的相机位置。默认距离defaultDistance表示相机应从follow点后退多少距离。
            // desiredCamPos代表理想情况下摄像机应放置的位置（没有碰撞干扰的情况下）
            Vector3 desiredCamPos = follow - transform.forward * defaultDistance;

            // 根据期望位置与follow点的关系，计算出一条从follow点到期望相机位置的方向向量rayDir
            Vector3 rayDir = (desiredCamPos - follow).normalized;

            // 定义相机的球体半径，用于球体碰撞检测
            float cameraCollisionRadius = 0.3f;

            // 定义相机最小距离，避免相机过于接近follow点，产生抖动或穿模感
            float minDistance = 0.15f;

            // layerMask用于指定检测哪些层级的碰撞体
            // 这里是 int layerMask = 1 << 28; 
            // 意思是只检测28层所在的物体(如摄像机可碰撞层)
            int layerMask = 1 << 28;

            // 初始化最终距离finalDistance为默认距离
            float finalDistance = defaultDistance;

            RaycastHit hit;
            // 首先使用Raycast沿rayDir方向从follow点发射一条射线，最大距离为defaultDistance
            // 如果射线检测到了碰撞物体，则说明期望的位置与之重叠或在其后方
            // 我们将finalDistance缩短到hit.distance，这样相机不会穿过障碍物
            if (Physics.Raycast(follow, rayDir, out hit, defaultDistance, layerMask))
            {
                finalDistance = hit.distance;
            }

            // 基于最终计算出的距离finalDistance，确定相机候选位置candidatePos
            Vector3 candidatePos = follow + rayDir * finalDistance;

            // 保存调试数据（用于 OnDrawGizmos 可视化）
            debugCandidatePos = candidatePos;
            debugCameraCollisionRadius = cameraCollisionRadius;
            debugRayStart = follow;
            debugRayDir = rayDir;
            debugRayDistance = finalDistance;

            // 使用 CheckSphere 来检测candidatePos位置处，半径为cameraCollisionRadius的球体
            // 是否与场景发生碰撞。如果有碰撞，说明即使距离缩短了，但考虑相机体积后仍在穿模
            bool isColliding = Physics.CheckSphere(candidatePos, cameraCollisionRadius, layerMask);

            // 如果球体检测仍有碰撞，则需要继续往回缩短距离，直到无碰撞或缩短到最小距离minDistance
            while (isColliding && finalDistance > minDistance)
            {
                // 每次向内收缩0.05f的距离
                finalDistance -= 0.05f;
                finalDistance = Mathf.Max(finalDistance, minDistance);

                // 根据新的距离重新计算candidatePos
                candidatePos = follow + rayDir * finalDistance;

                // 再次检测球体碰撞
                isColliding = Physics.CheckSphere(candidatePos, cameraCollisionRadius, layerMask);

                // 更新调试数据，以便在场景中查看最终结果
                debugCandidatePos = candidatePos;
                debugRayDistance = finalDistance;
            }

            // 将相机的位置平滑插值到最终确定的candidatePos
            // d为deltaTime，followSpeed为插值速度
            camTrans.position = Vector3.Lerp(camTrans.position, candidatePos, d * followSpeed);
        }
    }
}
