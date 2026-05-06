using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject player;
    public GameObject enemyParent;
    public GameObject doorBlocker;
    Collider pcol;
    public bool isCleared = false;
    public Bounds enemySpawnArea;
    public GameObject[] enemies;
    private bool hasSpawnedEnemies = false;
    public bool closedDoors = false;

    void Update()
    {
        pcol = player.GetComponent<BoxCollider>();
        if (pcol.bounds.Intersects(enemySpawnArea) && isCleared == false && hasSpawnedEnemies == false)
        {
            hasSpawnedEnemies = true;
            closedDoors = true;
            SpawnAllEnemies();
        }
        if (enemyParent.transform.childCount == 0 && hasSpawnedEnemies == true)
        {
            isCleared = true;
        }
        if (isCleared == true)
        {
            closedDoors = false;
        }
        if (closedDoors == true)
        {
            doorBlocker.SetActive(true);
        }
        else
        {
            doorBlocker.SetActive(false);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hasSpawnedEnemies = false;
        enemySpawnArea.center = transform.position;
        enemySpawnArea.center += new Vector3(0, 1, 0);
    }

    GameObject GetRandomEnemie()
    {
        int randomIndex = Random.Range(0, enemies.Length);

        return enemies[randomIndex];
    }

    Vector3 GetRandomPosition(Bounds enemySpawnArea, Vector3 size)
    {
        float spawnPointX = Mathf.RoundToInt(Random.Range(enemySpawnArea.min.x, enemySpawnArea.max.x));
        float spawnPointY = Mathf.RoundToInt(Random.Range(enemySpawnArea.min.y, enemySpawnArea.max.y));
        float spawnPointZ = Mathf.RoundToInt(Random.Range(enemySpawnArea.min.z, enemySpawnArea.max.z));

        Vector3 position = new(spawnPointX, spawnPointY, spawnPointZ);
        return position;
    }

    public void SpawnEnemie()
    {
        GameObject newEnemie = Instantiate(GetRandomEnemie(), Vector3.zero, Quaternion.identity);

        if (newEnemie.TryGetComponent(out CapsuleCollider capsule))
        {
            Vector3 spawnPoint = GetRandomPosition(enemySpawnArea, capsule.bounds.size);
            newEnemie.transform.position = spawnPoint;
            newEnemie.transform.parent = enemyParent.transform;
        }
        
    }

    public void SpawnAllEnemies()
    {
        int enemieCount = Random.Range(2, 6);
        for (int i = 0; i < enemieCount; i++)
        {
            SpawnEnemie();
        }
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(enemySpawnArea.center, enemySpawnArea.size);
    }
}
