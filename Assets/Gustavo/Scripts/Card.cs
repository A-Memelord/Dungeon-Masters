using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    public UnityEvent onUse;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
}
