using System.Collections;
using System.Collections.Generic;
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
        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        rb.AddForce(forward * Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);

        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        rb.AddForce(right * Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }


}
