using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
    public float windMaxTime = 2f;  // 最長待機時間
    private float windTimer = 0f;
    private float windInterval = 0f;
    private Vector3 windVelocity = Vector3.zero;
    private float windDurationTimer = 0f;
    private float windDuration = 0.5f;
    private HashSet<string> windedTags = new HashSet<string>();
    private string currentWindTag = "";
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
        string tag = haptics != null ? haptics.currentBeamTag : "";
        bool onWindBeam = haptics != null && haptics.isOnBeam
                          && (tag == "BeamFirst" || tag == "BeamSecond");

        if (!onWindBeam)
        {
            windScheduled = false;
        }

        if (onWindBeam && !windScheduled && !windedTags.Contains(tag))
        {
            windInterval = Random.Range(windMinTime, windMaxTime);
            windScheduled = true;
            windTimer = 0f;
            currentWindTag = tag;
        }

        if (windScheduled)
        {
            windTimer += Time.deltaTime;
            if (windTimer >= windInterval)
            {
                windedTags.Add(currentWindTag);
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
        
        bool canJump = controller.isGrounded || (haptics != null && haptics.isOnBeam);

        if (canJump)
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
    public void CancelWind()
    {
        windScheduled = false;
        windTimer = 0f;
        windDurationTimer = 0f;
        windVelocity = Vector3.zero;
    }
}
