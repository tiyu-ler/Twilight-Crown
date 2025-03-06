using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public CanvasGroup MainMenuGroup, SettingsMenuGroup, SavesMenuGroup;
    private Transform MainMenuTransform, SettingsMenuTransform, SavesMenuTransform;
    public float fadeDuration = 0.05f;
    public List<TextMeshProUGUI> LocationInfo = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> MoneyInfo = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> TimeInfo = new List<TextMeshProUGUI>();

    public List<GameObject> StartNewGameButton = new List<GameObject>();
    public GameManager gameManager;
    private List<bool> SaveExists = new List<bool> { false, false, false };
    private void ProfilesDataSet()
    {
        for (int i = 0; i<= 2; i++)
        {
            List<float> profileData = gameManager.GetSaveProfilesData(i);
            if (profileData != null)
            {
                StartNewGameButton[i+3].SetActive(true);
                LocationInfo[i].text = GetLocationname(profileData[0]);
                LocationInfo[i+3].text = GetLocationname(profileData[0]);
                MoneyInfo[i].text = profileData[1].ToString("F0");
                MoneyInfo[i+3].text = profileData[1].ToString("F0");
                TimeInfo[i].text = ReturnTime(profileData[2]);
                TimeInfo[i+3].text = ReturnTime(profileData[2]);
                SaveExists[i] = true;
            }
            else
            {
                SaveExists[i] =  false;
                LocationInfo[i].gameObject.SetActive(false);
                MoneyInfo[i].gameObject.SetActive(false);
                TimeInfo[i].gameObject.SetActive(false);
                StartNewGameButton[i].SetActive(true);
                StartNewGameButton[i+3].SetActive(false);
            }
        }
    }

    private string GetLocationname(float obeliskID)
    {
        string currentLocation = "Error";
        switch(obeliskID)
        {
            case 0: currentLocation = "Spawn Location"; break;
            case 1: currentLocation = "First Location"; break;
            case 2: currentLocation = "Second Location"; break;
            case 3: currentLocation = "Third Location"; break;
        }
        return currentLocation;
    }
    public string ReturnTime(float playTime)
    {
        int hours = Mathf.FloorToInt(playTime / 3600);
        int minutes = Mathf.FloorToInt(playTime % 3600 / 60);

        return string.Format("{0}h {1}m", hours, minutes);
    }

    public void StartGame(int SaveID)
    {
        gameManager.LoadGame(SaveID);
        SceneManager.LoadScene("GameScene");
    }

    public void DeleteSave(int SaveID)
    {
        if (SaveExists[SaveID])
        {
            SaveSystem.DeleteSave(SaveID);
            SaveExists[SaveID] = false;
            ProfilesDataSet();
        }
    }

    private void Start()
    {
        // gameManager.SaveGame(1); // Save Game Test
        MainMenuTransform = MainMenuGroup.transform;
        SettingsMenuTransform = SettingsMenuGroup.transform;
        SavesMenuTransform = SavesMenuGroup.transform;
        SetCanvas(MainMenuGroup, MainMenuTransform, 1, true);
        SetCanvas(SettingsMenuGroup, SettingsMenuTransform, 0, false);
        SetCanvas(SavesMenuGroup, SavesMenuTransform, 0, false);
        ProfilesDataSet();
    }
    public void MainMenuActive()
    {
        StopAllCoroutines();
        StartCoroutine(SwitchCanvas(MainMenuGroup, MainMenuTransform, SettingsMenuGroup, SettingsMenuTransform, SavesMenuGroup, SavesMenuTransform));
    }

    public void SettingsMenuActive()
    {
        StopAllCoroutines();
        StartCoroutine(SwitchCanvas(SettingsMenuGroup, SettingsMenuTransform, MainMenuGroup, MainMenuTransform, SavesMenuGroup, SavesMenuTransform));
    }

    public void SaveMenuActive()
    {
        StopAllCoroutines();
        StartCoroutine(SwitchCanvas(SavesMenuGroup, SavesMenuTransform, MainMenuGroup, MainMenuTransform, SettingsMenuGroup, SettingsMenuTransform));
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    private IEnumerator SwitchCanvas(CanvasGroup toEnable, Transform toEnableTransform, CanvasGroup toDisable1, Transform toDisable1Transform, CanvasGroup toDisable2, Transform toDisable2Transform)
    {
        yield return StartCoroutine(FadeAndScaleCanvas(toDisable1, toDisable1Transform, 0, false));
        yield return StartCoroutine(FadeAndScaleCanvas(toDisable2, toDisable2Transform, 0, false));

        yield return StartCoroutine(FadeAndScaleCanvas(toEnable, toEnableTransform, 1, true));
    }

    private IEnumerator FadeAndScaleCanvas(CanvasGroup canvas, Transform canvasTransform, float targetAlpha, bool enableAfterFade)
    {
        float startAlpha = canvas.alpha;
        Vector3 startScale = canvasTransform.localScale;
        Vector3 targetScale = (targetAlpha == 1) ? Vector3.one : new Vector3(0.85f, 0.85f, 1f);
        float time = 0;

        if (enableAfterFade) canvas.gameObject.SetActive(true);

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            canvasTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        canvas.alpha = targetAlpha;
        canvasTransform.localScale = targetScale;
        canvas.interactable = (targetAlpha == 1);
        canvas.blocksRaycasts = (targetAlpha == 1);

        if (targetAlpha == 0) canvas.gameObject.SetActive(false);
    }

    private void SetCanvas(CanvasGroup canvas, Transform canvasTransform, float alpha, bool isActive)
    {
        canvas.alpha = alpha;
        canvas.interactable = isActive;
        canvas.blocksRaycasts = isActive;
        canvas.gameObject.SetActive(isActive);
        canvasTransform.localScale = isActive ? Vector3.one : new Vector3(0.85f, 0.85f, 1f);
    }
}
