using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private float FollowSpeed = 2f;
    [SerializeField] private Vector3 Offset = new(0, 0, -10);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + Offset, FollowSpeed * Time.deltaTime);
    }
}
