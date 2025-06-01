using UnityEngine;
using OscJack;

public class OSCSender : MonoBehaviour
{
    private static OSCSender _instance;
    public static OSCSender Instance { get => _instance; }
    private OscClient oscClient;
    [SerializeField] private string ip;
    [SerializeField] private int port;
    [SerializeField]
    private bool isDebug;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
        oscClient = new OscClient(ip, port);
    }

    public void PlaySound(string soundName)
    {
        if (oscClient != null)
        {
            oscClient.Send($"/{soundName}", 1, 0);
            if(isDebug)
                Debug.Log($"Sent OSC message to play sound: {soundName}");
        }
        else
        {
            Debug.LogError("OSC Client is not initialized.");
        }
    }
}
