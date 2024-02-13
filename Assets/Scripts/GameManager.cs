using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    public GameState previousState;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;


    [Header("StatDisplay")]
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    [Header("Result Screen")]
    public Image chosenCharImg;
    public TextMeshProUGUI chosenCharName;
    public TextMeshProUGUI levelReachedDisplay;
    public TextMeshProUGUI timeSurvivedDisplay;

    [Header("Stopwatch")]
    public float timeLimit; // Time limit in seconds
    float stopwatchTime;
    public TextMeshProUGUI stopwatchDisplay;

    public List<Image> chosenWeaponsUI = new(6);
    public List<Image> chosenPassiveItemsUI = new(6);

    public bool isGameOver = false;

    public bool choosingUpgrade;

    //Ref to players game object.
    public GameObject playerObject;

    void Awake()
    {
        //Singleton warning.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Extra" + this + " DELETED");
            Destroy(gameObject);
        }
        DisableScreens();
    }
    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game is over.");
                    DisplayResults();
                }
                break;

            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    Debug.Log("Upgrades shown");
                    levelUpScreen.SetActive(true);
                    
                }
                break;

            default:
                Debug.LogWarning("INVALID GAMESTATE");
                break;

        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }
    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            Debug.Log("Game is paused");
        }


    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);

            Debug.Log("Game is resumed");
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }
    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;
        ChangeState(GameState.GameOver);

    }

    public void DisplayResults()
    {
        resultScreen.SetActive(true);
    }

    public void AssignChosenCharUI(CharacterScriptableObject chosenCharacterData)
    {
        chosenCharImg.sprite = chosenCharacterData.Icon;
        chosenCharName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassivesUI(List<Image> chosenWeaponsData, List<Image> chosenPassivesData)
    {
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassivesData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("Weapons and passives data lists have diff lengths.");
            return;
        }

        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;

            }
        }

        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenPassivesData[i].sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassivesData[i].sprite;
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false;

            }
        }

    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            GameOver();
        }
    }

    void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }
}
