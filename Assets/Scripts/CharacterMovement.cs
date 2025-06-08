using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speedX = 5f; // Speed of the character movement
    [SerializeField] private float gravity = 9.8f; // Gravity value to apply when not moving
    [SerializeField] private bool isGrounded = false; // Check if the character is grounded
    private List<bool> grounded = new List<bool>(); // List to track ground contacts
    private bool jump = false; // Flag to indicate if the character should jump
    [SerializeField] private float jumpForce = 50f; // Force applied when jumping
    private float acceleration = 0f; // Acceleration factor for movement
    [SerializeField] private int decay_frame = 15; // Decay frame for acceleration
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = false; // Initialize jump flag
        acceleration = 0f; // Initialize acceleration
    }


    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.8f)
            {
                grounded.Add(true); // We have contact with a ground-like surface
                return; // early exit: we found ground
            }
        }
        grounded.Add(false); // No ground contact found
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && isGrounded)
        {
            jump = true; // Set jump flag when space is pressed and character is grounded
        }
    }
    void FixedUpdate()
    {
        if (grounded.Count == 0)
        {
            isGrounded = false; // No contacts, not grounded
        }
        else
        {
            // if any contact was ground-like, we are grounded
            isGrounded = grounded.Contains(true);
            grounded.Clear(); // Clear the list for the next FixedUpdate
        }

        if (acceleration > 0)
        {
            acceleration -= jumpForce/decay_frame; // Decrease acceleration over time
        }
        else
        {
            acceleration = 0; // Reset acceleration if not jumping
        }
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y + acceleration * Time.fixedDeltaTime, 0);
        if (!isGrounded)
        {
            rb.linearVelocity -= new Vector3(0, gravity * Time.fixedDeltaTime, 0); // Apply gravity if not grounded
        }
        // else
        // {
        //     rb.linearVelocity = new Vector3(0, 0, 0); // Reset vertical velocity when grounded
        // }

        if (Input.GetKey(KeyCode.L))
        {
            rb.linearVelocity += new Vector3(speedX, 0, 0); // Move right
        }
        else if (Input.GetKey(KeyCode.J))
        {
            rb.linearVelocity += new Vector3(-speedX, 0, 0); // Move backward
        }
        if (jump)
        {
            // rb.linearVelocity += new Vector3(0, jumpForce, 0); // Apply jump force
            acceleration = jumpForce;
            jump = false; // Reset jump flag after applying force
        }
    }
}
