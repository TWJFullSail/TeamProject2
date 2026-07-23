using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;
    public GameObject checkpointPopup;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public Image playerHPBar;
    public Image playerStaminaBar;

    public TMP_Text ammoText;
    public TMP_Text totalAmmoText;
    public TMP_Text playerHPText;
    public TMP_Text playerStaminaText;
    public GameObject playerDamageScreen;
    public GameObject playerSpawnPos;

    float timeScaleOrig;

    public int gameGoalCount;

    bool isFinalWave = true;												// keeps the current single wave working

    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        updateGameGoalText();
    }
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void startWave(int amount, bool finalWave)
    {
        gameGoalCount = Mathf.Max(0, amount);
        isFinalWave = finalWave;

        updateGameGoalText();                                           // displays the new enemy total
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCount = Mathf.Max(0, gameGoalCount);

        updateGameGoalText();

        if (gameGoalCount <= 0 && isFinalWave)
        {
            youWin();
        }
    }

    public bool isGameGoalComplete()
    {
        return gameGoalCount <= 0;
    }

    void updateGameGoalText()
    {
        if (gameGoalCountText != null)
        {
            gameGoalCountText.text =
                gameGoalCount.ToString("F0");
        }
    }

    public void youWin()
    {
        statePause();

        menuActive = menuWin;

        if (menuActive != null)
        {
            menuActive.SetActive(true);
        }
    }
    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}