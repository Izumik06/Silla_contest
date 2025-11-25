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
    [SerializeField] Slider ballSpeedSlider;
    [SerializeField] Toggle guideLineToggle;
    [SerializeField] Sprite volumeSprite;
    [SerializeField] Sprite mutedVolumeSprite;

    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject pauseBtn;

    [SerializeField] GameObject fadeOut;

    [SerializeField] GameObject gameOverUI;
    [SerializeField] TextMeshProUGUI gameOverHighScore;
    [SerializeField] TextMeshProUGUI gameOverScore;

    //Singleton 패턴
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
        //설정, 최고 점수에 따라 UI를 초기화
        SetSettingUI();
        SetScoreUI();
        SetBallCountUI();
    }

    /// <summary>
    /// SettingManager에 있는 설정에 따라 UI변경
    /// </summary>
    void SetSettingUI()
    {
        guideLineToggle.SetIsOnWithoutNotify(SettingManager.Instance.useGuideLine);
        volumeSlider.value = SettingManager.Instance.volume;
        ballSpeedSlider.value = SettingManager.Instance.ballSpeed;
        SetVolumeSprite();
    }

    /// <summary>
    /// 초기화 버튼 클릭하면 실행
    /// </summary>
    public void ResetSettingBtn()
    {
        SettingManager.Instance.SetDefault();
        SetSettingUI();
    }

    /// <summary>
    /// 버튼 클릭 사운드 실행
    /// </summary>
    public void PlayBtnSound()
    {
        AudioManager.Instance.PlaySound(SoundType.Button);
    }

    /// <summary>
    /// 게임오버 UI 표시 / 설정
    /// </summary>
    public void SetGameOverUI()
    {
        gameOverUI.SetActive(true);
        gameOverHighScore.text = highScoreText.text;
        gameOverScore.text = scoreText.text;
    }

    /// <summary>
    /// 게임 시작 버튼 클릭하면 실행
    /// </summary>
    public void StartBtn()
    {
        startBtn.SetActive(false); 
        pauseBtn.SetActive(true);
        GameManager.Instance.isStartGame = true;
        Invoke("Unpause", 0.05f); //게임 시작 버튼을 누른 것을 트리거로 공 발사 방지
        GameManager.Instance.Next();
    }

    /// <summary>
    /// 메인화면으로 가기 버튼 클릭하면 실행
    /// </summary>
    public void MainBtn()
    {
        fadeOut.SetActive(true);
    }

    /// <summary>
    /// 볼륨조절 슬라이더의 값이 변경되었을 때 실행
    /// </summary>
    public void VolumeSlider()
    {
        SettingManager.Instance.volume = volumeSlider.value;
        AudioManager.Instance.ApplyVolumeSetting();
    }

    /// <summary>
    /// 공 속도 조절 슬라이더의 값이 변경되었을 때 실행
    /// </summary>
    public void BallSpeedSlider()
    {
        SettingManager.Instance.ballSpeed = ballSpeedSlider.value;
    }

    /// <summary>
    /// 음소거 / 활성화 버튼 클릭하면 시행
    /// </summary>
    public void VolumeBtn()
    {
        SettingManager.Instance.useVolume = !SettingManager.Instance.useVolume;
        AudioManager.Instance.ApplyVolumeSetting();
        SetVolumeSprite();
    }

    /// <summary>
    /// 음소거 / 활성화 아이콘 변경
    /// </summary>
    void SetVolumeSprite()
    {
        volumeBtn.gameObject.GetComponent<Image>().sprite = SettingManager.Instance.useVolume ? volumeSprite : mutedVolumeSprite;
    }

    /// <summary>
    /// 조준선 토글의 값이 변경될 때 실행
    /// </summary>
    public void OnGuideLineToggle()
    {
        SettingManager.Instance.useGuideLine = guideLineToggle.isOn;
    }

    /// <summary>
    /// Pause, Setting창을 닫는 버튼 클릭하면 실행
    /// </summary>
    public void CloseBtn()
    {
        Time.timeScale = 1.0f;
        if (GameManager.Instance.isStartGame) //게임 시작 전 설정 UI를 닫음으로 게임 시작 방지
        {
            Invoke("Unpause", 0.05f); //닫기 버튼 누른걸로 공 발사되는거 방지
        }
        PauseUI.SetActive(false);
        SettingUI.SetActive(false);
    }

    /// <summary>
    /// Pause버튼을 클릭하면 실행
    /// </summary>
    public void PauseBtn()
    {
        Time.timeScale = 0f;
        GameManager.Instance.isPause = true;
        PauseUI.SetActive(true);
    }

    /// <summary>
    /// Setting버튼을 클릭하면 실행
    /// </summary>
    public void SettingBtn()
    {
        Time.timeScale = 0f;
        GameManager.Instance.isPause = true;
        SettingUI.SetActive(true);
    }

    /// <summary>
    /// 공 개수 표시 UI를 비활성화
    /// </summary>
    public void DisableBallCountUI()
    {
        ballCountText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 공 개수 표시 UI를 활성화 / 설정
    /// </summary>
    public void SetBallCountUI()
    {
        ballCountText.transform.position = Camera.main.WorldToScreenPoint(GameManager.Instance.ballPosition.position + Vector3.up * 1f);
        ballCountText.gameObject.SetActive(true);
        ballCountText.text = "X" + (GameManager.Instance.balls.Count + GameManager.Instance.inactivatedBalls.Count);
    }

    /// <summary>
    /// 스코어 표시
    /// </summary>
    public void SetScoreUI()
    {
        highScoreText.text = "최고기록 : " + GameManager.Instance.highScore;
        scoreText.text = "현재점수 : " + GameManager.Instance.score;
    }

    /// <summary>
    /// 퍼즈 풀기
    /// </summary>
    void Unpause()
    {
        GameManager.Instance.isPause = false;
    }

    //Singleton 패턴
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