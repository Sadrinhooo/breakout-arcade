using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    public static float gameDuration {get; private set;}
    private bool shouldBeRunning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameDuration = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldBeRunning) DisplayTimer();
    }

    public void DisplayTimer()
    {
        gameDuration += Time.deltaTime;
        int minutesDisplay = (int)gameDuration / 60;
        int secondsDisplay = (int)gameDuration % 60;
        timerText.text = "Time : " + minutesDisplay.ToString("00") + ":" + secondsDisplay.ToString("00");
    }

    public void StopClock()
    {
        shouldBeRunning = false;
    }

    public void StartClock()
    {
        shouldBeRunning = true;
    }
}
