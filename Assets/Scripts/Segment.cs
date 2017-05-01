using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    public Transform transform;
    public Queue<Turn> turnPoints;

    public Segment(Transform transform)
    {
        this.transform = transform;
        turnPoints = new Queue<Turn>();
    }
}