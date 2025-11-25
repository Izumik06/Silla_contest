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

    AudioSource audioSource;
    [SerializeField] AudioClip bombSound;

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
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("highScore"))
        {
            highScore = PlayerPrefs.GetInt("highScore");
        }
        else
        {
            highScore = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ReloadMain()
    {
        PlayerPrefs.SetInt("highScore", highScore);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
    public void Next()
    {
        score++;
        if(score >= highScore)
        {
            highScore = score;
        }
        maxHp++;
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
        if (blocks.Find(_ => _.level >= 8) != null)
        {
            GameOver();
            return;
        }
        UIManager.Instance.SetBallCountUI();
    }
    void GameOver()
    {
        isStartGame = false;
        PlayerPrefs.SetInt("highScore", highScore);
        while (balls.Count > 0)
        {
            Destroy(balls[0].gameObject);
            balls.RemoveAt(0);
        }
        audioSource.clip = bombSound;
        audioSource.Play();
        GameObject particle = Instantiate(ballParticlePrefab);
        particle.transform.position = ballPosition.position;
        UIManager.Instance.SetGameOverUI();
    }
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
    public void SetBlockColors()
    {
        for(int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetStatus();
        }
    }
    public void CreateObjects()
    {
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
            SettingManager.Instance.audioSources.Add(block.GetComponent<AudioSource>());
        }
        GameObject item = Instantiate(itemPrefab);
        item.transform.position = new Vector2(position[Random.Range(0, position.Count)], 8f);
        item.transform.parent = itemParent;
        SettingManager.Instance.audioSources.Add(item.GetComponent<AudioSource>());
        items.Add(item.GetComponent<Item>());
    }
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
    void MoveInactivatedBalls()
    {
        if(inactivatedBalls.Count == 0) { return; }
        audioSource.Play();
        for(int i = 0; i < inactivatedBalls.Count; i++)
        {
            inactivatedBalls[i].GetComponent<Ball>().MoveForBallPosition();   
        }
    }
    public void ActivateBalls()
    {
        while (inactivatedBalls.Count > 0)
        {
            inactivatedBalls[0].Activate();
        }
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("highScore", highScore);
    }
    public bool CanShoot
    {
        get
        {
            return balls.Find(_ => _.isShooted) == null;
        }
    }

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
