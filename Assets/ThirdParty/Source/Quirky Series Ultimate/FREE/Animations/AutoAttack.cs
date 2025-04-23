using UnityEngine;
using System.Collections;

public class AutoAttack : MonoBehaviour
{
    private Animator animator;
    public float interval = 4f;
    private float timer;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            Debug.Log("AutoAttack 正在运行: " + Time.time);

            // 启动一个协程，在下一帧切换 Attack
            StartCoroutine(PlayAttackSequence());
        }
    }

    IEnumerator PlayAttackSequence()
    {
        animator.Play("Idle_A", 0, 0f); // 强制回 Idle_A
        yield return null; // 等待一帧
        animator.Play("Attack", 0, 0f); // 下一帧再播放 Attack
    }
}


