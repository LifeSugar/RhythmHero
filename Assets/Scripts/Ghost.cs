using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;
using rhythmhero.audio;

namespace rhythmhero
{
    public class Ghost : MonoBehaviour
    {
        public static Ghost instance; //请保证场景中只有一个ghost

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Duplicate Ghost!!!");
            }
            instance = this;
        }

        //飘起来的音效，可以没有, 如果有的话，请让时长为2s
        [Header("漂浮上来的音效，时长最好为2s")]
        public EventReference flowUpSound;
        
        //这个方法是小幽灵从井里飘起来
        public void FlowUpGhost()
        {
            
            // 计算目标位置：当前 Y 坐标 + 5
            float targetY = transform.position.y + 2f;
            var targetPos = new Vector3(transform.position.x, targetY, transform.position.z);
            if (!flowUpSound.IsNull)
            {
                AudioManager.instance.PlayOneShot(flowUpSound, targetPos); //有音效那么播放音效
            }
            // 使用 DOTween 中的 DOMoveY 方法来平滑移动 Y 坐标，移动时长为 2 秒
            transform.DOMoveY(targetY, 2f).onComplete = () => DialogueManager.instance.StartDialogue();
        }
    }

}