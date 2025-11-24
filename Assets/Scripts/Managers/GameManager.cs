using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public void Next()
    {
        score++;
        if(score >= highScore)
        {
            highScore = score;
        }
        maxHp++;
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
        while(balls.Count > 0)
        {
            Destroy(balls[0].gameObject);
            balls.RemoveAt(0);
        }
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
        }
        GameObject item = Instantiate(itemPrefab);
        item.transform.position = new Vector2(position[Random.Range(0, position.Count)], 8f);
        item.transform.parent = itemParent;
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
