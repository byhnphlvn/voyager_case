using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using AssetKits.ParticleImage;
//using ElephantSDK;
public class GameManager : MonoBehaviour
{


    public TextMeshProUGUI _levelText;
    public TextMeshProUGUI _coinText;


    [SerializeField] GameObject inGameCanvas;
    [SerializeField] GameObject winCanvas;
    [SerializeField] GameObject failCanvas;
    public ParticleImage _coinImage;
    public Button _nextButton;

    public TextMeshProUGUI _coinGiveAmountText;
    int oldRandomLevel;
    public Image _finalCanvasCoinImage;
    public ParticleImage _finalConfettiParticle;
    public bool finished;
    public GameObject _timeFail;
    public GameObject _spaceFail;
    public static GameManager instance;



    [SerializeField] TextMeshProUGUI _timerTexter;
    int lastSecond;
    [SerializeField] Color _timerTexterColorer;
    bool firstClick;

    public float timeInSeconds;
    bool tutorail;
    public List<GameObject> _timerCanvas;
    public void SetClock()
    {
        if (!tutorail)
        {
            timeInSeconds -= Time.deltaTime;
            if (timeInSeconds <= 0)
            {
                GameManager.instance.FailGame(true);
                string niceTime = string.Format("{0:0}:{1:00}", 0, 0);
            }
            else
            {
                int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
                int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
                if (lastSecond != seconds)
                {
                    if (timeInSeconds < 10)
                    {
                        _timerTexter.color = Color.red;
                        _timerTexter.DOColor(_timerTexterColorer, .1f).SetDelay(.1f);
                        _timerTexter.transform.DORotate(new Vector3(0, 0, 5), .04f);
                        _timerTexter.transform.DORotate(new Vector3(0, 0, -5), .04f).SetDelay(.04f);
                        _timerTexter.transform.DORotate(new Vector3(0, 0, -0), .02f).SetDelay(.08f);
                        _timerTexter.transform.DOScale(Vector3.one * 1.2f, .1f);
                        _timerTexter.transform.DOScale(Vector3.one, .1f).SetDelay(.1f);
                    }
                    else
                    {
                        _timerTexter.transform.DOScale(Vector3.one * 1.1f, .1f);
                        _timerTexter.transform.DOScale(Vector3.one, .1f).SetDelay(.1f);
                    }

                    lastSecond = seconds;
                }
                string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds + 1);
                _timerTexter.text = niceTime;
            }
        }
    }
    private void Awake()
    {
        instance = this;
        if (timeInSeconds < 60)
        {
            timeInSeconds = 180;
        }
        SetClock();
        Application.targetFrameRate = 60;
        if (PlayerPrefs.GetInt("Level") == 0)
        {
            PlayerPrefs.SetInt("Level", 1);
        }
        _levelText.text = "Level " +PlayerPrefs.GetInt("Level").ToString();
        if (PlayerPrefs.GetInt("LevelPref") == 0)
        {
            PlayerPrefs.SetInt("LevelPref", 1);
        }
        if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt("LevelPref"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("LevelPref"));
        }
        RefreshCoinText();
    }
    private void Start()
    {
        // Elephant.LevelStarted(PlayerPrefs.GetInt("Level"));
    }
    public void RefreshCoinText()
    {
        _coinText.text = PlayerPrefs.GetInt("Coin").ToString("0");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            WinGame();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            NextLevel();
        }

        if (!GameManager.instance.finished)
        {
            if (firstClick)
            {
                SetClock();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!firstClick)
            {
                firstClick = true;
            }
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public IEnumerator SummonCanvas()
    {
        yield return new WaitForSeconds(.4f);
        inGameCanvas.gameObject.SetActive(false);
        inGameCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(true);
        winCanvas.transform.localScale = Vector3.zero;
        winCanvas.transform.DOScale(Vector3.one * 1.2f, .13f);
        winCanvas.transform.DOScale(Vector3.one, .1f).SetDelay(.13f);
        //_coinText.DOColor(Color.white, .2f);
    }
    public IEnumerator SummonFailCanvas(float waitTime)
    {
        yield return new WaitForSeconds(.4f);
        inGameCanvas.gameObject.SetActive(false);
        inGameCanvas.gameObject.SetActive(false);
        failCanvas.gameObject.SetActive(true);
        failCanvas.transform.localScale = Vector3.zero;
        failCanvas.transform.DOScale(Vector3.one * 1.2f, .13f);
        failCanvas.transform.DOScale(Vector3.one, .1f).SetDelay(.13f);
        //_coinText.DOColor(Color.white, .2f);
    }
    public void WinGame()
    {
        if (!finished)
        {
            // _coinGiveAmountText.text = "+" + (WaitZone.instance._colorsNeeded.Count * 10).ToString();
            Taptic.Success();
            // Elephant.LevelCompleted(PlayerPrefs.GetInt("Level"));
            _finalConfettiParticle.Play();
            StartCoroutine(SummonCanvas());
            finished = true;
        }
    }
    public void FailGame(bool timeFail = false)
    {
        if (!finished)
        {
            if (timeFail)
            {
                //_failTextImager.sprite = _timeFailTexter;
                _spaceFail.gameObject.SetActive(false);
                _timeFail.gameObject.SetActive(true);
            }
            Taptic.Failure();
            //Elephant.LevelFailed(PlayerPrefs.GetInt("Level"));
            float waiterTimer = .4f;
            if (timeFail)
            {
                waiterTimer = .2f;
            }
            StartCoroutine(SummonFailCanvas(waiterTimer));
            finished = true;
        }
    }
    public void NextButton()
    {
        Taptic.Medium();
        _nextButton.enabled = false;
        _finalCanvasCoinImage.transform.DOScale(Vector3.zero, .2f);
        _coinImage.Play();
        int currentCoinAmount = PlayerPrefs.GetInt("Coin");
        // int coinTo = currentCoinAmount + (WaitZone.instance._colorsNeeded.Count * 10) ;
        int coinTo = 10;
        PlayerPrefs.SetInt("Coin", coinTo);
        DOTween.To(() => currentCoinAmount, x => currentCoinAmount = x, coinTo, 1f)
            .OnUpdate(() => {
                _coinText.text = currentCoinAmount.ToString("0");
            }).OnComplete(() => {
                NextLevel();
                RefreshCoinText();
            }).SetDelay(1.5f);
    }
    public void NextLevel()
    {
        if (PlayerPrefs.GetInt("LevelsFinished") == 0)
        {
            if (PlayerPrefs.GetInt("LevelPref") < SceneManager.sceneCountInBuildSettings - 1)
            {
                PlayerPrefs.SetInt("LevelPref", PlayerPrefs.GetInt("LevelPref") + 1);
            }
            else
            {
                PlayerPrefs.SetInt("LevelsFinished", 1);
                List<int> levelList = new List<int>();
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 1; i++)
                {
                    levelList.Add(i);
                }
                if (PlayerPrefs.GetInt("RandomizedLevels") == 0)
                {
                    PlayerPrefs.SetInt("RandomizedLevels", 1);
                }
                levelList.Remove(oldRandomLevel);
                int newLevel = levelList[Random.Range(5, levelList.Count)];
                if (newLevel == 0)
                {
                    newLevel = 1;
                }
                //oldRandomLevel = newLevel;
                PlayerPrefs.SetInt("OldRandomLevel", newLevel);
                PlayerPrefs.SetInt("LevelPref", newLevel);
            }
        }
        else
        {
            List<int> levelList = new List<int>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                levelList.Add(i);
            }
            if (PlayerPrefs.GetInt("RandomizedLevels") == 0)
            {
                PlayerPrefs.SetInt("RandomizedLevels", 1);
            }
            levelList.Remove(PlayerPrefs.GetInt("OldRandomLevel"));
            int newLevel = levelList[Random.Range(5, levelList.Count)];
            PlayerPrefs.SetInt("OldRandomLevel", newLevel);
            if (newLevel == 0)
            {
                newLevel = 1;
            }
            PlayerPrefs.SetInt("LevelPref", newLevel);
        }
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("LevelPref"));
    }
}
