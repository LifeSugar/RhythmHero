using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    private Animator animator;
    private float timer = 0f;

    [Header("定时参数")]
    [Tooltip("每次攻击间隔时间（秒）")]
    public float interval = 2f;

    [Tooltip("初始蓄力时间（秒）")]
    public float chargeTime = 1f;

    private bool firstAttackDone = false;
    private bool isTriggered = false; // 控制是否启用攻击

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isTriggered) return; // 没点击前不做事

        timer += Time.deltaTime;
        var state = animator.GetCurrentAnimatorStateInfo(0);

        if (!firstAttackDone && timer >= chargeTime)
        {
            animator.Play("Attack", 0, 0f);
            Debug.Log("【首次蓄力完成】播放 Attack！");
            firstAttackDone = true;
            timer = 0f;
            return;
        }

        if (firstAttackDone && timer >= interval && !state.IsName("Attack"))
        {
            animator.Play("Attack", 0, 0f);
            Debug.Log("【循环】播放 Attack！");
            timer = 0f;
        }
    }

    //  点击按钮时调用
    public void TriggerAttack()
    {
        isTriggered = true;
        timer = 0f;
        firstAttackDone = false; // 重新蓄力
    }
}
