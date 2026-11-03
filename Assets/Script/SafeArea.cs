using UnityEngine;

/// <summary>
/// Resizes this saved RectTransform to the device safe area.
/// The SafeArea object and its children are authored in the scene; this script
/// never creates or reparents objects.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class SafeArea : MonoBehaviour
{
    private RectTransform safeAreaRect;
    private Rect lastSafeArea;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        safeAreaRect = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void LateUpdate()
    {
        Rect currentSafeArea = Screen.safeArea;
        if (currentSafeArea != lastSafeArea ||
            Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (safeAreaRect == null || Screen.width <= 0 || Screen.height <= 0)
            return;

        Rect safeArea = Screen.safeArea;
        safeAreaRect.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
        safeAreaRect.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
        safeAreaRect.offsetMin = Vector2.zero;
        safeAreaRect.offsetMax = Vector2.zero;

        lastSafeArea = safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}
