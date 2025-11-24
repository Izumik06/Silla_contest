using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] GameObject inactivatedBallPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move()
    {
        StartCoroutine(_Move());
    }
    IEnumerator _Move()
    {
        int frame = 10;
        for (int i = 0; i < frame; i++)
        {
            transform.position += Vector3.down * (2f / frame);
            yield return new WaitForSeconds(0.1f / frame);
        }
        transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Ball") || collision.transform.CompareTag("Bottom"))
        {
            GameObject ball = Instantiate(inactivatedBallPrefab);
            GameManager.Instance.inactivatedBalls.Add(ball.GetOrAddComponent<Ball>());
            ball.transform.parent = GameManager.Instance.ballParent;
            ball.transform.position = transform.position;

            GameManager.Instance.items.Remove(this);
            Destroy(gameObject);
        }
    }
}
