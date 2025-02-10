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
            HandleMovement();
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal"); // A、D 控制左右移动
            float vertical = Input.GetAxis("Vertical");     // W、S 控制前后移动

            Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

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
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
    }
}
