using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class LapTimeManager : MonoBehaviour
{
    public TextMeshProUGUI lapText, bestTimeText; //startCounter;
    public static float SecCount;
    public System.TimeSpan time;
    private static TimeSpan bestTime;
    // public PrometeoCarController carController;
   
    void Update()
    {
        SecCount += Time.deltaTime;
        time = System.TimeSpan.FromSeconds(SecCount);
        lapText.text = string.Format("Lap Time: {0:D2}:{1:D2}.{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);
    }
    public void SetBestTime(float best)
    {
        if (best == Mathf.Infinity)
        {
            bestTimeText.text = "00:00.00";
            return;
        }
        bestTime = System.TimeSpan.FromSeconds(best);
        bestTimeText.text = string.Format("Best time: {0:D2}:{1:D2}.{2:D2}", bestTime.Minutes, bestTime.Seconds, bestTime.Milliseconds / 10);
    }
}
