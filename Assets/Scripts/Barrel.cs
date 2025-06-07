using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public bool isGrounded = false; // Check if the barrel is grounded
    public bool isOnConveyor = false; // Check if the barrel is on a conveyor
    private List<bool> grounded = new List<bool>();

    void OnCollisionStay(Collision collision)
    {
        if (isOnConveyor)
        {
            // If on conveyor, we don't check for grounded state
            return;
        }
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

    void FixedUpdate()
    {
        if (isOnConveyor)
        {
            // If on conveyor, we don't check for grounded state
            isGrounded = true;
            return;
        }
        if (grounded.Count == 0)
        {
            isGrounded = false; // No contacts, not grounded
            return;
        }
        // if any contact was ground-like, we are grounded
        isGrounded = grounded.Contains(true);
        grounded.Clear(); // Clear the list for the next FixedUpdate
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("conveyor"))
        {
            isOnConveyor = true; // Set on conveyor to true if collided with conveyor
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("conveyor"))
        {
            isOnConveyor = false; // Set on conveyor to false if exited conveyor
        }
    }
}
