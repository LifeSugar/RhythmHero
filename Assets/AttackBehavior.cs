using UnityEngine;

public class AttackBehavior : StateMachineBehaviour
{
    public string boolName = "isAttacking";  // 要操作的 bool 参数名

    // 当动画状态开始时调用
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, true);
    }

    // 当动画状态结束时调用
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, false);
    }
}