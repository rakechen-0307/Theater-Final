using UnityEngine;

public class Claw : MonoBehaviour
{
    public bool touched = false; // Indicates if the claw has touched an object
    public GameObject contact = null; // The object that the claw is currently touching

    void OnTriggerEnter(Collider other)
    {
        touched = true;
        contact = other.gameObject;
        // Check if the claw has touched an object
        // if (other.CompareTag("barrel"))
        // {
        //     touched = true; // Set touched to true when the claw touches a barrel
        // }
    }
    void OnTriggerExit(Collider other)
    {
        touched = false; // Reset touched when the claw exits the object
        contact = null; // Clear the contact object
        // Reset touched when the claw exits the barrel
        // if (other.CompareTag("barrel"))
        // {
        //     touched = false; // Set touched to false when the claw no longer touches a barrel
        // }
    }
}
