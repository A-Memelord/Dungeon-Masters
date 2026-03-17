using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject ceilingPrefab;

    public GameObject floorParent;
    public GameObject wallParent;

    public GameObject Player;

    public bool isRoofNeeded = true;

    public int tilesToRemove = 350;

    public int mapSize = 30;

    bool isPlayerPlaced = false;

    bool[,] mapData;

    void Start()
    {
        mapData = GenerateMapData();

        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                if (mapData[z, x])
                {
                    CreateChildPrefabInstance(wallPrefab, wallParent, new Vector3(x, 1, z));
                    CreateChildPrefabInstance(wallPrefab, wallParent, new Vector3(x, 2, z));
                    CreateChildPrefabInstance(wallPrefab, wallParent, new Vector3(x, 3, z));
                    wallPrefab.layer = wallParent.layer;
                }
                else if (!isPlayerPlaced)
                {
                    Player.transform.SetPositionAndRotation(new Vector3(x, 1, z), Quaternion.identity);
                    isPlayerPlaced = true;
                }

                CreateChildPrefabInstance(floorPrefab, floorParent, new Vector3(x, 0, z));
                floorPrefab.layer = floorParent.layer;

                if (isRoofNeeded)
                {
                    CreateChildPrefabInstance(ceilingPrefab, wallParent, new Vector3(x, 4, z));
                    ceilingPrefab.layer = wallParent.layer;
                }
            }
        }
    }

    bool[,] GenerateMapData()
    {
        bool[,] data = new bool[mapSize, mapSize];
        
        // We will initialize all the walls to true
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                data[y, x] = true;
            }
        }

        // Clear out the walls somewhere in the quantity of tilesToRemove

        int tilesConsumed = 0;
        int mapX = 0, mapY = 0;

        // Iterate with our random crawler and clear out the walls needed
        while (tilesConsumed < tilesToRemove)
        {
            int xDirection = 0, yDirection = 0;

            if (Random.value < 0.5)
            {
                xDirection = Random.value < 0.5 ? 1 : -1;
            }
            else if (Random.value < 0.5)
            {
                yDirection = Random.value < 0.5 ? 1 : -1;
            }

            int numSpaceMove = (int)(Random.Range(1, mapSize - 1));

            for (int i = 0; i < numSpaceMove; i++)
            {
                mapX = Mathf.Clamp(mapX + xDirection, 1, mapSize - 2);
                mapY = Mathf.Clamp(mapY + yDirection, 1, mapSize - 2);

                if (data[mapY, mapX])
                {
                    data[mapY, mapX] = false;
                    tilesConsumed++;
                }
            }
        }

        return data;
    }

    void CreateChildPrefabInstance(GameObject prefab, GameObject parent, Vector3 spawnPosition)
    {
        var newGameObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        newGameObject.transform.parent = parent.transform;
    }
}
