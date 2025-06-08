using UnityEngine;

public class Handle : MonoBehaviour
{
    public bool touched = false; // Indicates if the handle has been touched

    void OnTriggerEnter(Collider other)
    {
        touched = true; // Set touched to true when the handle is touched
        // Additional logic can be added here if needed
    }
    void OnTriggerExit(Collider other)
    {
        touched = false; // Reset touched when the handle is no longer touched
        // Additional logic can be added here if needed
    }
}
