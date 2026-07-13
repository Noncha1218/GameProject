using UnityEngine;
using UnityEngine.InputSystem;

public class HeartbeatHaptics : MonoBehaviour
{
    [Header("心拍設定")]
    public float minBPM = 60f;
    public float maxBPM = 150f;

    [Header("鉄骨設定")]
    public Transform beam;
    public float beamWidth = 0.4f;

    private Gamepad gamepad;
    private float timer = 0f;
    public float currentBPM;
    private bool isOnBeam = false;

    void Start()
    {
        gamepad = Gamepad.current;
        currentBPM = minBPM;
    }

    void OnTriggerEnter(Collider other)

    {
        Debug.Log("触れた: " + other.tag);
        if (other.CompareTag("Beam"))
        {
            isOnBeam = true;
            // 記録スタート
            DataRecorder recorder = GetComponent<DataRecorder>();
            if (recorder != null)
            {
                recorder.StartRecording();
            }
        }
        if (other.CompareTag("SavePoint"))
        {
            DataRecorder recorder = GetComponent<DataRecorder>();
            if (recorder != null)
            {
                recorder.StopAndSave();
                recorder.ResetData();
            }
        }
            }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Beam"))
        {
            isOnBeam = false;
            gamepad?.SetMotorSpeeds(0f, 0f);
            currentBPM = minBPM;
        }
    }

    void Update()
    {
        Debug.Log("isOnBeam: " + isOnBeam + " gamepad: " + (gamepad != null ? "OK" : "NULL") + " beam: " + (beam != null ? "OK" : "NULL"));
        if (!isOnBeam || beam == null)
        {
            gamepad?.SetMotorSpeeds(0f, 0f);
            currentBPM = minBPM;
            return;
        }

        Vector3 localPos = beam.InverseTransformPoint(transform.position);
        float offset = Mathf.Abs(localPos.x);
        float distanceRatio = 1f - (offset / (beamWidth / 2f));
        distanceRatio = Mathf.Clamp01(distanceRatio);
        currentBPM = Mathf.Lerp(maxBPM, minBPM, distanceRatio);

        Debug.Log("現在のBPM: " + currentBPM);

        float beatInterval = 60f / currentBPM;
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            timer = 0f;
            StartCoroutine(HeartbeatPulse());
        }
    }

    System.Collections.IEnumerator HeartbeatPulse()
    {
        gamepad.SetMotorSpeeds(1f, 0f);
        yield return new WaitForSeconds(0.05f);
        gamepad.SetMotorSpeeds(0f, 0f);

        yield return new WaitForSeconds(0.05f);

        gamepad.SetMotorSpeeds(0.3f, 0.1f);
        yield return new WaitForSeconds(0.04f);
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    void OnDisable()
    {
        gamepad?.SetMotorSpeeds(0f, 0f);
    }
}