using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class DungeronGenerator1 : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject endRoom;
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

    public void SpawnStartEndAndShopRooms()
    {
        if (roomsIntersecting == true)
        {
            ClearRooms();
            roomsIntersecting = false;
        }

        GameObject startRoom_ = Instantiate(startRoom, Vector3.zero, GetRandomRotation());
        startRoom_.transform.SetParent(roomParent.transform);
        //GameObject shop_ = Instantiate(shop, Vector3.zero, GetRandomRotation());
        //shop_.transform.SetParent(roomParent.transform);
        GameObject endRoom_ = Instantiate(endRoom, Vector3.zero, GetRandomRotation());
        endRoom_.transform.SetParent(roomParent.transform);

        if (startRoom_.TryGetComponent(out BoxCollider startBox))
        {
            Vector3 spawnPoint = GetRandomPosition(startBox.bounds.size);
            startRoom_.transform.position = spawnPoint;

            _allBounds.Add(startBox.bounds);
        }
        if (endRoom_.TryGetComponent(out BoxCollider endBox))
        {
            Vector3 spawnPoint = GetRandomPosition(endBox.bounds.size);
            endRoom_.transform.position = spawnPoint;

            _allBounds.Add(endBox.bounds);
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

        SpawnStartEndAndShopRooms();
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
