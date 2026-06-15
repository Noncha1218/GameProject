using UnityEngine;

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

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // マウスで視点回転
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // WASDで移動
        float moveX = Input.GetAxis("Horizontal");
        
        float moveZ = Input.GetAxis("Vertical");
        Debug.Log("moveX: " + moveX + " moveZ: " + moveZ);

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // アニメーション
        float speed = new Vector2(moveX, moveZ).magnitude;
        animator.SetFloat("Speed", speed);

        // 重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        Debug.Log("Speed: " + speed + " Animator: " + (animator != null ? "OK" : "NULL"));
    }
}