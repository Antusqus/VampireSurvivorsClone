using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

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

    public static GameState currentState;
    public static GameState previousState;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;


    [Header("StatDisplay")]
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentStaminaDisplay;
    public TextMeshProUGUI currentManaDisplay;

    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;
    public TextMeshProUGUI currentMaxSummonsDisplay;


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


    [Header("SpellsList")]
    public List<Image> chosenSpellsUI = new(6);
    public bool isGameOver = false;

    public bool choosingUpgrade;

    //Ref to players game object.
    public InputSystemUIInputModule UIModule;
    public GameObject playerObject;

    private DefaultPlayerActions playerActions;
    private InputAction pauseAction;

    void Awake()
    {
        //Singleton warning.
        if (instance == null)
        {
            instance = this;
            ChangeState(GameState.Gameplay);
        }
        else
        {
            Debug.LogWarning("Extra" + this + " DELETED");
            Destroy(gameObject);
        }
        DisableScreens();

    }


    private void OnEnable()
    {
        playerActions = new DefaultPlayerActions();

        pauseAction = playerActions.UI.Pause;
        pauseAction.performed += CheckForPauseAndResume;
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        isGameOver = false;
        pauseAction.performed -= CheckForPauseAndResume;
        pauseAction.Disable();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                UpdateStopwatch();
                break;

            case GameState.Paused:
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
    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI temp = textObj.AddComponent<TextMeshProUGUI>();
        temp.text = text;
        temp.horizontalAlignment = HorizontalAlignmentOptions.Center;
        temp.verticalAlignment = VerticalAlignmentOptions.Middle;
        temp.fontSize = textFontSize;
        if (textFont) temp.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        Destroy(textObj, duration);


        WaitForEndOfFrame w = new WaitForEndOfFrame();

        float t = 0;
        float yOffset = 0;
        while (t < duration)
        {

            if (!rect) break;

            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 1 - t / duration);
            yOffset += speed * Time.deltaTime;
            if (target)
            {
                rect.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0, yOffset));

            }
            else
            {
                rect.position += new Vector3(0, speed * Time.deltaTime, 0);
            }
            yield return w;
            t += Time.deltaTime;

        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        // If the canvas is not set, end the fucntion so we don't generate any floating text
        if (!instance.damageTextCanvas) return;
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;
        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
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

    void CheckForPauseAndResume(InputAction.CallbackContext context)
    {
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

    public void AssignChosenCharUI(CharacterData chosenCharacterData)
    {
        chosenCharImg.sprite = chosenCharacterData.Icon;
        chosenCharName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassivesUI(List<PlayerInventory.Slot> chosenWeaponsData, List<PlayerInventory.Slot> chosenPassivesData)
    {
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassivesData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("Weapons and passives data lists have diff lengths.");
            return;
        }

        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].image.sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].image.sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;

            }
        }

        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenPassivesData[i].image.sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassivesData[i].image.sprite;
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
            playerObject.SendMessage("Kill");
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
