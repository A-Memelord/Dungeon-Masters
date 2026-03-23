using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class DungeronGenerator1 : MonoBehaviour
{
    public GameObject[] rooms;
    public GameObject roomParent;

    public Bounds dungeon;
    public int roomCount;
    public float boundsExtra;
    public bool stressTestDungeonGen;
    bool roomsIntersecting;

    private List<Bounds> _allBounds = new();

    GameObject GetRandomRoom()
    {
        int randomIndex = Random.Range(0, rooms.Length);

        return rooms[randomIndex];
    }

    Quaternion GetRandomRotation()
    {
        int[] rotations = new int[] { 0, 90, 180, 270 };
        int rotation = rotations[Random.Range(0, rotations.Length)];

        return Quaternion.Euler(0, rotation, 0);
    }

    Vector3 GetRandomPosition(Vector3 size)
    {
        float spawnPointX = Random.Range(dungeon.min.x, dungeon.max.x);
        float spawnPointY = Random.Range(dungeon.min.y, dungeon.max.y);
        float spawnPointZ = Random.Range(dungeon.min.z, dungeon.max.z);
        Vector3 position = new(spawnPointX, spawnPointY, spawnPointZ);

        Bounds oldBounds = new(position, size);
        oldBounds.Expand(boundsExtra);
        if (_allBounds.Any(room => room.Intersects(oldBounds)) || roomsIntersecting == true)
        {
            roomsIntersecting = true;
            return position;
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

    public void SpawnRoom()
    {
        GameObject newRoom = Instantiate(GetRandomRoom(), Vector3.zero, GetRandomRotation());
        newRoom.transform.SetParent(roomParent.transform);

        if (newRoom.TryGetComponent(out BoxCollider box))
        {
            Vector3 spawnPoint = GetRandomPosition(box.bounds.size);
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
        
        for (int i = 0; i < roomCount; i++)
        {
            SpawnRoom();
            
        }
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
        Gizmos.DrawWireCube(dungeon.center, dungeon.size);
    }
}
