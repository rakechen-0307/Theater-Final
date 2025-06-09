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
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            oscClient = new OscClient(ip, port);
        }
        else if (_instance != this)
            Destroy(gameObject);
    }

    public void PlaySound(string soundName, int start)
    {
        if (oscClient != null)
        {
            oscClient.Send($"/{soundName}", start);
            if(isDebug)
                Debug.Log($"Sent OSC message to play sound: {soundName}");
        }
        else
        {
            Debug.LogError("OSC Client is not initialized.");
        }
    }
}
