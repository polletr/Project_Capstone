using UnityEngine;

public class ObjectFloater : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float height = 1f;
    [SerializeField] private AnimationCurve curve;

    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        var time = Time.time * speed;
        var curveValue = curve.Evaluate(Mathf.PingPong(time, 1f));
        var newY = startPos.y + (curveValue - 0.5f) * 2 * height;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
