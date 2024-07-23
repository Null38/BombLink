using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;

public class MainButtonManager : MonoBehaviour
{
    [SerializeField] GameObject pcUI;
    [SerializeField] GameObject mobileUI;

    [SerializeField] int pcSelectButten;
    [SerializeField] RectTransform[] pcChoseButten;
    [SerializeField] RectTransform[] pcMainButtens;
    [SerializeField] RectTransform[] pcDifficultyButten;
    [SerializeField] Image[] pcPointerImage;
    [SerializeField] RectTransform pcPointerTrans;

    [SerializeField] GameObject pcChoiceDifficulty;
    [SerializeField] GameObject pcOptionsUI;
    [SerializeField] GameObject pcPoint;
    [SerializeField] GameObject creditsUI;

    [SerializeField] GameObject mobileMainUI;
    [SerializeField] GameObject mobileChoiceDifficulty;
    [SerializeField] GameObject mobileOptionsUI;
    [SerializeField] GameObject mobileCreditsUI;
    [SerializeField] GameObject mobileQutiyUI;

    public AudioSource ButtenSound;

    int gameMode;
    
    float escapeTimeCheck;

    public Image escapeImage;

    private bool curserMove;

    int openUiNumber = 0;

    private void Awake()
    {
#if UNITY_STANDALONE_WIN
        pcChoiceDifficulty.SetActive(false);
        pcOptionsUI.SetActive(false);
        mobileUI.SetActive(false);
        creditsUI.SetActive(false);
        pcUI.SetActive(true);
#endif

#if UNITY_ANDROID
        pcUI.SetActive(false);
        mobileUI.SetActive(true);
        mobileChoiceDifficulty.SetActive(false);
        mobileOptionsUI.SetActive(false);
        mobileCreditsUI.SetActive(false);
        mobileMainUI.SetActive(true);
        mobileQutiyUI.SetActive(false);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        LogIn();

        StartCoroutine(CurserMove());
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_WIN
        if(Input.GetKey(KeyCode.Escape))
        {
            escapeTimeCheck += Time.deltaTime;
            escapeImage.fillAmount = Mathf.Lerp(0, 1 , escapeTimeCheck / 2f);
            if (escapeTimeCheck >= 2)
            {
                Debug.Log("GameQuit");
                Application.Quit();
            }
        }
        else if(escapeTimeCheck > 0)
        {
            escapeTimeCheck -= Time.deltaTime * 2;
            escapeImage.fillAmount = Mathf.Lerp(0, 1, escapeTimeCheck / 2f);
            if (escapeTimeCheck < 0)
            {
                escapeTimeCheck = 0;
            }
        }
#endif

#if UNITY_ANDROID
        if (Input.GetKey(KeyCode.Escape))
        {
            if (!mobileOptionsUI.activeSelf && !mobileCreditsUI.activeSelf && !mobileChoiceDifficulty.activeSelf)
            {
                mobileQutiyUI.SetActive(true);
            }
        }
#endif

    }

    public void GameQuit()
    {
        Application.Quit();
    }

    private void FixedUpdate()
    {
        #if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.UpArrow) && pcSelectButten > 0)
        {
            pcSelectButten--;
            ButtenSelet(pcSelectButten);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && pcSelectButten < 3)
        {
            pcSelectButten++;
            ButtenSelet(pcSelectButten);
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            ButtenSound.Play();
            switch (openUiNumber)
            {
                case 0:
                    switch (pcSelectButten)
                    {
                        case 0:
                            DifficultyChose(0);
                            break;
                        case 1:
                            DifficultyChose(1);
                            break;
                        case 2:
                            OptionsUiOnOff(true);
                            break;
                        case 3:
                            CreditsUiOnOff(true);
                            break;
                    }
                    break;
                case 1:
                    switch (pcSelectButten)
                    {
                        case 3:
                            DifficultyClose();
                            break;
                        default:
                            GameLoad(pcSelectButten);
                            break;
                    }
                    break;
                case 2:
                    break;
            }
            
        }
        #endif
    }

    public void ButtenSelet(int select)
    {
        pcSelectButten = select;
    }

    IEnumerator CurserMove()
    {
        while (true)
        {
            pcPointerTrans.position = Vector3.Lerp(pcPointerTrans.position, pcChoseButten[pcSelectButten].position, 0.25f);
            pcPointerTrans.sizeDelta = Vector3.Lerp(pcPointerTrans.sizeDelta, pcChoseButten[pcSelectButten].sizeDelta, 0.25f);
            
            yield return null;
        }
    }

    public void DifficultyChose(int mode)
    {
        if (pcChoiceDifficulty.activeSelf) return;
        pcChoseButten = pcDifficultyButten;
        pcSelectButten = 0;
        pcPoint.SetActive(true);
        gameMode = mode;
        ((RectTransform)pcPoint.transform).position = pcPointerTrans.position;
        ((RectTransform)pcPoint.transform).sizeDelta = pcPointerTrans.sizeDelta;
        pcChoiceDifficulty.SetActive(true);
        openUiNumber = 1;
    }

    public void MobileDifficultyChose(int mode)
    {
        if (mobileChoiceDifficulty.activeSelf) return;
        gameMode = mode;
        mobileChoiceDifficulty.SetActive(true);
        mobileMainUI.SetActive(false);
    }

    public void DifficultyClose()
    {
        pcSelectButten = gameMode;
        pcChoseButten = pcMainButtens;
        pcChoiceDifficulty.SetActive(false);
        pcPoint.SetActive(false);
        openUiNumber = 0;
    }

    public void MobileDifficultyClose()
    {
        mobileChoiceDifficulty.SetActive(false);
        mobileMainUI.SetActive(true);
    }

    public void OptionsUiOnOff(bool isOn)
    {
        pcOptionsUI.SetActive(isOn);
    }

    public void MobileOptionsUiOnOff(bool isOn)
    {
        mobileOptionsUI.SetActive(isOn);
        mobileMainUI.SetActive(!isOn);
    }

    public void CreditsUiOnOff(bool isOn)
    {
        creditsUI.SetActive(isOn);
    }

    public void MobileCreditsUiOnOff(bool isOn)
    {
        mobileCreditsUI.SetActive(isOn);
        mobileMainUI.SetActive(!isOn);
    }

    public void GameLoad(int diff)
    {
        PlayerPrefs.SetInt("diff", diff);
        PlayerPrefs.SetInt("mode", gameMode);
        SceneManager.LoadScene("Game");
    }

    public void LogIn()
    {
        Social.localUser.Authenticate((bool success) =>{});
    }
}
