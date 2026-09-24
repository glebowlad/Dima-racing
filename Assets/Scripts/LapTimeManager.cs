using UnityEngine;

public class FinishOpener : MonoBehaviour
{
    public LapTimeManager lapTimeManager;
    // public BoxCollider opener;
    public float lapTime;
    public float bestTime;
    void Start()
    {
        bestTime = Mathf.Infinity;
        // bestTime= PlayerPrefs.GetFloat("best", bestTime);
        //lapTimeManager.SetBestTime(bestTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //opener.isTrigger = true;
            LapTime = LapTimeManager.SecCount;
            LapTimeManager.SecCount = 0;
            LapCounter.CurrentLap++;
            if (lapTime < bestTime)
            {
                bestTime = lapTime;
                lapTimeManager.SetBestTime(bestTime);
                //PlayerPrefs.SetFloat("best", bestTime);
                // PlayerPrefs.Save();
            }
        }
    }
}