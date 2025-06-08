using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Transform enemyParent;

    [SerializeField] private GameObject enemyIconPrefab; // Icon on UI
    [SerializeField] private RectTransform uiMap; // The UI map panel

    private List<EnemyData> activeEnemies = new List<EnemyData>();
    private int nextEnemyID = 0;
    private Vector3 spawnCenter = new Vector3(0, 0, -4);

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
        Vector3 direction = new Vector3(Mathf.Sin(angle), 2f, Mathf.Cos(angle));
        return spawnCenter + direction * spawnDistance;
    }

    void DestroyEnemyByID(int id)
    {
        EnemyData data = activeEnemies.Find(e => e.ID == id);
        if (data != null)
        {
            Destroy(data.EnemyObject);
            Destroy(data.UIIcon);
            activeEnemies.Remove(data);
        }
    }

    void Update()
    {
        // Update icon positions in case enemies move (optional)
        foreach (var data in activeEnemies)
        {
            UpdateIconPosition(data);
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
}
