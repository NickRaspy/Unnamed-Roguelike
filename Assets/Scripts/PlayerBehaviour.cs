using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private float gravityOffset = 0f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private GameObject playerCamera;

    private Rigidbody rb;
    private float speedMultiplier = 1f;
    public bool isOnLand = false;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal")*Time.deltaTime * speed * speedMultiplier, 0f, Input.GetAxis("Vertical") * Time.deltaTime * speed * speedMultiplier);
        Rotate();
    }
    void Rotate()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime, 0);
    }
    private void OnGUI()
    {
        if(Event.current.type == EventType.KeyDown)
        {
            if(Event.current.keyCode == KeyCode.Space && isOnLand)
            {
                rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            }
            if(Event.current.keyCode == KeyCode.LeftShift)
            {
                speedMultiplier = 1.5f;
            }
        }
        else if(Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.LeftShift)
            {
                speedMultiplier = 1f;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Planet")) isOnLand = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Planet")) isOnLand = false;
    }
}
