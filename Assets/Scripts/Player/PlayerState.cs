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
        [SerializeField] private bool isrunning = false;
        private Vector3 inputDirection;
        
        [Header("攻击状态")]
        [SerializeField] bool isAttacking = false;
        [SerializeField] bool attackOne = false;
        [SerializeField] bool attackTwo = false;
        [SerializeField] bool attackThree = false;
        
        

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
            attackOne = animator.GetBool("OnAttackOne");
            attackTwo = animator.GetBool("OnAttackTwo");
            attackThree = animator.GetBool("OnAttackThree");
            isAttacking = attackOne || attackTwo || attackThree;
            
            float horizontal = Input.GetAxis("Horizontal"); // A、D 控制左右移动
            float vertical = Input.GetAxis("Vertical");     // W、S 控制前后移动
            inputDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            HandleAttack();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            animator.SetBool("isRunning", isrunning);
            if (inputDirection.magnitude >= 0.1f && !isAttacking)
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

                isrunning = true;
            }
            else
            {
                isrunning = false;
            }
        }

        private void HandleAttack()
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                if (!attackOne && !attackTwo && animator.GetCurrentAnimatorStateInfo(4).normalizedTime >= 0.35f)
                {
                    animator.CrossFade("combo1", 0.1f);
                }
                else if (attackOne && animator.GetCurrentAnimatorStateInfo(4).normalizedTime >= 0.35f)
                {
                    animator.CrossFade("combo2", 0.1f);
                }
                else if (attackTwo && animator.GetCurrentAnimatorStateInfo(4).normalizedTime >= 0.3f)
                {
                    animator.CrossFade("combo3", 0.1f);
                }
            }
        }
    }
}
