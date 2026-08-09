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

    [Header("風設定")]
    public float windForce = 2f;
    public float windMinTime = 1f;  // 最短待機時間
    public float windMaxTime = 2.2f;  // 最長待機時間
    private float windTimer = 0f;
    private float windInterval = 0f;
    private Vector3 windVelocity = Vector3.zero;
    private float windDurationTimer = 0f;
    private float windDuration = 0.5f;
    private int windCount = 0; // 風を吹かせた回数
    private bool windScheduled = false;

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

        Debug.Log("isGrounded: " + controller.isGrounded);

        // 風の処理（鉄骨の上にいるときだけ）
        HeartbeatHaptics haptics = GetComponent<HeartbeatHaptics>();
        bool onBeam = haptics != null && haptics.isOnFirstBeam;

        if (onBeam && windCount < 2 && !windScheduled)
        {
            windInterval = Random.Range(windMinTime, windMaxTime);
            windScheduled = true;
            windTimer = 0f;
        }

        if (windScheduled && windCount < 2)
        {
            windTimer += Time.deltaTime;
            if (windTimer >= windInterval)
            {
                windCount++;
                windScheduled = false;
                float direction = Random.Range(0, 2) == 0 ? -1f : 1f;
                windVelocity = transform.right * direction * windForce;
                windDurationTimer = windDuration;
            }
        }

        if (windDurationTimer > 0)
        {
            controller.Move(windVelocity * Time.deltaTime);
            windDurationTimer -= Time.deltaTime;
        }
        else
        {
            windVelocity = Vector3.zero;
        }

        // 地面に接地してたらvelocityをリセット
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
            if (controller.isGrounded)
        {

           
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
