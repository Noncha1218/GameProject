using UnityEngine;
using System.IO;
using System;

public class DataRecorder : MonoBehaviour
{
    [Header("記録設定")]
    public HeartbeatHaptics heartbeatHaptics;
    public float dangerBPM = 100f; // これ以上で危険ゾーンとみなす

    private int fallCount = 0;
    private float dangerTime = 0f;
    private float playTime = 0f;
    private bool isRecording = true;

    void Update()
    {
        if (!isRecording) return;

        playTime += Time.deltaTime;

        // 危険ゾーンにいる時間を記録
        if (heartbeatHaptics != null && heartbeatHaptics.currentBPM >= dangerBPM)
        {
            dangerTime += Time.deltaTime;
        }
    }

    public void RecordFall()
    {
        fallCount++;
    }

    public void StopAndSave()
    {
        isRecording = false;
        SaveToCSV();
    }

    void SaveToCSV()
    {
        string path = Application.dataPath + "/result.csv";
        string data = "項目,値\n";
        data += "プレイ時間(秒)," + playTime.ToString("F2") + "\n";
        data += "落下回数," + fallCount + "\n";
        data += "危険ゾーン滞在時間(秒)," + dangerTime.ToString("F2") + "\n";

        File.WriteAllText(path, data);
        Debug.Log("データを保存しました: " + path);
    }
}