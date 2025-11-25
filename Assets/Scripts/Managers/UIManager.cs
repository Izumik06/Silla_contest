using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI ballCountText;
    [SerializeField] GameObject PauseUI;
    [SerializeField] GameObject SettingUI;
    [SerializeField] Button volumeBtn;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle guideLineToggle;
    [SerializeField] Sprite volumeSprite;
    [SerializeField] Sprite mutedVolumeSprite;

    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject pauseBtn;

    [SerializeField] GameObject fadeOut;

    [SerializeField] GameObject gameOverUI;
    [SerializeField] TextMeshProUGUI gameOverHighScore;
    [SerializeField] TextMeshProUGUI gameOverScore;

    AudioSource audioSource;
    //Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        guideLineToggle.isOn = SettingManager.Instance.useGuideLine;
        volumeSlider.value = SettingManager.Instance.volume;
        SetVolumeSprite();
        SetScoreUI();
        SetBallCountUI();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void PlayBtnSound()
    {
        audioSource.Play();   
    }
    public void SetGameOverUI()
    {
        gameOverUI.SetActive(true);
        gameOverHighScore.text = highScoreText.text;
        gameOverScore.text = scoreText.text;
    }
    public void StartBtn()
    {
        startBtn.SetActive(false);
        pauseBtn.SetActive(true);
        GameManager.Instance.isStartGame = true;
        Invoke("isPauseDisable", 0.05f);
        GameManager.Instance.Next();
    }
    public void MainBtn()
    {
        fadeOut.SetActive(true);
    }
    public void VolumeSlider()
    {
        SettingManager.Instance.volume = volumeSlider.value;
    }
    public void VolumeBtn()
    {
        SettingManager.Instance.useVolume = !SettingManager.Instance.useVolume;
        SetVolumeSprite();
    }
    void SetVolumeSprite()
    {
        volumeBtn.gameObject.GetComponent<Image>().sprite = SettingManager.Instance.useVolume ? volumeSprite : mutedVolumeSprite;
    }
    public void OnGuideLineToggle()
    {
        SettingManager.Instance.useGuideLine = guideLineToggle.isOn;
    }
    public void CloseBtn()
    {
        Time.timeScale = 1.0f;
        if (GameManager.Instance.isStartGame)
        {
            Invoke("isPauseDisable", 0.05f); //닫기 버튼 누른걸로 공 발사되는거 방지
        }
        PauseUI.SetActive(false);
        SettingUI.SetActive(false);
    }
    void isPauseDisable()
    {
        GameManager.Instance.isPause = false;
    }
    public void PauseBtn()
    {
        Time.timeScale = 0f;
        GameManager.Instance.isPause = true;
        PauseUI.SetActive(true);
    }
    public void SettingBtn()
    {
        Time.timeScale = 0f;
        GameManager.Instance.isPause = true;
        SettingUI.SetActive(true);
    }
    public void RankBtn()
    {

    }
    public void DisableBallCountUI()
    {
        ballCountText.gameObject.SetActive(false);
    }
    public void SetBallCountUI()
    {
        ballCountText.transform.position = Camera.main.WorldToScreenPoint(GameManager.Instance.ballPosition.position + Vector3.up * 1f);
        ballCountText.gameObject.SetActive(true);
        ballCountText.text = "X" + (GameManager.Instance.balls.Count + GameManager.Instance.inactivatedBalls.Count);
    }
    public void SetScoreUI()
    {
        highScoreText.text = "최고기록 : " + GameManager.Instance.highScore;
        scoreText.text = "현재점수 : " + GameManager.Instance.score;
    }
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}