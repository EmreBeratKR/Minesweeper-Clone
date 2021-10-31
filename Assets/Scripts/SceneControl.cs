using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneControl : MonoBehaviour
{

    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] GameObject WinPanel;
    [SerializeField] TextMeshProUGUI gameOverTime;
    [SerializeField] TextMeshProUGUI winTime;
    [SerializeField] TextMeshProUGUI timeCounter;
    [SerializeField] Toggle SoundSwitch;
    [SerializeField] GameObject EventBlocker;
    DataStorage dataStorage;
    [SerializeField] GameController gameController;
    [SerializeField] AudioControl audioControl;
    Coroutine flagRevCoroutine;
    bool isPaused = false;
    bool isInstant = false;
    float startTime;

    void Start()
    {
        dataStorage = GameObject.FindWithTag("DataStorage").GetComponent<DataStorage>();
        if (SceneManager.GetActiveScene().buildIndex == 1){SoundSwitch.isOn = dataStorage.soundON;}
        Time.timeScale = 1;
        startTime = Time.time;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && gameController.canPause)
            {
                if (!isPaused){if (!gameController.isGameOver && !gameController.isWin){pauseGame();}}
                else{resumeGame();}
            }
            updateTime();
        }
    }
    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void pauseGame()
    {
        PausePanel.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void resumeGame()
    {
        PausePanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }
    public void enterMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void enterGameOver()
    {
        gameOverTime.text = "Time\n" + timeCounter.text;
        gameController.isGameOver = true;
        GameOverPanel.SetActive(true);
        if (dataStorage.soundON){audioControl.GameOverMusic.Play();}
        EventBlocker.SetActive(false);
    }

    public void enterWin()
    {
        Time.timeScale = 0;
        winTime.text = "Time\n" + timeCounter.text;
        StartCoroutine(WinCO());
    }

    IEnumerator WinCO()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        WinPanel.SetActive(true);
        if (dataStorage.soundON){audioControl.WinMusic.Play();}
    }

    public void updateSound()
    {
        if (!SoundSwitch.isOn)
        {
            audioControl.GameOverMusic.Stop();
            audioControl.WinMusic.Stop();
        }
        else
        {
            if (gameController.isWin)
            {
                audioControl.WinMusic.Play();
            }
            else if (gameController.isGameOver)
            {
                audioControl.GameOverMusic.Play();
            }
        }
        dataStorage.soundON = SoundSwitch.isOn;
    }

    void updateTime()
    {
        int elapsedSec = System.Convert.ToInt32((Time.time - startTime).ToString().Split(char.Parse("."))[0]); //float (,) kullanırsa sıkıntı çıkar!
        int minute = (elapsedSec - (elapsedSec % 60)) / 60;

        string strMin = minute.ToString();
        if (strMin.Length == 1)
        {
            strMin = "0" + strMin;
        }

        string strSec = (elapsedSec - 60 * minute).ToString();
        if (strSec.Length == 1)
        {
            strSec = "0" + strSec;
        }
        timeCounter.text = strMin + ":" + strSec;
    }

    public void enableEventBlocker()
    {
        EventBlocker.SetActive(true);
    }

    public void revealMines()
    {
        flagRevCoroutine = StartCoroutine(revealMinesCO());
    }

    IEnumerator revealMinesCO()
    {
        foreach (var id in gameController.minesPos)
        {
            if (!gameController.tiles[id].GetComponent<TileControl>().isOpen)
            {
                yield return new WaitForSecondsRealtime(1f);
                gameController.backTiles[id].SetActive(true);
                if (dataStorage.soundON){audioControl.MineExplosionSound.Play();}
            }
        }
        yield return new WaitForSecondsRealtime(0.5f);
        enterGameOver();
    }

    public void InstantMineReveal()
    {
        if (!isInstant)
        {
            EventBlocker.transform.GetChild(0).gameObject.SetActive(false);
            isInstant = true;
            StopCoroutine(flagRevCoroutine);
            StartCoroutine(InstantMineRevealCO());
        }
    }

    IEnumerator InstantMineRevealCO()
    {
        foreach (var id in gameController.minesPos)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            gameController.backTiles[id].SetActive(true);
        }
        if (dataStorage.soundON){audioControl.MineExplosionSound.Play();}
        yield return new WaitForSecondsRealtime(0.5f);
        enterGameOver();
    }
}
