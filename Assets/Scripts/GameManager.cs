using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

    #region Inspector
    public List<Level> levels;
    public GameObject playerPrefab;
    public GameObject foodPrefab;
    public LevelGenerator levelGenerator;
    [Space]
    [Header("UI stuff")]
    public Text scoreText;
    public GameObject WonPanel;
    public GameObject LostPanel;
    public GameObject GameCompletePanel;
    public GameObject QuitGameButton;
    #endregion

    public static int currentLevel = -1;
    public static int maxScore;

    private static GameObject currentLevelInstance;
    private static GameObject playerInstance;
    private static GameObject[] foodSpawnPoints;

	void Start () {
        //levelGenerator.Generate(0);
        AudioManager.RegisterAudioSource("background_music_source", GetComponent<AudioSource>());
        LostPanel.SetActive(false);
        WonPanel.SetActive(false);
        QuitGameButton.SetActive(false);
        LoadNextLevel();
    }
	
    void Update()
    {
        //Toggle exit button
        if (Input.GetKeyDown(KeyCode.Escape)) QuitGameButton.SetActive(!QuitGameButton.activeSelf);
    }

	public static void PlayerDied () {
        Instance.LostPanel.SetActive(true);
        if (playerInstance)
        {
            playerInstance.SetActive(false);
        }
    }

    public static void PlayerWon()
    {
        if (playerInstance)
        {
            playerInstance.SetActive(false);
        }
        Instance.WonPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void LoadNextLevel()
    {
        AudioManager.PlaySound("background_music_source", "background_music");

        WonPanel.SetActive(false);
        LostPanel.SetActive(false);
        CancelInvoke();
        if (currentLevel + 1 < Instance.levels.Count)
        {
            if (currentLevelInstance)
            {
                currentLevelInstance.SetActive(false);
                Destroy(currentLevelInstance);
                foodSpawnPoints = null;
            }

            //Spawn the world prefab
            currentLevel += 1;
            currentLevelInstance = Instantiate(Instance.levels[currentLevel].prefab, Vector3.zero, Quaternion.identity);
            maxScore = Instance.levels[currentLevel].maxScore;
            scoreText.text = "2/" + maxScore;
            foodSpawnPoints = GameObject.FindGameObjectsWithTag("FoodSpawnPoint");
            FoodSpawn();
            if (playerInstance == null)
            {
                Transform respawn = GameObject.FindGameObjectWithTag("Respawn").transform;
                playerInstance = Instantiate(Instance.playerPrefab, respawn.position, respawn.rotation);
                playerInstance.GetComponent<SnakeController>().moveRate = Instance.levels[currentLevel].snakeSpeed;
            } else
            {
                //remove all the children apart from the first one
                for (int i = 1; i < playerInstance.transform.childCount; i++)
                {
                    Destroy(playerInstance.transform.GetChild(i).gameObject);
                }
                playerInstance.transform.GetChild(0).position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
                playerInstance.transform.GetChild(0).rotation = GameObject.FindGameObjectWithTag("Respawn").transform.rotation;
                playerInstance.GetComponent<SnakeController>().moveRate = Instance.levels[currentLevel].snakeSpeed;
                //wait until the segments have been removed and then enable the snake
                StartCoroutine(WaitUntilChildrenRemovedThenCallback(playerInstance.transform, 1, () => playerInstance.SetActive(true)));
            }


        } else
        {
            Debug.Log("Finished game");
            GameCompletePanel.SetActive(true);
        }
    }

    IEnumerator WaitUntilChildrenRemovedThenCallback(Transform t, int count, Action callback)
    {
        while (t.childCount > count)
        {
            yield return null;
        }

        callback();
    }

    public void FoodSpawn()
    {
        if (foodSpawnPoints == null) return;
        Vector3 position = foodSpawnPoints[UnityEngine.Random.Range(0, foodSpawnPoints.Length)].transform.position;
        Instantiate(foodPrefab, position, Quaternion.identity, currentLevelInstance.transform);
    }

    public void RestartLevel()
    {
        currentLevel -= 1;
        LoadNextLevel();
    }

    public static float GetSnakeSpeed()
    {
        return Instance.levels[currentLevel].snakeSpeed;
    }
}
