using UnityEngine;

public class NextDungeon : MonoBehaviour
{
    public DungeronGenerator2 dunGen;
    private void Start()
    {
        dunGen = FindFirstObjectByType<DungeronGenerator2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        dunGen.GenNextDungeon();

        Destroy(gameObject);

    }
}
