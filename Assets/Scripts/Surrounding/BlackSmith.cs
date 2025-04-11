using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BlacksmithInteraction : MonoBehaviour
{
    public MarkerTextPopUp markerTextPopUp;
    public GameObject dialogUI;
    public TextMeshProUGUI dialogText;
    public GameObject upgradeUI;
    public TextMeshProUGUI costText;

    private bool inTrigger = false;
    private bool dialogActive = false;
    private bool isTyping = false;
    private Coroutine dialogCoroutine;
    private GameManager gameManager;
    private int _currentLevel = 0;
    private string fullText;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private string[] dialogNoMoneyAtAll = new string[]
    { 
        "Sorry, you do not have enough."
    };
    private string[] dialogNoMoney = new string[] 
    {
        "Ok, few coins aint much, i will help you"
    };
    private string[] dialogEnd = new string[]
    {   
        "Be carefull, good luck",
        "See you soon.",
    };
    private string[] dialogFirstEncounter = new string[]
    {
        "Ah, a new blade in need of forging.",
        "I can temper this metal and bring out its true potential.",
        "It won't be cheap, but you'll feel the difference.",
    };

    private string[] dialogAfterFirstUpgrade = new string[]
    {
        "That swing's got some real bite now, eh?",
        "There's still more I can do, if you can pay the price.",
        "Let me know when you're ready.",
    };

    private string[] dialogAfterFinalUpgrade = new string[]
    {
        "This blade... it's as good as it gets.",
        "I’ve poured all my skill into it.",
        "Go now—and use it well.",
    };

    private int[] upgradeCosts = new int[] { 50, 90 };

    private void Start()
    {
        dialogUI.SetActive(false);
        upgradeUI.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        if (inTrigger && !dialogActive && Input.GetKeyDown(KeyCode.W))
        {
            int level = PlayerDataSave.Instance.SwordLevel;

            if (level == 0)
            {
                StartCoroutine(StartDialog(dialogFirstEncounter));
            }
            else if (level == 1)
            {
                StartCoroutine(StartDialog(dialogAfterFirstUpgrade));
            }
            else
            {
                StartCoroutine(StartDialog(dialogAfterFinalUpgrade, endAfterDialog: true));
            }
        }
        else if (dialogActive && Input.anyKeyDown && isTyping)
        {
            if (dialogCoroutine != null) StopCoroutine(dialogCoroutine);
            dialogText.text = fullText;
            isTyping = false;
        }
    }

    private IEnumerator StartDialog(string[] lines, bool endAfterDialog = false, bool recursionCall = false)
    {
        playerAttack.enabled = false;
        playerMovement.CanMove = false;
        playerMovement.HorizontalInput = 0;

        dialogActive = true;
        markerTextPopUp.DisableMarkUp();

        upgradeUI.SetActive(false);
        dialogUI.SetActive(true);
        dialogText.text = "";

        foreach (string line in lines)
        {
            fullText = line;
            isTyping = true;
            dialogCoroutine = StartCoroutine(TypeText(line));
            yield return new WaitUntil(() => !isTyping);
            yield return new WaitForSeconds(0.85f);
        }
        if (!recursionCall)
        {
            if (endAfterDialog)
            {
                EndDialog();
            }
            else
            {
                ShowUpgradeOptions();
            }
        }
        else
        {
            StartCoroutine(StartDialog(dialogEnd, endAfterDialog: true));
        }
    }

    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.06f);
        }
        isTyping = false;
    }

    private void ShowUpgradeOptions()
    {
        DisablePlayer();
        _currentLevel = PlayerDataSave.Instance.SwordLevel-1;
        costText.text = $"Upgrade sword for {upgradeCosts[_currentLevel]}  ?";
        dialogUI.SetActive(false);
        upgradeUI.SetActive(true);
    }

    // 🎯 Assign this to the Yes Button in Inspector
    public void OnYesClicked()
    {
        int cost = upgradeCosts[_currentLevel];
        Debug.Log("Cost: " + cost);
        Debug.Log("Money: " + PlayerDataSave.Instance.Money);
        if (PlayerDataSave.Instance.Money >= cost-5)
        {
            if (PlayerDataSave.Instance.Money < cost)
            {
                StartCoroutine(StartDialog(dialogNoMoney, recursionCall: true));
                cost = PlayerDataSave.Instance.Money;
            }
            _currentLevel += 1;
            PlayerDataSave.Instance.Money -= cost;
            PlayerDataSave.Instance.SwordLevel++;

            HitboxAttackCheck[] hitboxes = FindObjectsOfType<HitboxAttackCheck>();
            foreach (var hitbox in hitboxes)
            {
                hitbox.UpdateDamage();
            }

            gameManager.SaveGame(PlayerDataSave.Instance.saveID);
        }
        else
        {
            StartCoroutine(StartDialog(dialogNoMoneyAtAll, recursionCall: true));
        }
    }

    // 🎯 Assign this to the No Button in Inspector
    public void OnNoClicked()
    {
        StartCoroutine(StartDialog(dialogEnd, endAfterDialog: true));
    }

    private void EndDialog()
    {
        playerMovement.CanMove = true;
        playerAttack.enabled = true;
        dialogActive = false;
        dialogUI.SetActive(false);
        upgradeUI.SetActive(false);
        markerTextPopUp.ForcedPopUp();
        EnablePlayer();
    }

    private void DisablePlayer()
    {
        // disable Player
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EnablePlayer()
    {
        // disable Player
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = false;
            // if (!dialogActive)
            // {
            //     markerTextPopUp.DisableMarkUp();
            // }
        }
    }
}
