using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public Transform player;

    private Vector3 lastSafePosition;

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
        DataRecorder recorder = FindObjectOfType<DataRecorder>();
        if (recorder != null)
        {
            recorder.StopAndSave();
        }

        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // プレイヤーを最後の安全な位置に移動
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.position = lastSafePosition;
            if (cc != null) cc.enabled = true;
        }
    }

    public void RestartGame()
    {
        DataRecorder recorder = FindObjectOfType<DataRecorder>();
        if (recorder != null)
        {
            recorder.StopAndSave();
        }
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}