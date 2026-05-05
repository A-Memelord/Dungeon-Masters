using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public bool isCleared = false;
    private Bounds enemySpawnArea;
    public GameObject[] enemies;


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

        if (newEnemie.TryGetComponent(out BoxCollider box))
        {
            Vector3 spawnPoint = GetRandomPosition(enemySpawnArea, box.bounds.size);
            newEnemie.transform.position = spawnPoint;
        }
    }

    public void SpawnAllEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            SpawnEnemie();
        }
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(enemySpawnArea.center, enemySpawnArea.size);
    }
}
