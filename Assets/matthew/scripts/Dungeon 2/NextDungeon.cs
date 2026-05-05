using UnityEngine;

public class NextDungeon : MonoBehaviour
{
    public DungeronGenerator2 dunGen;
    bool hasTriggered = false;

    private void Start()
    {
        dunGen = FindFirstObjectByType<DungeronGenerator2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered) 
        {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        dunGen.GenNextDungeon();
        hasTriggered = true;
        }
        if (!hasTriggered)
        {
            hasTriggered = false; 
        }

    }
}
