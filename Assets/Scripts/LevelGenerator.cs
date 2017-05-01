using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubePosition
{
    inside = 1,
    face = 2,
    corner = 4,
    edge = 8
}

public class LevelGenerator : MonoBehaviour {
    public AnimationCurve obstacleFrequency;
    public AnimationCurve worldSize;
    public GameObject[] obstaclePrefabs;
    public GameObject groundPrefab;
    public GameObject foodSpawnPrefab;
    public GameObject playerSpawnPrefab;
    public int totalLevels;


    public GameObject Generate(int levelIndex)
    {
        Transform parent = new GameObject("Level_" + levelIndex).transform;
        int cubeSize = GetWorldSizeFromLevel(levelIndex);
        float start = -cubeSize / 2 + 0.5f;
        float finish = cubeSize / 2 - 0.5f;
        List<Vector3> keepClear = new List<Vector3>(); //List of game objects to keep clear of obstacles
        float obstacleSpawnProbability = GetObstacleFrequency(levelIndex);
        for (float x = start; x <= finish; x++)
        {
            for (float y = start; y <= finish; y++)
            {
                for (float z = start; z <= finish; z++)
                {
                    //Create ground
                    Vector3 position = new Vector3(x, y, z);
                    Quaternion rotation = CubeRotationFromPoint(cubeSize, position);
                    GameObject newVoxel = Instantiate(groundPrefab, position, rotation, parent);
                }
            }
        }
        return parent.gameObject;
    }

    GameObject CreateVoxelCube(int size)
    {

        return null;
    }

    void AddStuffToPlanet(Transform planet, int cubeSize)
    {

    }

    void InstantiateObstacleOrFoodSpawnOnCube(Transform cube, float obstacleSpawnProbability, Transform parent, bool forceCreate = false)
    {
        if (obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("No obstacle prefabs set");
            return;
        }
        if (Random.value < obstacleSpawnProbability || forceCreate)
        {
            Vector3 position = cube.position - cube.forward*0.5f; //half a cube behind
            Quaternion rotation = cube.rotation;
            int rand = Random.Range(0, obstaclePrefabs.Length);
            GameObject prefab = obstaclePrefabs[rand];
            GameObject newObstacle = Instantiate(prefab, position, rotation, parent);
        }
        else
        {
            //Create food spawnpoint
            Vector3 position = cube.position - cube.forward; //half a cube behind
            GameObject newObstacle = Instantiate(foodSpawnPrefab, position, Quaternion.identity, parent);
        }
    }

    List<Vector3> InstantiatePlayerSpawnOnCube(Transform block, Transform parent, int cubeSize)
    {
        List<Vector3> keepClear = new List<Vector3>();
        Vector3 position = block.position - block.forward;
        Quaternion rotation = Quaternion.FromToRotation(block.forward, block.right);
        GameObject playerSpawn = Instantiate(playerSpawnPrefab, position, rotation, parent);
        Vector3 clearPosition = playerSpawn.transform.position;
        Vector3 clearForward = playerSpawn.transform.forward;
        //Clear 3 spaces in front of it so we don't run into something straight away
        for (int i = 0; i < 12; i++)
        {
            Debug.Log(clearPosition);
            keepClear.Add(position);

            clearPosition += clearForward;
        }
        return keepClear;
    }

    public CubePosition GetCubePositionFromPoint(int cubeSize, Vector3 point)
    {
        float min = -cubeSize / 2 + 0.5f;
        float max = cubeSize / 2 - 0.5f;
        bool xOnMin = point.x == min;
        bool xOnMax = point.x == max;
        bool yOnMin = point.y == min;
        bool yOnMax = point.y == max;
        bool zOnMin = point.z == min;
        bool zOnMax = point.z == max;
        //Debug.Log("Max: " + max);
        //Debug.Log("Min: " + min);
        //Debug.Log(point);
        if ((xOnMin && yOnMin) || (xOnMax && yOnMin) || (xOnMin && yOnMax) || (xOnMax && yOnMax) ||
            (xOnMin && zOnMin) || (xOnMax && zOnMin) || (xOnMin && zOnMax) || (xOnMax && zOnMax) ||
            (yOnMin && zOnMin) || (yOnMax && zOnMin) || (yOnMin && zOnMax) || (yOnMax && zOnMax))
        {
            return CubePosition.edge;
        }
        else if ((point.x > min && point.x < max) && (point.y > min && point.y < max) && (point.z > min && point.z < max))
        {
            return CubePosition.inside;
        }
        else
        {
            return CubePosition.face;
        }
        
    }

    public Quaternion CubeRotationFromPoint(int cubeSize, Vector3 point)
    {
        Vector3 vec = Quaternion.LookRotation((Vector3.zero - point).normalized).eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        return Quaternion.Euler(vec);
    }

    public Quaternion PlayerRotationFromPoint(Vector3 point)
    {
        Vector3 vec = Quaternion.LookRotation((Vector3.zero - point).normalized).eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        return Quaternion.Euler(vec);
    }

    public int GetWorldSizeFromLevel(int levelIndex)
    {
        //how far through all the levels we are
        float levelProportion = (float)levelIndex / (float)totalLevels;
        float maxTime = worldSize[worldSize.length - 1].time;
        return (int) worldSize.Evaluate(maxTime * levelProportion);
    }

    public float GetObstacleFrequency(int levelIndex)
    {
        //how far through all the levels we are
        float levelProportion = (float) levelIndex / totalLevels;
        float maxTime = obstacleFrequency[worldSize.length - 1].time;
        return obstacleFrequency.Evaluate(maxTime * levelProportion);
    }

    [System.Serializable]
    public class Obstacle
    {

    }
}
