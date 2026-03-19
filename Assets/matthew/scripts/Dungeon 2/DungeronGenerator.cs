using UnityEngine;

public class DungeronGenerator : MonoBehaviour
{
    
    public GameObject[] rooms;

    public GameObject roomParent;


    public void SpawnRooms()
    {
        int spawnPointX = Random.Range(-50, 50);
        int spawnPointY = Random.Range(50, 100);
        int spawnPointZ = Random.Range(-50, 50);

        Vector3 spawnPosition = new Vector3(spawnPointX, 0, spawnPointZ);

        CreateChildPrefabInstance(rooms, roomParent, spawnPosition);
    }

    void CreateChildPrefabInstance(GameObject[] rooms, GameObject parent, Vector3 spawnPosition)
    {

        int randomIndex = Random.Range(0, rooms.Length); 



        var newGameObject = Instantiate(rooms[randomIndex], spawnPosition, Quaternion.identity);
        newGameObject.transform.parent = parent.transform;
    }
}
