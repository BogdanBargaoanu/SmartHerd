using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public float moveRange = 5f;
    public float moveSpeed = 0.1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}
