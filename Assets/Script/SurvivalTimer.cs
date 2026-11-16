using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    [Header("Level Data")]
    public LevelData levelData;

    public TMP_Text timerText;
    public Animator portalAnimator;

    private float timeLeft;
    private bool timeIsUp;

    void Start()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned.", this);
            enabled = false;
            return;
        }

        if (levelData.goalType != LevelGoalType.SurviveTimer)
        {
            enabled = false;
            return;
        }

        if (levelData.survivalTime <= 0f)
        {
            Debug.LogError(
                "Survival Timer goal requires a positive Survival Time.",
                this
            );
            enabled = false;
            return;
        }

        timeLeft = levelData.survivalTime;
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
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = "Time: " + seconds;
    }

    void OpenPortal()
    {
        if (portalAnimator != null)
            portalAnimator.SetBool("IsOpen", true);

        Debug.Log(
            levelData.survivalTime +
            " seconds survived! Portal opened."
        );
    }
}