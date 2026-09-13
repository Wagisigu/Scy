using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Vector3 offset = new(0, 0, -10);

    private void LateUpdate()
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        Vector3 targetPosition = PlayerController.Instance.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}