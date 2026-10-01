using UnityEngine;
using TMPro;
public class LapCounter : MonoBehaviour
{
    public TextMeshProUGUI lapText;
    public int totalLaps;
    public static int CurrentLap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        CurrentLap = 0;
        lapText.text = CurrentLap.ToString() + "/" + totalLaps.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        lapText.text = CurrentLap.ToString() + "/" + totalLaps.ToString();
    }
}