using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public Transform player;
    private Vector3 lastSafePosition;
    private Vector3 oneSecondAgoPosition;
    private float positionTimer = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            DataRecorder recorder = other.GetComponent<DataRecorder>();
            if (recorder != null)
            {
                recorder.RecordFall();
            }
        }
    }

    public void SetLastSafePosition(Vector3 position)
    {
        lastSafePosition = position;
    }

    void Update()
    {
        // 1•b‚²‚Æ‚ÉˆÊ’u‚ðXV
        positionTimer += Time.deltaTime;
        if (positionTimer >= 0.5f)
        {
            positionTimer = 0f;
            oneSecondAgoPosition = lastSafePosition;
        }

        if (gameOverUI.activeSelf)
        {
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                RestartFromLastPosition();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartFromLastPosition();
            }
        }
    }

    public void RestartFromLastPosition()
    {
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.position = oneSecondAgoPosition;
            if (cc != null) cc.enabled = true;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}