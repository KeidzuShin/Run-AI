using Cinemachine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject tutorialScreen;
    public AudioClip gameOverSound;
    public AudioSource audioSource;
    public GameSetting gameSetting;

    [Header("Text Stuff")]
    public GameObject enemyStats;
    public float elapsedTime;
    public float attackDamage = 10f, shootingForce = 40f, regenRate = 0f, maxHealth = 100f, attackRange = 10f, attackCooldown = 0.5f;
    public string printtime, printname;

    public class ScoreData
    {
        public int[] timescoreRP, timescoreAP, timescoreFP;
        public string[] namescoreRP, namescoreAP, namescoreFP;
    }

    [Header("ScoreboardAttackPlayer")]
    public int[] timescoreAP = { 1000, 800, 600, 400, 200, 100, 50, 25, 10, 5, 3, 2, 1 ,0};
    public string[] namescoreAP = { "AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "XXX", "XXX" , "XXX"};

    [Header("ScoreboardRunFromPlayer")]
    public int[] timescoreRP = { 1000, 800, 600, 400, 200, 100, 50, 25, 10, 5, 3, 2, 1 ,0};
    public string[] namescoreRP = { "AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "XXX", "XXX", "XXX" };

    [Header("ScoreboardFullRun")]
    public int[] timescoreFP = { 1000, 800, 600, 400, 200, 100, 50, 25, 10, 5, 3, 2, 1 , 0 };
    public string[] namescoreFP = { "AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG", "HHH", "III", "JJJ", "XXX", "XXX" , "XXX" };

    public class Score
    {
        public int scoreTimeRP;
        public int scoreTimeAP;
        public int scoreTimeFP;
        public string scoreNameRP;
        public string scoreNameAP;
        public string scoreNameFP;
    }

    private void Awake()
    {
        enemyStats = GameObject.FindGameObjectWithTag("EnemyStat");
        gameSetting = Resources.Load<GameSetting>("GameSetting");
        tutorialScreen = GameObject.FindGameObjectWithTag("Tutorial");
    }

    void Start()
    {
        if (pauseScreen.activeInHierarchy)
            UnPause();
        if (gameOverScreen.activeInHierarchy)
            gameOverScreen.SetActive(false);
        if (!tutorialScreen.activeInHierarchy)
            tutorialScreen.SetActive(true);

        if (!File.Exists(Application.persistentDataPath + "/scores.json"))
        {
            ScoreData scoreData = new ScoreData
            {
                timescoreRP = timescoreRP,
                namescoreRP = namescoreRP,
                timescoreAP = timescoreAP,
                namescoreAP = namescoreAP,
                timescoreFP = timescoreFP,
                namescoreFP = namescoreFP
            };
            string jsonData = JsonUtility.ToJson(scoreData);
            File.WriteAllText(Application.persistentDataPath + "/scores.json", jsonData);
        }

        //Freeze game to show tutorial on game start
        Time.timeScale = 0;
        attackDamage = 10f;
        shootingForce = 40f;
        regenRate = 0f; 
        maxHealth = 100f; 
        attackRange = 10f; 
        attackCooldown = 0.5f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (pauseScreen.activeInHierarchy)
                UnPause();
            else
                Pause();
        if (tutorialScreen.activeInHierarchy)
            if (Input.GetMouseButton(0))
                CloseTutorial();

        UpdateElapsedTime();
        UpdateEnemyStats();
    }

    //Activate Pause Screen
    public void Pause()
    {
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }

    //Deactivate Pause Screen
    public void UnPause()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
    }

    //Show GameOver Screen
    public void GameOver()
    {
        Resources.Load<GameSetting>("GameSetting").storeElapsedTime = (int)elapsedTime;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineBrain>().enabled = false; //Disable camera movement
        gameOverScreen.SetActive(true); //Display game over screen
        GameObject.FindGameObjectWithTag("YourTime").GetComponent<Text>().text = GameObject.FindGameObjectWithTag("TimeElapsed").GetComponent<TextMeshProUGUI>().text;
        
        PrintScores();
        audioSource.PlayOneShot(gameOverSound); // Play SFX
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }

    //Restart Game
    public void Restart()
    {
        Time.timeScale = 1;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Animate Main menu transition
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //Close Tutorial Screen
    public void CloseTutorial()
    {
        tutorialScreen.SetActive(false);
        Time.timeScale = 1;
        AudioListener.volume = 1;
        Cursor.lockState = CursorLockMode.Locked;
        UnPause();
    }

    public void UpdateElapsedTime()
    {
        elapsedTime += Time.deltaTime;
        int hours = (int)(elapsedTime / 3600);
        int minutes = (int)((elapsedTime % 3600) / 60);
        int seconds = (int)(elapsedTime % 60);
        GameObject.FindGameObjectWithTag("TimeElapsed").GetComponent<TextMeshProUGUI>().text = string.Format("Time Elapsed : {0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public void UpdateEnemyStats()
    {//Match changes based on AIstats.cs
        attackDamage = Mathf.MoveTowards(attackDamage, 50f, 0.02f * Time.deltaTime);
        attackCooldown = Mathf.MoveTowards(attackCooldown, 0.2f, 0.0001f * Time.deltaTime); 
        attackRange = Mathf.MoveTowards(attackRange, 30f, 0.01f * Time.deltaTime); 
        shootingForce = Mathf.MoveTowards(shootingForce, 50f, 0.01f * Time.deltaTime); 
        maxHealth = Mathf.MoveTowards(maxHealth, 200f, 0.05f * Time.deltaTime); 
        regenRate = Mathf.MoveTowards(regenRate, 5f, 0.002f * Time.deltaTime);
        /*
        attackDamage = Mathf.Clamp(10f + 0.02f * Time.deltaTime, 10f, 50f);
        attackCooldown = Mathf.Clamp(0.5f - 0.0001f * Time.deltaTime, 0.2f, 0.5f);
        attackRange = Mathf.Clamp(10f + 0.01f * Time.deltaTime, 10f, 30f);
        shootingForce = Mathf.Clamp(50f + 0.01f * Time.deltaTime, 40f, 60f);
        maxHealth = Mathf.Clamp(100 + 0.05f * Time.deltaTime, 100f, 200f);
        regenRate = Mathf.Clamp(0f + 0.002f * Time.deltaTime, 0f, 5f);
        */
        if (enemyStats.activeInHierarchy)
        GameObject.FindGameObjectWithTag("EnemyStat").GetComponent<TextMeshProUGUI>().text =
            attackDamage.ToString("F" + 0) + "\n" +
            attackCooldown.ToString("F" + 2) + "\n" +
            attackRange.ToString("F" + 0) + "\n" +
            shootingForce.ToString("F" + 0) + "\n" +
            maxHealth.ToString("F" + 0) + "\n" +
            regenRate.ToString("F" + 2);
    }

    public void SaveScores()
    {
        ScoreData scoreData = new ScoreData { 
            timescoreRP = timescoreRP, namescoreRP = namescoreRP, 
            timescoreAP = timescoreAP, namescoreAP = namescoreAP, 
            timescoreFP = timescoreFP, namescoreFP = namescoreFP
        };
        string jsonData = JsonUtility.ToJson(scoreData);
        File.WriteAllText(Application.persistentDataPath + "/scores.json", jsonData);        
        PrintScores();
    }

    public void LoadScores()
    {
        string jsonData = File.ReadAllText(Application.persistentDataPath + "/scores.json");
        ScoreData scoreData = JsonUtility.FromJson<ScoreData>(jsonData);
        timescoreRP = scoreData.timescoreRP;
        timescoreAP = scoreData.timescoreAP;
        timescoreFP = scoreData.timescoreFP;

        namescoreRP = scoreData.namescoreRP;
        namescoreAP = scoreData.namescoreAP;
        namescoreFP = scoreData.namescoreFP;
    }

    public void SortScores()
    {
        LoadScores();
        timescoreRP[10] = (int)Resources.Load<GameSetting>("GameSetting").storeElapsedTime;
        namescoreRP[10] = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;
        timescoreAP[10] = (int)Resources.Load<GameSetting>("GameSetting").storeElapsedTime;
        namescoreAP[10] = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;
        timescoreFP[10] = (int)Resources.Load<GameSetting>("GameSetting").storeElapsedTime;
        namescoreFP[10] = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {//Run From Player
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (timescoreRP[i] > timescoreRP[j])
                    {
                        // Swap timescores
                        int tempTime = timescoreRP[i];
                        timescoreRP[i] = timescoreRP[j];
                        timescoreRP[j] = tempTime;

                        // Swap namescores
                        string tempName = namescoreRP[i];
                        namescoreRP[i] = namescoreRP[j];
                        namescoreRP[j] = tempName;
                    }
                }
            }
        }

        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {//Attack Player
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (timescoreAP[i] > timescoreAP[j])
                    {
                        // Swap timescores
                        int tempTime = timescoreAP[i];
                        timescoreAP[i] = timescoreAP[j];
                        timescoreAP[j] = tempTime;

                        // Swap namescores
                        string tempName = namescoreAP[i];
                        namescoreAP[i] = namescoreAP[j];
                        namescoreAP[j] = tempName;
                    }
                }
            }
        }

        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {//Full Run
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (timescoreFP[i] > timescoreFP[j])
                    {
                        // Swap timescores
                        int tempTime = timescoreFP[i];
                        timescoreFP[i] = timescoreFP[j];
                        timescoreFP[j] = tempTime;

                        // Swap namescores
                        string tempName = namescoreFP[i];
                        namescoreFP[i] = namescoreFP[j];
                        namescoreFP[j] = tempName;
                    }
                }
            }
        }
        SaveScores();
    }

    public void PrintScores()
    {
        LoadScores();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {//Run From Player
            for (int i = 0; i < 10; i++)
            {
                int hours = timescoreRP[i] / 3600;
                int minutes = (timescoreRP[i] % 3600) / 60;
                int seconds = timescoreRP[i] % 60;

                string limitedString = namescoreRP[i].Length <= 3 ? namescoreRP[i] : namescoreRP[i].Substring(0, 3);

                printtime += string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + "\n";
                printname += limitedString + "\n";
            }
        }

        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {//Attack Player
            for (int i = 0; i < 10; i++)
            {
                int hours = timescoreAP[i] / 3600;
                int minutes = (timescoreAP[i] % 3600) / 60;
                int seconds = timescoreAP[i] % 60;

                string limitedString = namescoreAP[i].Length <= 3 ? namescoreAP[i] : namescoreAP[i].Substring(0, 3);

                printtime += string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + "\n";
                printname += limitedString + "\n";
            }
        }

        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {//Full Run
            for (int i = 0; i < 10; i++)
            {
                int hours = timescoreFP[i] / 3600;
                int minutes = (timescoreFP[i] % 3600) / 60;
                int seconds = timescoreFP[i] % 60;

                string limitedString = namescoreFP[i].Length <= 3 ? namescoreFP[i] : namescoreFP[i].Substring(0, 3);

                printtime += string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + "\n";
                printname += limitedString + "\n";
            }
        }

        GameObject.FindGameObjectWithTag("ScoreTime").GetComponent<Text>().text = printtime;
        GameObject.FindGameObjectWithTag("ScoreName").GetComponent<Text>().text = printname;        
        printtime = "";
        printname = "";
    }

    #region Update Scoreboard Old
    /*
    public void UpdateScoreboard()
    {
        //if (scoreboardShown) 
        SortScoreboard();

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {//Run From Player
            for (int i = 0; i < 10; i++)
            {
                int hours = Resources.Load<GameSetting>("GameSetting").timescoreRP[i] / 3600;
                int minutes = (Resources.Load<GameSetting>("GameSetting").timescoreRP[i] % 3600) / 60;
                int seconds = Resources.Load<GameSetting>("GameSetting").timescoreRP[i] % 60;

                string limitedString = Resources.Load<GameSetting>("GameSetting").namescoreRP[i].Length <= 3 ? Resources.Load<GameSetting>("GameSetting").namescoreRP[i] : Resources.Load<GameSetting>("GameSetting").namescoreRP[i].Substring(0, 3);

                printtime += string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + "\n";
                printname += limitedString + "\n";
            }
        }

        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {//Attack Player
            for (int i = 0; i < 10; i++)
            {
                int hours = Resources.Load<GameSetting>("GameSetting").timescoreAP[i] / 3600;
                int minutes = (Resources.Load<GameSetting>("GameSetting").timescoreAP[i] % 3600) / 60;
                int seconds = Resources.Load<GameSetting>("GameSetting").timescoreAP[i] % 60;

                string limitedString1 = Resources.Load<GameSetting>("GameSetting").namescoreAP[i].Length <= 3 ? Resources.Load<GameSetting>("GameSetting").namescoreAP[i] : Resources.Load<GameSetting>("GameSetting").namescoreAP[i].Substring(0, 3);

                printtime += string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + "\n";
                printname += limitedString1 + "\n";
            }
        }

        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {//Full Run
            for (int i = 0; i < 10; i++)
            {
                int hours = Resources.Load<GameSetting>("GameSetting").timescoreFP[i] / 3600;
                int minutes = (Resources.Load<GameSetting>("GameSetting").timescoreFP[i] % 3600) / 60;
                int seconds = Resources.Load<GameSetting>("GameSetting").timescoreFP[i] % 60;

                string limitedString2 = Resources.Load<GameSetting>("GameSetting").namescoreFP[i].Length <= 3 ? Resources.Load<GameSetting>("GameSetting").namescoreFP[i] : Resources.Load<GameSetting>("GameSetting").namescoreFP[i].Substring(0, 3);

                printtime += string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + "\n";
                printname += limitedString2 + "\n";
            }
        }
        GameObject.FindGameObjectWithTag("ScoreTime").GetComponent<Text>().text = printtime;
        GameObject.FindGameObjectWithTag("ScoreName").GetComponent<Text>().text = printname;
        GameObject.FindGameObjectWithTag("YourTime").GetComponent<Text>().text = GameObject.FindGameObjectWithTag("TimeElapsed").GetComponent<TextMeshProUGUI>().text;
        printtime = "";
        printname = "";
        scoreboardShown = true;
    }
    */
    #endregion

    #region Sort scoreboard Old
    /*
    public void SortScoreboard()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {//Run From Player
            Resources.Load<GameSetting>("GameSetting").timescoreRP[10] = (int)elapsedTime;
            Resources.Load<GameSetting>("GameSetting").namescoreRP[10] = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (Resources.Load<GameSetting>("GameSetting").timescoreRP[i] > Resources.Load<GameSetting>("GameSetting").timescoreRP[j])
                    {
                        int temptime = Resources.Load<GameSetting>("GameSetting").timescoreRP[i];
                        Resources.Load<GameSetting>("GameSetting").timescoreRP[i] = Resources.Load<GameSetting>("GameSetting").timescoreRP[j];
                        Resources.Load<GameSetting>("GameSetting").timescoreRP[j] = temptime;

                        string tempname = Resources.Load<GameSetting>("GameSetting").namescoreRP[i];
                        Resources.Load<GameSetting>("GameSetting").namescoreRP[i] = Resources.Load<GameSetting>("GameSetting").namescoreRP[j];
                        Resources.Load<GameSetting>("GameSetting").namescoreRP[j] = tempname;
                    }
                }
            }
        }

        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {//Attack Player
            Resources.Load<GameSetting>("GameSetting").timescoreAP[10] = (int)elapsedTime;
            Resources.Load<GameSetting>("GameSetting").namescoreAP[10] = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (Resources.Load<GameSetting>("GameSetting").timescoreAP[i] > Resources.Load<GameSetting>("GameSetting").timescoreAP[j])
                    {
                        int temptime = Resources.Load<GameSetting>("GameSetting").timescoreAP[i];
                        Resources.Load<GameSetting>("GameSetting").timescoreAP[i] = Resources.Load<GameSetting>("GameSetting").timescoreAP[j];
                        Resources.Load<GameSetting>("GameSetting").timescoreAP[j] = temptime;

                        string tempname = Resources.Load<GameSetting>("GameSetting").namescoreAP[i];
                        Resources.Load<GameSetting>("GameSetting").namescoreAP[i] = Resources.Load<GameSetting>("GameSetting").namescoreAP[j];
                        Resources.Load<GameSetting>("GameSetting").namescoreAP[j] = tempname;
                    }
                }
            }

        }

        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {//Full Run
            Resources.Load<GameSetting>("GameSetting").timescoreFP[10] = (int)elapsedTime;
            Resources.Load<GameSetting>("GameSetting").namescoreFP[10] = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (Resources.Load<GameSetting>("GameSetting").timescoreFP[i] > Resources.Load<GameSetting>("GameSetting").timescoreFP[j])
                    {
                        int temptime = Resources.Load<GameSetting>("GameSetting").timescoreRP[i];
                        Resources.Load<GameSetting>("GameSetting").timescoreFP[i] = Resources.Load<GameSetting>("GameSetting").timescoreFP[j];
                        Resources.Load<GameSetting>("GameSetting").timescoreFP[j] = temptime;

                        string tempname = Resources.Load<GameSetting>("GameSetting").namescoreFP[i];
                        Resources.Load<GameSetting>("GameSetting").namescoreFP[i] = Resources.Load<GameSetting>("GameSetting").namescoreFP[j];
                        Resources.Load<GameSetting>("GameSetting").namescoreFP[j] = tempname;
                    }
                }
            }
        }
    }
    */
    #endregion

    #region Not viable
    /*
    public void AddRecord()
    {//Collect data
        Score input = new Score();
        if (SceneManager.GetActiveScene().buildIndex == 2)
            input.scoreNameRP = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;
        else if (SceneManager.GetActiveScene().buildIndex == 3)
            input.scoreNameAP = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;
        else if (SceneManager.GetActiveScene().buildIndex == 4)
            input.scoreNameFP = GameObject.FindGameObjectWithTag("InputName").GetComponent<TextMeshProUGUI>().text;

        input.scoreTimeRP = (int)elapsedTime;
        input.scoreTimeAP = (int)elapsedTime;
        input.scoreTimeFP = (int)elapsedTime;

        ScoreList scoreTempList = new ScoreList();
        scoreTempList.score = scoreList.score;
        scoreList.score = new Score[10];
        for (int i = 0; i < 11; i++)
        {
            scoreList.score[i] = scoreTempList.score[i];
        }
        scoreList.score[10] = input;

        OutputJson();
    }

    public void OutputJson()
    {
        string str = JsonUtility.ToJson(scoreList);
        File.WriteAllText(Application.dataPath + "/Data/Scoreboard.json", str);
    }
    */
    #endregion

}
