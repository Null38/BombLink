using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtenManager : MonoBehaviour
{
    [SerializeField] GameObject pcUI;
    [SerializeField] GameObject mobileUI;

    [SerializeField] List<GameObject> pauseUi;
    [SerializeField] GameObject pauseSetUi;
    [SerializeField] GameObject optionsUI;
    
    [SerializeField] GameObject mobilePauseSetUi;
    [SerializeField] GameObject mobileOptionsUI;

    [SerializeField] GameObject mobilePlayButtenA;
    [SerializeField] GameObject mobilePlayButtenB;

    [SerializeField] GameObject endSheet;

    bool mobilePlayButtenModA = true;

    public AudioSource buttenSound;

    bool gamePause;

    int difficulty;
    int mode;

    private void Awake()
    {
        difficulty = PlayerPrefs.GetInt("diff");
        mode = PlayerPrefs.GetInt("mode");

#if UNITY_STANDALONE_WIN
        pcUI.SetActive(true);
        foreach (GameObject ui in pauseUi)
        {
            ui.SetActive(false);
        }
        pauseSetUi.SetActive(false);
        optionsUI.SetActive(false);
        mobileUI.SetActive(false);
#endif

#if UNITY_ANDROID
        mobileUI.SetActive(true);
        mobilePauseSetUi.SetActive(false);
        mobileOptionsUI.SetActive(false);
        pcUI.SetActive(false);
        #endif

        if (!PlayerPrefs.HasKey("controlMod"))
        {
            PlayerPrefs.SetInt("controlMod", 1);
        }

        mobilePlayButtenModA = Convert.ToBoolean(PlayerPrefs.GetInt("controlMod"));

        mobilePlayButtenA.SetActive(mobilePlayButtenModA);
        mobilePlayButtenB.SetActive(!mobilePlayButtenModA);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pause(bool pause)
    {
        gamePause = pause;
        Time.timeScale = Convert.ToInt32(!pause);
        foreach (GameObject ui in pauseUi)
        {
            ui.SetActive(pause);
        }

        #if UNITY_STANDALONE_WIN
        pauseSetUi.SetActive(pause);
        #endif

        #if UNITY_ANDROID
        mobilePauseSetUi.SetActive(pause);
        #endif
    }

    public void sceneLoad()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main");
    }

    public void OptionsUiOnOff(bool isOn)
    {
        optionsUI.SetActive(isOn);
    }

    public void MobilePlayButtenChange()
    {
        mobilePlayButtenModA = !mobilePlayButtenModA;
        mobilePlayButtenA.SetActive(mobilePlayButtenModA);
        mobilePlayButtenB.SetActive(!mobilePlayButtenModA);
        PlayerPrefs.SetInt("controlMod", Convert.ToInt32(mobilePlayButtenModA));
    }

    public void MobileOptionsUiOnOff(bool isOn)
    {
        mobileOptionsUI.SetActive(isOn);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            foreach (GameObject ui in pauseUi)
            {
                ui.SetActive(pause);
            }
        }
        else
        {
            foreach (GameObject ui in pauseUi)
            {
                if(!gamePause)
                ui.SetActive(pause);
            }
        }
    }

    public void GameLoad()
    {
        PlayerPrefs.SetInt("diff", difficulty);
        PlayerPrefs.SetInt("mode", mode);
        SceneManager.LoadScene("Game");
    }

    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
}
