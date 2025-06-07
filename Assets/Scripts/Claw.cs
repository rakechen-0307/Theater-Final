using UnityEngine;

public class Claw : MonoBehaviour
{
    public bool touched = false; // Indicates if the claw has touched an object

    void OnTriggerEnter(Collider other)
    {
        // Check if the claw has touched an object
        if (other.CompareTag("barrel"))
        {
            touched = true; // Set touched to true when the claw touches a barrel
        }
    }
    void OnTriggerExit(Collider other)
    {
        // Reset touched when the claw exits the barrel
        if (other.CompareTag("barrel"))
        {
            touched = false; // Set touched to false when the claw no longer touches a barrel
        }
    }
}
