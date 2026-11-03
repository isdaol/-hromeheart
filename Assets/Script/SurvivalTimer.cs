
using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    public float survivalTime = 60f;
    public TMP_Text timerText;
    public Animator portalAnimator;

    private float timeLeft;
    private bool timeIsUp;

    void Start()
    {
        timeLeft = survivalTime;
        UpdateTimerText();
    }

    void Update()
    {
        if (timeIsUp)
            return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            timeIsUp = true;
            UpdateTimerText();
            OpenPortal();
            return;
        }

        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeLeft);
            timerText.text = "Time: " + seconds;
        }
    }

    void OpenPortal()
    {
        if (portalAnimator != null)
        {
            portalAnimator.SetBool("IsOpen", true);
        }

        Debug.Log("60 seconds survived! Portal opened.");
    }
}
