using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public bool isCleared = false;
    private Bounds enemySpawnArea;
    public GameObject[] enemies;


    //GameObject GetRandomEnemie(GameObject enemies)
    //{
    //    int randomIndex = Random.Range(0, region.rooms.Length);

    //    return region.rooms[randomIndex];
    //}

    Vector3 GetRandomPosition(Bounds enemySpawnArea, Vector3 size)
    {
        float spawnPointX = Mathf.RoundToInt(Random.Range(enemySpawnArea.min.x, enemySpawnArea.max.x));
        float spawnPointY = Mathf.RoundToInt(Random.Range(enemySpawnArea.min.y, enemySpawnArea.max.y));
        float spawnPointZ = Mathf.RoundToInt(Random.Range(enemySpawnArea.min.z, enemySpawnArea.max.z));

        Vector3 position = new(spawnPointX, spawnPointY, spawnPointZ);
        return position;
    }

    public void SpawnAllEnemies()
    {

    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(enemySpawnArea.center, enemySpawnArea.size);
    }
}
