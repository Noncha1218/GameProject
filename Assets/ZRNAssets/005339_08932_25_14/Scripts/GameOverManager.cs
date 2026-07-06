using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // 落下回数を記録
            DataRecorder recorder = other.GetComponent<DataRecorder>();
            if (recorder != null)
            {
                recorder.RecordFall();
            }

            }
    }
    void Update()
    {
        if (gameOverUI.activeSelf)
        {
            // コントローラーのAボタン
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                RestartGame();
            }
            // キーボードのRキー
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
    public void RestartGame()
    {
        // データ保存を追加
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