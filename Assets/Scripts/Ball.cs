using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static bool ballHitGroundExist = false;
    public bool isShooted = false;
    public bool isActivated;
    [SerializeField] Color activatedColor;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //속도보정
        if(isShooted && isActivated && GameManager.Instance.isStartGame)
        {
            if (Mathf.Abs(rb.velocity.y) < 5f)
            {
                if (rb.velocity.y != 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * 5f);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 5f);
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        {
            if (collision.transform.CompareTag("Border") || collision.transform.CompareTag("Block"))
            {
                //// 여러 접촉점이 있으면 법선을 평균내서 모서리 충돌 안정화
                //Vector2 avgNormal = Vector2.zero;
                //Vector2 reflect = Vector2.zero;
                //foreach (var contact in collision.contacts)
                //{
                //    avgNormal += contact.normal;
                //}
                //avgNormal.Normalize();

                //// avgNormal이 너무 비스듬하면, X/Y 중 큰 축으로 정렬 (모서리 안정화)
                //if (Mathf.Abs(avgNormal.x) > Mathf.Abs(avgNormal.y))
                //    avgNormal = new Vector2(Mathf.Sign(avgNormal.x), 0);
                //else
                //    avgNormal = new Vector2(0, Mathf.Sign(avgNormal.y));
                //reflect = Vector2.Reflect(curVelocity, avgNormal).normalized;
                ////Debug.Log(reflect);;

                //ContactPoint2D cp = collision.contacts[0];
                //float radius = ((CircleCollider2D)collider).radius * transform.localScale.x;
                //Vector2 safePos = cp.point + avgNormal * radius;

                //transform.position = safePos;
                ////rb.velocity = reflect * GameManager.Instance.ballSpeed;
            }
            else if (collision.transform.CompareTag("Bottom") && isShooted)
            {
                transform.position = new Vector3(transform.position.x, GameManager.Instance.ballPosition.position.y, 0);
                isShooted = false;
                rb.velocity = Vector2.zero;
                if (isActivated)
                {
                    if (!ballHitGroundExist)
                    {
                        ballHitGroundExist = true;
                        GameManager.Instance.ballPosition.position = transform.position;
                    }
                    else
                    {
                        MoveForBallPosition();
                    }

                    if (GameManager.Instance.CanShoot)
                    {
                        GameManager.Instance.Next();
                    }
                }
            }
        }
    }
    public void Activate()
    {
        GetComponent<SpriteRenderer>().color = activatedColor;
        GameManager.Instance.inactivatedBalls.Remove(this);
        GameManager.Instance.balls.Add(this);
        transform.GetChild(0).gameObject.SetActive(true);
        isShooted = false;
        isActivated = true;
        gameObject.layer = 8;
        transform.name = "ball";
    }
    public void MoveForBallPosition()
    {
        StartCoroutine(_MoveForBallPosition());
    }
    IEnumerator _MoveForBallPosition()
    {
        while(!(transform.position.x <= GameManager.Instance.ballPosition.position.x + 0.55f && transform.position.x >= GameManager.Instance.ballPosition.position.x - 0.55f))
        {
            transform.position += (GameManager.Instance.ballPosition.position - transform.position).normalized * 1f;
            yield return new WaitForSeconds(0.02f);
        }
        transform.position = GameManager.Instance.ballPosition.position;
    }
}
