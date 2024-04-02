using UnityEngine;


public class CameraController : MonoBehaviour
{
    public Vector2 lookSpeed = new Vector2(5f, 5f);
    public float moveSpeed = 2f;
    public bool invertLook = false;

    private float yaw;
    private float pitch;

    private void Start()
    {
        // x - right    pitch
        // y - up       yaw
        // z - forward  roll
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        var velocity = new Vector3(
            Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime,
            Input.GetAxis("UpDown") * moveSpeed * Time.deltaTime,
            Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);

        Vector2 lookVelocity = new Vector2(
            pitch += Input.GetAxis("Mouse Y") * lookSpeed.y * ((invertLook) ? 1 : -1),
            yaw += Input.GetAxis("Mouse X") * lookSpeed.x);

        transform.Translate(velocity);
        transform.eulerAngles = lookVelocity;
    }

}
