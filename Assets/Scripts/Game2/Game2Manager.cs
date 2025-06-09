using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyData
{
    public int ID;
    public GameObject EnemyObject;
    public GameObject UIIcon;

    public EnemyData(int id, GameObject obj, GameObject icon)
    {
        ID = id;
        EnemyObject = obj;
        UIIcon = icon;
    }
}

public class Game2Manager : MonoBehaviour
{
    [SerializeField] private float spawnAngle = 60f;
    [SerializeField] private float spawnDistance = 5f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnPeriod = 1f;
    [SerializeField] private float spawnProb = 0.2f;
    [SerializeField] private float hitRadius = 20f;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private GameObject enemyIconPrefab;
    [SerializeField] private RectTransform uiMap;
    [SerializeField] private int uiWidth = 1920;
    [SerializeField] private int uiHeight = 1080;
    [SerializeField] private float minY = -0.3f;
    [SerializeField] private float maxY = 0.25f;
    [SerializeField] private int completeCount = 20;
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool isDebug = false;

    private List<EnemyData> activeEnemies = new List<EnemyData>();
    private int nextEnemyID = 0;
    private Vector3 spawnCenter = new Vector3(0, 0, -6);
    private int hitCount = 0;

    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            if (activeEnemies.Count < 5 && Random.value < spawnProb)
            {
                Vector3 pos = GenerateRandomPosition();
                GameObject enemyObj = Instantiate(enemyPrefab, pos, Quaternion.identity, enemyParent);
                enemyObj.SetActive(true);

                int id = nextEnemyID++;
                GameObject icon = Instantiate(enemyIconPrefab, uiMap); // For clicking on UI
                // TODO: remove this line when using Hokuyo
                icon.GetComponent<Button>().onClick.AddListener(() => DestroyEnemyByID(id));
                icon.SetActive(true);

                EnemyData data = new EnemyData(id, enemyObj, icon);
                activeEnemies.Add(data);

                UpdateIconPosition(data);
            }

            yield return new WaitForSeconds(spawnPeriod);
        }
    }

    Vector3 GenerateRandomPosition()
    {
        float angle = Random.Range(-spawnAngle / 2f, spawnAngle / 2f) * Mathf.Deg2Rad;
        float y = Random.Range(minY, maxY);
        Vector3 direction = new Vector3(Mathf.Sin(angle), y, Mathf.Cos(angle));
        return spawnCenter + direction * spawnDistance;
    }

    void DestroyEnemyByID(int id)
    {
        EnemyData data = activeEnemies.Find(e => e.ID == id);
        if (data != null)
        {
            OSCSender.Instance.PlaySound("shot", 1);
            Destroy(data.EnemyObject);
            Destroy(data.UIIcon);
            activeEnemies.Remove(data);
            hitCount++;
        }
    }

    void Update()
    {
        /*
        // Receive Hokuyo data
        FrameData data = OSCDataParser.Instance.GetLatestFrame();
        if (data != null)
        {
            if (isDebug) {
                foreach (var entity in data.Entities)
                {
                    Debug.Log($"Entity {entity.ID}: X={entity.X}, Y={entity.Y}");
                }
            }
            CheckHits(data);
        }
        */

        if (hitCount >= completeCount)
        {
            GoToNextScene();
            return;
        }

        // Update icon positions in case enemies move (optional)
        foreach (var enemy in activeEnemies)
        {
            UpdateIconPosition(enemy);
        }
    }

    void UpdateIconPosition(EnemyData data)
    {
        Vector3 enemyPos = data.EnemyObject.transform.position;
        Vector2 mapPos = new Vector2(enemyPos.x, enemyPos.z); // Flatten to 2D

        // Map world position to UI space (assuming centered UI map)
        Vector2 anchoredPos = new Vector2(
            (mapPos.x / (spawnDistance * 2)) * uiMap.rect.width,
            (mapPos.y / (spawnDistance * 2)) * uiMap.rect.height
        );
        data.UIIcon.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
    }

    public void CheckHits(FrameData dataPoints)
    {
        foreach (var entity in dataPoints.Entities)
        {
            Vector2 point = new Vector2(entity.X, entity.Y);
            /*
            if (isDebug)
            {
                Debug.Log($"Sensor Point: {point}");
            }
            */

            foreach (var enemy in new List<EnemyData>(activeEnemies))
            {
                RectTransform iconRect = enemy.UIIcon.GetComponent<RectTransform>();
                Vector2 pos = iconRect.anchoredPosition;
                Vector2 iconPos = new Vector2(
                    uiWidth / 2 + pos.x,
                    uiHeight / 2 + pos.y
                );

                if (isDebug)
                {
                    Debug.Log($"Icon Pos: {iconPos}");
                }

                if (Vector2.Distance(point, iconPos) < hitRadius)
                {
                    DestroyEnemyByID(enemy.ID);
                    break;
                }
            }
        }
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
