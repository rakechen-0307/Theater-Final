using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ManualGravity : MonoBehaviour
{
    [SerializeField] private float gravity = -9.8f; // Gravity value
    private float verticalVelocity = 0f; // Current vertical velocity
    private Rigidbody rb;
    private bool isGrounded = false; // Check if the object is grounded

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isGrounded)
            verticalVelocity += gravity * Time.fixedDeltaTime;

        Vector3 movement = new Vector3(0, verticalVelocity, 0) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        verticalVelocity = 0f;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

}
