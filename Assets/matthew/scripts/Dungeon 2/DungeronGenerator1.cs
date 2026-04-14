using JetBrains.Annotations;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct Region
{
    public string name;
    public Bounds bounds;
    public GameObject[] rooms;
    public int roomCount;
}

public class DungeronGenerator1 : MonoBehaviour
{
    public Region[] regions;

    public GameObject player;
    public GameObject roomParent;

    public float gridSize = 1;
    public float boundsExtra;
    public bool stressTestDungeonGen;
    bool roomsIntersecting;

    private List<Bounds> _allBounds = new();

    GameObject GetRandomRoom(Region region)
    {
        int randomIndex = Random.Range(0, region.rooms.Length);

        return region.rooms[randomIndex];
    }

    Quaternion GetRandomRotation()
    {
        int[] rotations = new int[] { 0, 90, 180, 270 };
        int rotation = rotations[Random.Range(0, rotations.Length)];

        return Quaternion.Euler(0, rotation, 0);
    }

    Vector3 GetRandomPosition(Region region, Vector3 size)
    {
        float spawnPointX = Mathf.RoundToInt(Random.Range(region.bounds.min.x, region.bounds.max.x) / gridSize) * gridSize;
        float spawnPointY = Mathf.RoundToInt(Random.Range(region.bounds.min.y, region.bounds.max.y) / gridSize) * gridSize;
        float spawnPointZ = Mathf.RoundToInt(Random.Range(region.bounds.min.z, region.bounds.max.z) / gridSize) * gridSize;

        Vector3 position = new(spawnPointX, spawnPointY, spawnPointZ);

        Bounds oldBounds = new(position, size);
        oldBounds.Expand(boundsExtra);
        if (_allBounds.Any(room => room.Intersects(oldBounds)) || roomsIntersecting == true)
        {
            if (region.name == "Shop Room")
            {
                return GetRandomPosition(region, size);
            }
            else
            {
                roomsIntersecting = true;
                return position;
            } 
        }
        else
        {
            roomsIntersecting = false;
            return position;
        }


    }

    void Update()
    {
        if (stressTestDungeonGen == true)
        {
            SpawnAllRooms();
        }
    }

    public void SpawnRoom(Region region)
    {
        GameObject newRoom = Instantiate(GetRandomRoom(region), Vector3.zero, GetRandomRotation());
        newRoom.transform.SetParent(roomParent.transform);

        if (newRoom.TryGetComponent(out BoxCollider box))
        {
            Vector3 spawnPoint = GetRandomPosition(region, box.bounds.size);
            newRoom.transform.position = spawnPoint;
            if(roomsIntersecting == true)
            {
                DestroyImmediate(newRoom);
                roomsIntersecting = false;
            }
            else
            {
                _allBounds.Add(box.bounds);
            }
        }
    }

    public void SpawnAllRooms()
    {
        ClearRooms();
        Debug.Log("RoomSpawnCounter");

        // Generate all rooms by their region.
        foreach(Region region in regions)
        {
            for (int i = 0; i < region.roomCount; i++)
            {
                SpawnRoom(region); 
            } 
        }
        
        GameObject startRoom = GameObject.FindGameObjectWithTag("StartRoom");

        Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None).OrderBy(room => Vector3.Distance(room.transform.position, startRoom.transform.position)).ToArray();
        List<Door> allDoors = FindObjectsByType<Door>(FindObjectsSortMode.None).ToList();

        // Generate all corridors between rooms.
        foreach (var room in allRooms)
        {
            Door[] doors = room.GetComponentsInChildren<Door>();

            foreach (var door in doors)
            {
                Door nextDoor = allDoors
                    .Where(item => item.transform.parent.parent != room.transform)
                    .OrderBy(item => Vector3.Distance(item.transform.position, door.transform.position))
                    .FirstOrDefault();

                if (nextDoor == null)
                    continue;

                GameObject line = new GameObject();
                LineRenderer lr = line.AddComponent<LineRenderer>();

                lr.SetPositions(new Vector3[] { door.transform.position, nextDoor.transform.position });

                allDoors.Remove(door);
                allDoors.Remove(nextDoor);
            }
        }

        // Teleport the player to the start room.
        float playerSpawnPointX = startRoom.transform.position.x;
        float playerSpawnPointY = startRoom.transform.position.y;
        float playerSpawnPointZ = startRoom.transform.position.z;

        Vector3 playerSpawnPoint = new(playerSpawnPointX, playerSpawnPointY + 1.1f, playerSpawnPointZ);
        player.transform.position = playerSpawnPoint;
    }

    public void ClearRooms()
    {
        int childedRooms = _allBounds.Count;
        _allBounds.Clear();
        
        for (int i = 0; i < childedRooms; i++)
        {
            DestroyImmediate(roomParent.transform.GetChild(0).gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        foreach(Region region in regions)
        {
            Gizmos.DrawWireCube(region.bounds.center, region.bounds.size);
        }
    }

}
