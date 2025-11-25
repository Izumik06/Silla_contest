using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] GameObject inactivatedBallPrefab;
    [SerializeField] GameObject particlePrefab;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
            ball.GetComponent<Rigidbody2D>().AddForce(Vector2.down * GameManager.Instance.ballSpeed, ForceMode2D.Impulse);
            GameManager.Instance.items.Remove(this);

            GameObject particle = Instantiate(particlePrefab);
            particle.transform.position = transform.position;

            audioSource.Play();

            Destroy(gameObject);
        }
    }
}
