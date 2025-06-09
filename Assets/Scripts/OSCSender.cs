using UnityEngine;
using OscJack;

public class OSCSender : MonoBehaviour
{
    private static OSCSender _instance;
    public static OSCSender Instance { get => _instance; }
    private OscClient oscClient;
    [SerializeField] private string ip = "192.168.2.103";
    [SerializeField] private int port = 9995;
    [SerializeField] private bool isDebug;

    private void Awake()
    {
        Debug.Log("Awake");
        if (_instance == null)
        {
            Debug.Log("Null");
            _instance = this;
            DontDestroyOnLoad(gameObject);
            oscClient = new OscClient(ip, port);
        }
        else if (_instance != this) {
            Debug.Log("Destroy");
            Destroy(gameObject);
        }
    }

    public void PlaySound(string soundName, int start)
    {
        Debug.Log("PLAY SOUND");
        if (oscClient != null)
        {
            oscClient.Send($"/{soundName}", start);
            /*
            if(isDebug)
            */
            Debug.Log($"Sent OSC message to play sound: {soundName}");
        }
        else
        {
            Debug.LogError("OSC Client is not initialized.");
        }
    }
}
