using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speedX = 5f; // Speed of the character movement
    [SerializeField] private float gravity = 9.8f; // Gravity value to apply when not moving
    public bool isGrounded = false; // Check if the character is grounded
    private List<bool> grounded = new List<bool>(); // List to track ground contacts
    private bool jump = false; // Flag to indicate if the character should jump
    [SerializeField] private float jumpForce = 50f; // Force applied when jumping
    private float acceleration = 0f; // Acceleration factor for movement
    [SerializeField] private int decay_frame = 15; // Decay frame for acceleration
    [SerializeField] private string nextSceneName;
    [SerializeField] private float goalX = 6f;
    [SerializeField] private float goalY = 1.6f;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = false; // Initialize jump flag
        acceleration = 0f; // Initialize acceleration

        animator = GetComponent<Animator>();
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
            animator.SetBool("isJump", true);
        }
        else
        {
            animator.SetBool("isJump", false);
        }

        if (gameObject.transform.position.x >= goalX && gameObject.transform.position.y >= goalY)
        {
            GoToNextScene();
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

        if (Input.GetKey(KeyCode.L))
        {
            rb.linearVelocity += new Vector3(speedX, 0, 0); // Move right
            transform.rotation = Quaternion.Euler(0, 90, 0);
            animator.SetBool("isWalk", true);
        }
        else if (Input.GetKey(KeyCode.J))
        {
            rb.linearVelocity += new Vector3(-speedX, 0, 0); // Move backward
            transform.rotation = Quaternion.Euler(0, -90, 0);
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
        if (jump)
        {
            acceleration = jumpForce;
            jump = false; // Reset jump flag after applying force
        }
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
