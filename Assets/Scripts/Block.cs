using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public int level = 0;
    public int hp;
    [SerializeField] TextMeshPro hpText;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetStatus();

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
        for(int i = 0; i < frame; i++)
        {
            transform.position += Vector3.down * (2f / frame);
            yield return new WaitForSeconds(0.1f / frame);
        }
        level++;
        transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            hp -= 1;
            if (hp <= 0)
            {
                GameManager.Instance.blocks.Remove(this);
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(ColorChange());
            }
        }
    }
    IEnumerator ColorChange()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.7f);
        yield return new WaitForSeconds(0.1f);
        SetStatus();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
    }
    public void SetStatus()
    {
        hpText.text = hp.ToString();
        spriteRenderer.color = Color.HSVToRGB((30 - 25 * ((float)hp / GameManager.Instance.maxHp)) / 360f, (40 + 30 * ((float)hp / GameManager.Instance.maxHp)) / 100f, 1f);
    }
}
