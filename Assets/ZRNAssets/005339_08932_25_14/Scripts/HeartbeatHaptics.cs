using UnityEngine;
using UnityEngine.InputSystem;

public class HeartbeatHaptics : MonoBehaviour
{
    public enum VibrationMode
    {
        None,      // 条件A：振動なし
        Alert,     // 条件B：端で一回だけ振動
        Heartbeat  // 条件C：心拍振動
    }
    [Header("UIゲージ")]
    public UnityEngine.UI.Slider dangerMeter;

    [Header("モード設定")]
    public VibrationMode mode = VibrationMode.Heartbeat;

    [Header("心拍設定")]
    public float minBPM = 60f;
    public float maxBPM = 150f;

    [Header("鉄骨設定")]
    public Transform beam;
    public float beamWidth = 0.4f;

    [Header("アラート設定")]
    public float alertThreshold = 0.2f; // 端からこの距離以内でアラート振動

    private Gamepad gamepad;
    private float timer = 0f;
    public float currentBPM;
    public bool isOnBeam = false;
    public bool isOnFirstBeam = false;
    private bool hasSaved = false;
    private bool alertTriggered = false; // アラート振動済みフラグ

    void Start()
    {
        gamepad = Gamepad.current;
        currentBPM = minBPM;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Beam") || other.CompareTag("BeamFirst"))
        {
            isOnBeam = true;
            isOnFirstBeam = true;
            hasSaved = false;
            alertTriggered = false;
            Transform parent = other.transform.parent;
            Transform beamCenter = parent.Find("BeamCenter");
            beam = beamCenter != null ? beamCenter : parent;
            DataRecorder recorder = GetComponent<DataRecorder>();
            if (recorder != null)
            {
                recorder.StartRecording();
            }
        }
        if (other.CompareTag("SavePoint") && !hasSaved)
        {
            hasSaved = true;
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
        if (other.CompareTag("Beam") || other.CompareTag("BeamFirst"))
        {
            isOnBeam = false;
            isOnFirstBeam = false;
            gamepad?.SetMotorSpeeds(0f, 0f);
            currentBPM = minBPM;
            alertTriggered = false;
        }
    }

    void Update()
    {
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

        if (mode == VibrationMode.None)
        {
            gamepad?.SetMotorSpeeds(0f, 0f);
            // ゲージを更新
            if (dangerMeter != null)
            {
                dangerMeter.value = 1f - distanceRatio;
            }
        }
        else if (mode == VibrationMode.Alert)
        {
            // 端に近づいたら一回だけ振動
            if (distanceRatio < alertThreshold && !alertTriggered)
            {
                alertTriggered = true;
                StartCoroutine(AlertPulse());
            }
            else if (distanceRatio >= alertThreshold)
            {
                alertTriggered = false;
            }
        }
        else if (mode == VibrationMode.Heartbeat)
        {
            // 心拍振動
            float beatInterval = 60f / currentBPM;
            timer += Time.deltaTime;
            if (timer >= beatInterval)
            {
                timer = 0f;
                StartCoroutine(HeartbeatPulse());
            }
        }

        Debug.Log("現在のBPM: " + currentBPM + " Mode: " + mode);
    }

    System.Collections.IEnumerator AlertPulse()
    {
        // 一回だけ強く振動
        gamepad?.SetMotorSpeeds(1f, 1f);
        yield return new WaitForSeconds(0.2f);
        gamepad?.SetMotorSpeeds(0f, 0f);
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