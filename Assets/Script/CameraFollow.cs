
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public SpriteRenderer background;
    public float smoothSpeed = 5f;
    public Vector2 offset = new Vector2(0f, 1f);

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null || background == null || cam == null)
            return;

        Bounds bounds = background.bounds;

        // Размер видимой области камеры в мировых координатах
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        // Ограничения с учётом размеров камеры
        float minX = bounds.min.x + halfWidth;
        float maxX = bounds.max.x - halfWidth;
        float minY = bounds.min.y + halfHeight;
        float maxY = bounds.max.y - halfHeight;

        // Целевая позиция камеры
        float targetX = target.position.x + offset.x;
        float targetY = target.position.y + offset.y;

        // Если фон меньше области камеры, центрируем камеру по этой оси
        if (minX > maxX)
            targetX = bounds.center.x;
        else
            targetX = Mathf.Clamp(targetX, minX, maxX);

        if (minY > maxY)
            targetY = bounds.center.y;
        else
            targetY = Mathf.Clamp(targetY, minY, maxY);

        Vector3 desiredPosition = new Vector3(
            targetX,
            targetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
