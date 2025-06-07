using UnityEngine;

public class KeepAlive : MonoBehaviour
{
    private static KeepAlive instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}