using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace rhythmhero
{
    
    /// <summary>
    /// 这个脚本用来控制引导，我在这里使用一个特效
    /// </summary>
    public class ParticleMovement : MonoBehaviour
    {
        [Header("参考对象")]
        // 玩家以及目标位置的 Transform 引用，需要在 Inspector 指定
        public Transform player;

        public Transform posA;
        public Transform posB;
        public Transform posC;

        [Header("下降参数")]
        // 从玩家正上方出现的高度偏移
        public float heightOffset = 5f;

        // 下降目标为玩家位置加一个水平偏移，这样物体不会正好落在玩家脚下
        public Vector3 fallOffset = new Vector3(2f, 0f, 2f);

        [Header("等待检测参数")]
        // 判断玩家是否“接近”目标点的距离阈值
        public float arrivalThreshold = 1f;

        [Header("摇动参数")]
        // 摇动上下的幅度和时长
        public float shakeAmplitude = 0.5f;

        public float shakeDuration = 0.3f;

        // 保存当前的摇动 tween，便于后续停止
        private Tween shakeTween;

        void Start()
        {
            // 开始执行动画序列
            StartCoroutine(MovementSequence());
        }

        IEnumerator MovementSequence()
        {
            // 【阶段1】物体从玩家正上方出现，再下降到玩家附近
            Vector3 startPos = player.position + Vector3.up * heightOffset;
            transform.position = startPos;
            Vector3 fallTarget = player.position + fallOffset;
            yield return transform.DOMove(fallTarget, 1f).WaitForCompletion();

            // 【阶段2】物体围绕玩家旋转两圈（720度） - 用 DOPath 绘制圆形路径
            float orbitDuration = 3f;
            float totalDegrees = 810f; // 两圈半
            int segments = 16; // 路径分段数，分段数越多，曲线越平滑
            float orbitRadius = (fallTarget - player.position).magnitude; // 半径取初始位置与玩家的距离
            Vector3 orbitCenter = player.position + new Vector3(0f, 0.5f, 0f);
            Vector3[] path = new Vector3[segments + 1];

            for (int i = 0; i <= segments; i++)
            {
                float angle = totalDegrees * i / segments;
                float rad = angle * Mathf.Deg2Rad;
                // 以玩家位置为中心生成圆周上对应的坐标，保持物体当前高度不变
                Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
                path[i] = orbitCenter + offset;
            }

            // 使用 CatmullRom 曲线使运动更平滑
            yield return transform.DOPath(path, orbitDuration, PathType.CatmullRom)
                .SetLookAt(0.01f) // 让物体运动时始终朝向前进方向
                .WaitForCompletion();

            // 【阶段3】移动到位置 A (2秒)
            yield return transform.DOMove(posA.position, 2f).WaitForCompletion();

            // 【阶段4】在 A 点开始上下摇动，等待玩家靠近
            shakeTween = transform.DOMoveY(transform.position.y + shakeAmplitude, shakeDuration)
                .SetLoops(-1, LoopType.Yoyo);
            yield return new WaitUntil(() => Vector3.Distance(player.position, posA.position) <= arrivalThreshold);
            // 玩家接近后停止摇动 tween
            shakeTween.Kill();

            // 【阶段5】移动到位置 B (2秒)
            yield return transform.DOMove(posB.position, 2f).WaitForCompletion();
            // 到达 B 点后，同样开始上下摇动等待玩家
            shakeTween = transform.DOMoveY(transform.position.y + shakeAmplitude, shakeDuration)
                .SetLoops(-1, LoopType.Yoyo);
            yield return new WaitUntil(() => Vector3.Distance(player.position, posB.position) <= arrivalThreshold);
            shakeTween.Kill();

            // 【阶段6】移动到位置 C (2秒)
            yield return transform.DOMove(posC.position, 2f).WaitForCompletion();
            // 在 C 点，设置更显著的摇动
            shakeTween = transform.DOMoveY(transform.position.y + shakeAmplitude * 2, shakeDuration)
                .SetLoops(-1, LoopType.Yoyo);
            yield return new WaitUntil(() => Vector3.Distance(player.position, posC.position) <= arrivalThreshold);
            shakeTween.Kill();
            
            this.transform.parent = Ghost.instance.transform;
            this.transform.localPosition = new Vector3(0, 0.145999998f, -0.342999995f);

        }
    }
}