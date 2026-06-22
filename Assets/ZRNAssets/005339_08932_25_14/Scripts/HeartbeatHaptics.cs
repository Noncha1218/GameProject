using UnityEngine;
using UnityEngine.InputSystem;

public class HeartbeatHaptics : MonoBehaviour
{
    [Header("心拍設定")]
    public float minBPM = 60f;   // 中央にいるとき
    public float maxBPM = 150f;  // 端にいるとき

    [Header("鉄骨設定")]
    public Transform beam;        // 鉄骨のGameObject
    public float beamWidth = 0.4f; // 鉄骨の幅（メートル）

    private Gamepad gamepad;
    private float timer = 0f;
    private float currentBPM;

    void Start()
    {
        gamepad = Gamepad.current;
        currentBPM = minBPM;
    }

    void Update()
    {
        if (beam == null) return;

        // キャラと鉄骨中心のX軸のズレ
        Vector3 localPos = beam.InverseTransformPoint(transform.position);
        float offset = Mathf.Abs(localPos.x);
        Debug.Log("localPos: " + localPos);

        // 端からの距離を0〜1に正規化（0=端、1=中央）
        float distanceRatio = 1f - (offset / (beamWidth / 2f));
        distanceRatio = Mathf.Clamp01(distanceRatio);

        // BPMをなめらかに変化
        currentBPM = Mathf.Lerp(maxBPM, minBPM, distanceRatio);
        Debug.Log("現在のBPM: " + currentBPM);

        // 心拍タイマー
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
        // 第1音（強）
        gamepad.SetMotorSpeeds(1f, 0f);
        yield return new WaitForSeconds(0.05f);
        gamepad.SetMotorSpeeds(0f, 0f);

        yield return new WaitForSeconds(0.05f);

        // 第2音（弱）
        gamepad.SetMotorSpeeds(0.3f, 0.1f);
        yield return new WaitForSeconds(0.04f);
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    void OnDisable()
    {
        gamepad?.SetMotorSpeeds(0f, 0f);
    }
}