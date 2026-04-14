using System.Collections.Generic;
using UnityEngine;

public class NpcPathFinder : MonoBehaviour
{
    public List<GameObject> Nodes = new List<GameObject>();
    public Transform PathParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i <= Nodes.Count; i++)
        {
            Nodes.Add(PathParent.GetChild(i).gameObject);

            //gameObject.transform = Nodes[i].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
