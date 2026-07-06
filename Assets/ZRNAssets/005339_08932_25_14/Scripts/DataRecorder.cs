using UnityEngine;
using System.IO;
using System;

public class DataRecorder : MonoBehaviour
{
    [Header("記録設定")]
    public HeartbeatHaptics heartbeatHaptics;
    public float dangerBPM = 100f;
    public string participantID = "player1";

    private int fallCount = 0;
    private float dangerTime = 0f;
    private float playTime = 0f;
    private bool isRecording = true;

    void Update()
    {
        if (!isRecording) return;

        playTime += Time.deltaTime;

        if (heartbeatHaptics != null && heartbeatHaptics.currentBPM >= dangerBPM)
        {
            dangerTime += Time.deltaTime;
        }
    }
    public void ResetData()
    {
        fallCount = 0;
        dangerTime = 0f;
        playTime = 0f;
        isRecording = true;
    }

    public void RecordFall()
    {
        fallCount++;
    }

    public void StopAndSave()
    {
        isRecording = false;
        Debug.Log("参加者ID: " + participantID); // 追加
        SaveToCSV();
    }

    void SaveToCSV()
    {
        string path = Application.dataPath + "/result.csv";
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                if (new FileInfo(path).Length == 0)
                {
                    sw.WriteLine("参加者ID,プレイ時間(秒),落下回数,危険ゾーン滞在時間(秒)");
                }
                sw.WriteLine(participantID + "," + playTime.ToString("F2") + "," + fallCount + "," + dangerTime.ToString("F2"));
            }
            Debug.Log("データを保存しました: " + path);
        }
        catch (Exception e)
        {
            Debug.LogWarning("CSVの保存に失敗しました: " + e.Message);
        }
    }
}