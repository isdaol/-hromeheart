using UnityEngine;
using TMPro;

public class PlayerProfile : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public Color[] colors = { Color.red, Color.blue, Color.green };

    void Start()
    {
        SaveManager saveManager = SaveManager.Instance;

        if (saveManager == null || saveManager.Data == null)
        {
            Debug.LogError("SaveManager data is not available.", this);
            return;
        }

        string playerName = saveManager.Data.playerName;

        if (string.IsNullOrEmpty(playerName))
            playerName = "Player";

        if (nameText != null)
            nameText.text = playerName;

        if (colors == null || colors.Length == 0)
        {
            Debug.LogError("No player colors are configured.", this);
            return;
        }

        int colorIndex = Mathf.Clamp(
            saveManager.Data.playerColor,
            0,
            colors.Length - 1
        );

        Renderer playerRenderer = GetComponent<Renderer>();

        if (playerRenderer == null)
        {
            Debug.LogError("Renderer is missing.", this);
            return;
        }

        playerRenderer.material.color = colors[colorIndex];
    }
}