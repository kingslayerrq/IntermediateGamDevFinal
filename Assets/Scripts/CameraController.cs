using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private KeyCode panUp = KeyCode.UpArrow;
    [SerializeField] private KeyCode panDown = KeyCode.DownArrow;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraSpeed;

    private Vector3 newPos;
    public Transform player;
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (player)
        {
            followPlayer();
        }
    }
    void followPlayer()
    {
        newPos = player.position + cameraOffset;
        transform.position = Vector3.Lerp(transform.position, newPos, cameraSpeed * Time.deltaTime);
    }
}
