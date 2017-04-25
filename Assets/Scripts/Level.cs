using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {
    public string name = "level";
    public GameObject prefab;
    public int maxScore;
    public float snakeSpeed = 5;
}
