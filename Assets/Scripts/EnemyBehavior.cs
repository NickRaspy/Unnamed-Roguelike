using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Transform target;

    void Start()
    {
    }
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime*speed);
        var relPos = transform.InverseTransformDirection(target.position);
        relPos.y = 0;
        var targetPos = transform.TransformPoint(relPos);
        transform.LookAt(targetPos, transform.up);
    }
}
