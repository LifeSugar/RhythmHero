using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using rhythmhero.audio;

public class AnimationScript : MonoBehaviour
{
    public EventReference stepSound;
    
    private Animator animator;
    private Transform playerTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = transform.parent;  // 获取 Player 的 Transform
    }

    void OnAnimatorMove()
    {
        if (animator && playerTransform)
        {
            Vector3 rootPosition = animator.rootPosition;
            Quaternion rootRotation = animator.rootRotation;

            // 将根运动应用到 Player
            playerTransform.position += rootPosition - transform.position;
            playerTransform.rotation = rootRotation;
            
            // 重置 Character 的位置，防止重复叠加
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void PlayStepSound()
    {
        AudioManager.instance.PlayOneShot(stepSound, this.transform.parent.transform.position);
    }
}
