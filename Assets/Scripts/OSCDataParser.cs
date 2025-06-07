using UnityEngine;
using System.Collections.Generic;
using OscJack;

public class EntityData
{
    public int ID;
    public float X;
    public float Y;

    public EntityData(int id, float x, float y)
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

            entities.Add(new EntityData(id, x, y));
        }

        latestFrameData = new FrameData(frameIdx, entities);

        if (isDebug)
        {
            Debug.Log($"Stored frame {frameIdx} with {entities.Count} entities.");
        }
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