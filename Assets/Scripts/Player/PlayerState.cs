using rhythmhero.audio;
using UnityEngine;

namespace rhythmhero
{
    public class PlayerState : MonoBehaviour
    {
        private Animator animator;
        private Rigidbody rb;

        [Header("移动参数")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        
        // 用来记录上一帧是否在跑动
        [SerializeField] bool isrunning = false;
        [SerializeField] private bool isPunching = false;
        private Vector3 inputDirection;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CameraController.instance.Init(this);

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody>();

            if (rb == null)
            {
                Debug.LogError("未找到 Rigidbody 组件！");
            }
            else
            {
                rb.freezeRotation = true;  // 防止 Rigidbody 因碰撞导致角色旋转
            }
        }

        private void Update()
        {
            float horizontal = Input.GetAxis("Horizontal"); // A、D 控制左右移动
            float vertical = Input.GetAxis("Vertical");     // W、S 控制前后移动

            inputDirection = new Vector3(horizontal, 0, vertical).normalized;

            if (!isPunching)
            {
                HandleMovement();
            }
            
            HandlePunch();
        }

        private void HandleMovement()
        {
            
            if (inputDirection.magnitude >= 0.1f)
            {
                // 根据摄像机方向计算世界坐标的移动方向
                Vector3 moveDirection = Camera.main.transform.forward * inputDirection.z + Camera.main.transform.right * inputDirection.x;
                moveDirection.y = 0;  // 确保角色只在水平面移动
                moveDirection.Normalize();

                // 移动角色
                rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);

                // 角色朝向移动方向
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                animator.SetBool("isRunning", true);
                
                isrunning = true;
            }
            else
            {
                if (isrunning)
                {
                    animator.CrossFade("Idle", 0.4f, 0, BiasCalculator.instance.oneBeatBias / 4f);

                    isrunning = false;
                }
                animator.SetBool("isRunning", false);
                
            }
        }

        private void HandlePunch()
        {
            // 按空格触发打拳
            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.CrossFade("Punching", 0.1f);
                isPunching = true;
            }

            // 如果正在打拳，检测何时结束
            if (isPunching)
            {
                // 获取当前动画状态信息
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                // 如果当前播放的就是“Punching”动画，并且进度（normalizedTime）接近尾声
                if (stateInfo.IsName("Punching") && stateInfo.normalizedTime >= 0.9f)
                {
                    if (inputDirection.magnitude >= 0.1f)
                    {
                        animator.CrossFade("Jog", 0.2f);
                        isPunching = false;  // 重置标记
                    }
                    else
                    {
                        // 用和移动脚本里一样的逻辑，回到 Idle
                        animator.CrossFade("Idle", 0.2f, 0, BGMManager.instance.checkerhalf / 4f);

                        isPunching = false;  // 重置标记
                    }
                    
                }
            }
        }
    }
}
