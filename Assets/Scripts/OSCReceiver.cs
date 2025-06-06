using UnityEngine;
using OscJack;

public class OSCReceiver : MonoBehaviour
{
    private static OSCReceiver _instance;
    public static OSCReceiver Instance { get => _instance; }
    private OscServer oscServer;
    [SerializeField] private string oscAddress;
    [SerializeField] private int port;
    [SerializeField]
    private bool isDebug;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        oscServer = new OscServer(port);
        oscServer.MessageDispatcher.AddCallback(oscAddress, OnReceiveExample);
    }

    private void OnReceiveExample(string address, OscDataHandle data)
    {
        // Example: Expecting a float and an int
        float value1 = data.GetElementAsFloat(0);

        if (isDebug)
        {
            Debug.Log($"Received on {address}: float={value1}");
        }

        // TODO: Handle the data (e.g., update a variable, trigger behavior, etc.)
    }

    private void OnDestroy()
    {
        // Clean up
        oscServer?.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
