using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    [SerializeField] Transform cam;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        rb.AddForce(forward * inputDir.y * moveSpeed * Time.fixedDeltaTime, ForceMode.Force);

        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        rb.AddForce(right * inputDir.x * moveSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }
}
