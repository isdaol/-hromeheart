using UnityEngine;

public static class EnemyScreenVisibility
{
    private static Camera cachedCamera;

    public static bool IsOnScreen(Renderer targetRenderer)
    {
        if (targetRenderer == null)
            return false;

        if (cachedCamera == null)
        {
            cachedCamera = Camera.main;
            if (cachedCamera == null)
                cachedCamera = Object.FindAnyObjectByType<Camera>();
        }

        if (cachedCamera == null)
            return false;

        Bounds bounds = targetRenderer.bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;
        float minDepth = float.PositiveInfinity;
        float maxDepth = float.NegativeInfinity;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 corner = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z
                    );
                    Vector3 viewport = cachedCamera.WorldToViewportPoint(corner);

                    minX = Mathf.Min(minX, viewport.x);
                    maxX = Mathf.Max(maxX, viewport.x);
                    minY = Mathf.Min(minY, viewport.y);
                    maxY = Mathf.Max(maxY, viewport.y);
                    minDepth = Mathf.Min(minDepth, viewport.z);
                    maxDepth = Mathf.Max(maxDepth, viewport.z);
                }
            }
        }

        bool overlapsViewport =
            maxX >= 0f && minX <= 1f &&
            maxY >= 0f && minY <= 1f;
        bool overlapsCameraDepth =
            maxDepth >= cachedCamera.nearClipPlane &&
            minDepth <= cachedCamera.farClipPlane;

        return overlapsViewport && overlapsCameraDepth;
    }
}
