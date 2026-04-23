using UnityEngine;


[CreateAssetMenu(menuName = "Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string Name;
    public float DOTAmount;
    public float TickSpeed;
    public float MovementBoost;
    public float Lifetime;

    public GameObject Effectparticles;
}
