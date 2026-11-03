using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 offset = new Vector3(4f, 0f, 0f);
    public float speed = 0.5f;

    private Vector3 start;

    void Start()
    {
        start = transform.position;
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = Vector3.Lerp(start, start + offset, t);
    }
}