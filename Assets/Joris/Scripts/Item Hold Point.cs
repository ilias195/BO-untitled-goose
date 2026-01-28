using UnityEngine;

public class ItemHoldPoint : MonoBehaviour
{
    [Header("Drag the hold point (empty GameObject) here")]
    public Transform[] holdPoint;

    [Header("Item weight")]
    public bool isHeavy = false; // mark in inspector if item is heavy
}
