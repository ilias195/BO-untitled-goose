using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{

    [SerializeField] private List<Transform> points = new List<Transform>();

    public List<Transform> Points
    {
        get // als iemand om de lijst vraagt geef het
        {
            return points;
        }
    }
}
