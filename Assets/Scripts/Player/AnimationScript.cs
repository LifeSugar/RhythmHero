using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using rhythmhero.audio;

public class AnimationScript : MonoBehaviour
{
    [Header("脚步声")]
    public EventReference stepSound;

    [Header("挥剑")] public EventReference swishSound;
    public Transform swordPosition;

    [Header("特效")] public ParticleSystem slash1;
    public ParticleSystem slash2;
    public ParticleSystem slash3;

    private Animator animator;
    private Transform playerTransform;
    
    public Collider hitcollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = transform.parent;  // 获取 Player 的 Transform
    }

    // void OnAnimatorMove()
    // {
    //     if (animator && playerTransform)
    //     {
    //         Vector3 rootPosition = animator.rootPosition;
    //         Quaternion rootRotation = animator.rootRotation;

    //         // 将根运动应用到 Player
    //         playerTransform.position += rootPosition - transform.position;
    //         playerTransform.rotation = rootRotation;

    //         // 重置 Character 的位置，防止重复叠加
    //         transform.localPosition = Vector3.zero;
    //         transform.localRotation = Quaternion.identity;
    //     }
    // }

    void OnAnimatorMove()
    {
        if (!animator || !playerTransform) return;

        // 取本帧动画带来的位移与旋转增量
        Vector3 deltaPos = animator.deltaPosition;
        Quaternion deltaRot = animator.deltaRotation;

        // 将增量应用到 Player 上
        playerTransform.position += deltaPos;
        playerTransform.rotation *= deltaRot;

        // 让子物体(模型)回到父物体的原点，避免子物体重复叠加根运动
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OpenCollider()
    {
        hitcollider.enabled = true;
    }

    public void CloseCollider()
    {
        hitcollider.enabled = false;
    }


    public void PlayStepSound()
    {
        AudioManager.instance.PlayOneShot(stepSound, this.transform.parent.transform.position);
    }

    public void PlaySwishSound()
    {
        AudioManager.instance.PlayOneShot(swishSound, swordPosition.position);
    }

    public void EmitSlahFXslah1()
    {
        AudioManager.instance.PlayOneShot(swishSound, swordPosition.position);
        slash1.Play();
    }

    public void EmitSlahFXslah2()
    {
        AudioManager.instance.PlayOneShot(swishSound, swordPosition.position);
        slash2.Play();
    }

    public void EmitSlahFXslah3()
    {
        AudioManager.instance.PlayOneShot(swishSound, swordPosition.position);
        slash3.Play();
    }
    
    
}
