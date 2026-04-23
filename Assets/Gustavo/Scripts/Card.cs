using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    public UnityEvent<GameObject> onUse;
    public UnityEvent<UiCard> onUpdate;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
    public StatusEffectData statusEffect;
    public void LastStand(UiCard card)
    {
        
        
    }








}