using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 3f;
    public float gravity = -9.81f;

    [Header("カメラ設定")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    [Header("アニメーション")]
    public Animator animator;

    [Header("ジャンプ設定")]
    public float jumpHeight = 3f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        velocity.y = -2f; // これを追加
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        float speed = new Vector2(moveX, moveZ).magnitude;
        animator.SetFloat("Speed", speed);

        if (controller.isGrounded)
        {
            velocity.y = -2f;
            if (Input.GetButtonDown("Jump") ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        // 安全な位置を定期的に記録
        if (controller.isGrounded)
        {
            GameOverManager gom = FindObjectOfType<GameOverManager>();
            if (gom != null)
            {
                gom.SetLastSafePosition(transform.position);
            }
        }
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, -20f);
        controller.Move(velocity * Time.deltaTime);
    }
}
