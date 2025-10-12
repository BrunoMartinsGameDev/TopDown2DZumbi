using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetPosition = target.position;
        if (target.TryGetComponent<Rigidbody2D>(out var rb))
            targetPosition = rb.position;
        transform.position = targetPosition + offset;
    }
}
