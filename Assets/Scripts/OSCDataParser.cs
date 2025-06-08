using UnityEngine;
using System.Collections.Generic;
using OscJack;

public class EntityData
{
    public int ID;
    public int X;
    public int Y;

    public EntityData(int id, int x, int y)
    {
        ID = id;
        X = x;
        Y = y;
    }
}

public class FrameData
{
    public int FrameNumber;
    public List<EntityData> Entities;

    public FrameData(int frame, List<EntityData> entities)
    {
        FrameNumber = frame;
        Entities = entities;
    }
}

public class OSCDataParser : MonoBehaviour 
{
    private static OSCDataParser _instance;
    public static OSCDataParser Instance { get => _instance; }
    private OscServer oscServer;
    [SerializeField] private string oscAddress;
    [SerializeField] private int port;
    [SerializeField] private bool isDebug;
    [SerializeField] private float realMinWidth = -1.98f;
    [SerializeField] private float realMaxWidth = 2.05f;
    [SerializeField] private float realMinHeight = -0.95f;
    [SerializeField] private float realMaxHeight = 0.97f;
    [SerializeField] private int uiWidth = 1920;
    [SerializeField] private int uiHeight = 1080;

    private FrameData latestFrameData;

    private void ParseHokuyoData(OscDataHandle data)
    {
        int count = data.GetElementCount();
        if ((count - 1) % 3 != 0) 
        {
            Debug.LogWarning("Unexpected Hokuyo data format.");
            return;
        }

        int frameIdx = data.GetElementAsInt(0);
        int numEntities = (count - 1) / 3;

        List<EntityData> entities = new List<EntityData>();

        for (int i = 0; i < numEntities; i++)
        {
            int baseIndex = 1 + i * 3;
            int id = Mathf.RoundToInt(data.GetElementAsFloat(baseIndex));
            float x = data.GetElementAsFloat(baseIndex + 1);
            float y = data.GetElementAsFloat(baseIndex + 2);
            List<int> uiCoord = Real2UI(x, y);

            entities.Add(new EntityData(id, uiCoord[0], uiCoord[1]));
        }

        latestFrameData = new FrameData(frameIdx, entities);

        if (isDebug)
        {
            Debug.Log($"Stored frame {frameIdx} with {entities.Count} entities.");
        }
    }

    // Map position in real world to UI coordinate
    private List<int> Real2UI(float x, float y)
    {
        float normX = Mathf.InverseLerp(realMinWidth, realMaxWidth, x);
        float normY = Mathf.InverseLerp(realMinHeight, realMaxHeight, y);
        int uiX = Mathf.RoundToInt(normX * uiWidth);
        int uiY = Mathf.RoundToInt(normY * uiHeight);

        return new List<int> { uiX, uiY };
    }

    private void OnReceiveHokuyo(string address, OscDataHandle data) 
    {
        ParseHokuyoData(data);
    }

    public FrameData GetLatestFrame()
    {
        return latestFrameData;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        oscServer.MessageDispatcher.AddCallback(oscAddress, OnReceiveHokuyo);
    }
}