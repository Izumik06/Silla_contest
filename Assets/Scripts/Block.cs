using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public int level = 0; //현재 공이 있는 행
    public int hp;

    [SerializeField] GameObject particlePrefab;
    [SerializeField] TextMeshPro hpText;

    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetStatus();
    }

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
        //위치 표시 변수 변경
        level++; 

        int frame = 10;
        for(int i = 0; i < frame; i++)
        {
            transform.position += Vector3.down * (2f / frame);
            yield return new WaitForSeconds(0.1f / frame);
        }
        
        //애매한 위치값 보정
        transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //공과 충돌하였을 경우
        if (collision.gameObject.CompareTag("Ball"))
        {
            hp -= 1;
            AudioManager.Instance.PlaySound(SoundType.Block); //사운드 실행

            if (hp <= 0)
            {
                //파티클 생성
                GameObject particle = Instantiate(particlePrefab);
                particle.transform.position = transform.position;

                //자신 삭제
                GameManager.Instance.blocks.Remove(this);
                Destroy(gameObject);
            }
            else
            {
                //색 변경
                StartCoroutine(ColorChange()); 
            }
        }
    }

    /// <summary>
    /// 색 변경
    /// </summary>
    /// <returns></returns>
    IEnumerator ColorChange()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.7f);
        yield return new WaitForSeconds(0.1f);
        SetStatus();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
    }

    /// <summary>
    /// GameManager의 maxHp를 참조하여 자신의 색 재설정
    /// </summary>
    public void SetStatus()
    {
        hpText.text = hp.ToString();
        spriteRenderer.color = Color.HSVToRGB((30 - 25 * ((float)hp / GameManager.Instance.maxHp)) / 360f, (40 + 30 * ((float)hp / GameManager.Instance.maxHp)) / 100f, 1f);
    }
}
