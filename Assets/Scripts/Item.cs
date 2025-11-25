using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] GameObject inactivatedBallPrefab;
    [SerializeField] GameObject particlePrefab;

    /// <summary>
    /// 한 칸 아래로 이동
    /// </summary>
    public void Move()
    {
        StartCoroutine(_Move());
    }

    /// <summary>
    /// 실질적인 이동 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator _Move()
    {
        int frame = 10;
        for (int i = 0; i < frame; i++)
        {
            transform.position += Vector3.down * (2f / frame);
            yield return new WaitForSeconds(0.1f / frame);
        }
        //애매한 위치값 보정
        transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //공 또는 바닥과 충돌하였을 경우 실행
        if (collision.transform.CompareTag("Ball") || collision.transform.CompareTag("Bottom"))
        {
            //공 생성
            GameObject ball = Instantiate(inactivatedBallPrefab);
            GameManager.Instance.inactivatedBalls.Add(ball.GetOrAddComponent<Ball>());
            ball.transform.parent = GameManager.Instance.ballParent;
            ball.transform.position = transform.position;
            ball.GetComponent<Rigidbody2D>().AddForce(Vector2.down * GameManager.Instance.ballSpeed, ForceMode2D.Impulse);

            //파티클 생성
            GameObject particle = Instantiate(particlePrefab);
            particle.transform.position = transform.position;

            //소리 재생
            AudioManager.Instance.PlaySound(SoundType.GetItem);

            //자신 삭제
            GameManager.Instance.items.Remove(this);
            Destroy(gameObject);
        }
    }
}
