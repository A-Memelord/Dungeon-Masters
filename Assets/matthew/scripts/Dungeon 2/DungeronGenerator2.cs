using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class DungeronGenerator2 : MonoBehaviour
{
    public Region[] regions;
    public GameObject[] hallways;

    public GameObject player;
    public GameObject roomParent;
    public GameObject hallwayParent;


    public float gridSize = 1;
    public float boundsExtra;
    public bool stressTestDungeonGen;
    bool roomsIntersecting;

    public Material lineMat;
    private List<Bounds> _allBounds = new();

    // gets random room in regions
    GameObject GetRandomRoom(Region region)
    {
        int randomIndex = Random.Range(0, region.rooms.Length);

        return region.rooms[randomIndex];
    }

    // gets random rotation for rooms
    Quaternion GetRandomRotation()
    {
        int[] rotations = new int[] { 0, 90, 180, 270 };
        int rotation = rotations[Random.Range(0, rotations.Length)];

        return Quaternion.Euler(0, rotation, 0);
    }

    // gets random position for rooms
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

        Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None)
            .OrderBy(room => Vector3.Distance(room.transform.position, startRoom.transform.position))
            .ToArray();
        
        List<Door> allDoors = FindObjectsByType<Door>(FindObjectsSortMode.None)
            .ToList();

        // Generate all corridors between rooms.
        foreach (var room in allRooms)
        {
            room.GetComponentsInChildren<Door>();
            
            Door[] roomDoors = room.GetComponentsInChildren<Door>();


            foreach (var door in roomDoors)
            {
                Door nextDoor = allDoors
                    .Where(item => item.transform.parent.parent != room.transform)
                    .OrderBy(item => Vector3.Distance(item.transform.position, door.transform.position))
                    .FirstOrDefault();

                if (nextDoor == null)
                    continue;

                CreateCorridor(door, nextDoor);

                allDoors.Remove(door);
                allDoors.Remove(nextDoor);

                DestroyImmediate(door);
                DestroyImmediate(nextDoor);
            }
        }

        // Teleport the player to the start room.
        float playerSpawnPointX = startRoom.transform.position.x;
        float playerSpawnPointY = startRoom.transform.position.y;
        float playerSpawnPointZ = startRoom.transform.position.z;

        Vector3 playerSpawnPoint = new(playerSpawnPointX, playerSpawnPointY + 1.1f, playerSpawnPointZ);
        player.transform.position = playerSpawnPoint;
    }

    private void CreateCorridor(Door doorA, Door doorB)
    {
        //GameObject line = new();
        //LineRenderer lr = line.AddComponent<LineRenderer>();
        //line.transform.SetParent(hallwayParent.transform);

        Vector3[] positions = Mathf.Abs(Vector3.Dot(doorA.transform.forward, doorB.transform.forward)) < 0.1f ?
            GenerateCornerCorridor(doorA, doorB) : Vector3.Angle(doorA.transform.right, Vector3.right) >= 10f ?
            GenerateHorizontalCorridor(doorA, doorB) :
            GenerateVerticalCorridor(doorA, doorB);
        
        //lr.endColor = lr.startColor = Mathf.Abs(Vector3.Dot(doorA.transform.forward, doorB.transform.forward)) < 0.1f ?
        //    Color.yellow : Vector3.Angle(doorA.transform.right, Vector3.right) >= 10f ?
        //    Color.red :
        //    Color.blue;
        //lr.material = lineMat;

        doorA.GetComponentInChildren<PortalTeleport>().linkedPortal = doorB.GetComponentInChildren<PortalTeleport>();
        doorA.GetComponentInChildren<PortalCamera>().otherPortal = doorB.GetComponentInChildren<PortalTeleport>().transform;

        doorB.GetComponentInChildren<PortalTeleport>().linkedPortal = doorA.GetComponentInChildren<PortalTeleport>();
        doorB.GetComponentInChildren<PortalCamera>().otherPortal = doorA.GetComponentInChildren<PortalTeleport>().transform;

    //    lr.positionCount = positions.Length;
    //    lr.SetPositions(positions);
    }

    private Vector3[] GenerateHorizontalCorridor(Door doorA, Door doorB)
    {
        float lerp = Random.Range(0.5f, 0.5f);
        Vector3[] positions = new Vector3[4];

        positions[0] = doorA.transform.position;
        positions[1] = new Vector3
        (
            x: Mathf.Lerp(doorA.transform.position.x, doorB.transform.position.x, lerp),
            y: doorA.transform.position.y,
            z: doorA.transform.position.z
        );
        positions[2] = new Vector3
        (
            x: Mathf.Lerp(doorA.transform.position.x, doorB.transform.position.x, lerp),
            y: doorB.transform.position.y,
            z: doorB.transform.position.z
        );
        positions[3] = doorB.transform.position;

        return positions;
    }

    private Vector3[] GenerateVerticalCorridor(Door doorA, Door doorB)
    {
        float lerp = Random.Range(0.5f, 0.5f);
        Vector3[] positions = new Vector3[4];

        positions[0] = doorA.transform.position;
        positions[1] = new Vector3
        (
            x: doorA.transform.position.x,
            y: doorA.transform.position.y,
            z: Mathf.Lerp(doorA.transform.position.z, doorB.transform.position.z, lerp)
        );
        positions[2] = new Vector3
        (
            x: doorB.transform.position.x,
            y: doorB.transform.position.y,
            z: Mathf.Lerp(doorA.transform.position.z, doorB.transform.position.z, lerp)
        );
        positions[3] = doorB.transform.position;

        return positions;
    }

    private Vector3[] GenerateCornerCorridor(Door doorA, Door doorB)
    {
        Vector3[] positions = new Vector3[3];

        positions[0] = doorA.transform.position;
        positions[1] = new Vector3
        (
            x: doorA.transform.position.x,
            y: doorA.transform.position.y,
            z: doorB.transform.position.z
        );
        positions[2] = doorB.transform.position;

        return positions;
    }

    // Destroys entire dungeon
    public void ClearRooms()
    {
        int childedRooms = roomParent.transform.childCount;
        _allBounds.Clear();
        int allDoors = hallwayParent.transform.childCount;
        

        for (int i = 0; i < childedRooms; i++)
        {
            DestroyImmediate(roomParent.transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < allDoors; i++)
        {
            DestroyImmediate(hallwayParent.transform.GetChild(0).gameObject);
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
