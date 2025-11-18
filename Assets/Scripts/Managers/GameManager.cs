using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public List<GameObject> balls = new List<GameObject>();
    public Transform ballPosition;
    public int ballSpeed;
    public int maxHp;
    public int score;

    public  float[] probability = { 0.5f, 0.5f, 0, 0 };

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);   
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(GetProbability());
        }
    }
    int GetProbability()
    {
        float randValue = Random.Range(0f, 1f);

        float beforeProbability = 0;
        for(int i = 0; i < 4; i++)
        {
            beforeProbability += probability[i];
            if(randValue < beforeProbability)
            {
                Debug.Log(randValue);
                return i + 1;
            }
        }
        return -1;
    } 
    public bool CanShoot
    {
        get
        {
            return balls.Find(_ => _.GetComponent<Ball>().isShooted) == null;
        }
    }

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
