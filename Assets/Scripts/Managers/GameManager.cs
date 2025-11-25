using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public List<Ball> balls = new List<Ball>();
    public List<Ball> inactivatedBalls = new List<Ball>();
    public List<Block> blocks = new List<Block>();
    public List<Item> items = new List<Item>();
    public Transform ballPosition;
    public Transform ballParent;
    public Transform blockParent;
    public Transform itemParent;
    [SerializeField] GameObject BlockPrefab;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject ballParticlePrefab;
    public int ballSpeed;
    public int maxHp;
    public int score;
    public int highScore;
    public bool isPause;
    public bool isStartGame;

    float[] probability = { 0.5f, 0.5f, 0, 0 };

    //Singleton 패턴
    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        //게임 시작시 최고기록 조회
        if (PlayerPrefs.HasKey("highScore"))
        {
            highScore = PlayerPrefs.GetInt("highScore");
        }
        else
        {
            highScore = 0;
        }
    }

    /// <summary>
    /// 화면을 다시 불러와 게임 초기화면(메인메뉴)을 보여줌
    /// </summary>
    public void ReloadMain()
    {
        SettingManager.Instance.SaveSetting();
        PlayerPrefs.SetInt("highScore", highScore);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// 다음 단계로 넘어감
    /// </summary>
    public void Next()
    {
        //점수 증가
        score++; 
        if(score >= highScore)
        {
            highScore = score;
        }

        //블럭 최대체력 증가
        maxHp++;

        //진행도에 따라 블럭 생성 개수 확률 조정
        if(score == 5)
        {
            probability = new float[4] { 0.25f, 0.5f, 0.25f, 0 };
        }
        else if(score == 10)
        {
            probability = new float[4] { 0f, 0.5f, 0.25f, 0.25f };
        }
        else if(score == 20)
        {
            probability = new float[4] { 0f, 0.25f, 0.25f, 0.5f };
        }

        CreateObjects();
        MoveObjects();
        MoveInactivatedBalls();
        SetBlockColors();

        UIManager.Instance.SetScoreUI();

        //게임 오버 판단
        if (blocks.Find(_ => _.level >= 8) != null)
        {
            GameOver();
            return;
        }
        UIManager.Instance.SetBallCountUI();
    }

    /// <summary>
    /// 게임 오버시 실행되는 함수
    /// </summary>
    void GameOver()
    {
        //게임 중지
        isStartGame = false;
        while (balls.Count > 0)
        {
            Destroy(balls[0].gameObject);
            balls.RemoveAt(0);
        }

        //소리 재생
        AudioManager.Instance.PlaySound(SoundType.GameOver);

        //파티클 생성
        GameObject particle = Instantiate(ballParticlePrefab);
        particle.transform.position = ballPosition.position;

        //UI설정
        PlayerPrefs.SetInt("highScore", highScore);
        UIManager.Instance.SetGameOverUI();
    }

    /// <summary>
    /// probability 배열을 기반으로 랜덤한 블럭 개수 도출
    /// </summary>
    /// <returns></returns>
    int GetProbability()
    {
        float randValue = Random.Range(0f, 1f);

        float beforeProbability = 0;
        for (int i = 0; i < 4; i++)
        {
            beforeProbability += probability[i];
            if (randValue < beforeProbability)
            {
                Debug.Log(randValue);
                return i + 1;
            }
        }
        return -1;
    }

    /// <summary>
    /// 블럭 전체의 색을 재설정
    /// </summary>
    public void SetBlockColors()
    {
        for(int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetStatus();
        }
    }

    /// <summary>
    /// 블럭, 아이템 생성
    /// </summary>
    public void CreateObjects()
    {
        //블럭 생성
        int gen = GetProbability();
        List<float> position = new List<float>(){ -7.5f, -4.5f, -1.5f, 1.5f, 4.5f, 7.5f };

        for (int i = 0; i < gen; i++)
        {
            int randIdx = Random.Range(0, position.Count);
            Vector2 spawnPos = new Vector2(position[randIdx], 8f);
            position.RemoveAt(randIdx);

            GameObject block = Instantiate(BlockPrefab);
            block.GetComponent<Block>().hp = maxHp;
            block.transform.parent = blockParent;
            block.transform.position = spawnPos;

            blocks.Add(block.GetComponent<Block>());
        }

        //아이템 생성
        GameObject item = Instantiate(itemPrefab);
        item.transform.position = new Vector2(position[Random.Range(0, position.Count)], 8f);
        item.transform.parent = itemParent;
        items.Add(item.GetComponent<Item>());
    }

    /// <summary>
    /// 모든 블럭, 아이템을 한 칸 아래로 이동
    /// </summary>
    void MoveObjects()
    {
        for(int i = 0; i < blocks.Count; i++)
        {
            blocks[i].Move();
        }
        for(int i = 0; i < items.Count; i++)
        {
            items[i].Move();
        }
    }

    /// <summary>
    /// 모든 비활성화 상태인 공을 발사 위치로 이동
    /// </summary>
    void MoveInactivatedBalls()
    {
        if(inactivatedBalls.Count == 0) { return; }
        AudioManager.Instance.PlaySound(SoundType.GetBall);
        for(int i = 0; i < inactivatedBalls.Count; i++)
        {
            inactivatedBalls[i].GetComponent<Ball>().MoveForBallPosition();   
        }
    }

    /// <summary>
    /// 모든 비활성화 상태 공을 활성화함
    /// </summary>
    public void ActivateBalls()
    {
        while (inactivatedBalls.Count > 0)
        {
            inactivatedBalls[0].Activate();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("highScore", highScore); //프로그램 종료 전 최고점수 저장
    }

    //발사 가능 여부
    public bool CanShoot
    {
        get
        {
            return balls.Find(_ => _.isShooted) == null; //현재 모든 공이 바닥에 있는지를 기준으로 판단
        }
    }

    //Singleton 패턴
    public static GameManager Instance
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
