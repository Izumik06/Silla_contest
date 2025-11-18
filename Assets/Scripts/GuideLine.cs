using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] GameObject previewObj;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) { gameObject.SetActive(false); }
        ShowLineRenderer();
    }
    void ShowLineRenderer()
    {
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, 0.375f, transform.up,Mathf.Infinity,  (1 << 6)|(1 << 7));
        if(hit2D.collider != null && !hit2D.transform.CompareTag("Bottom"))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.up * 12.5f);
            //Debug.Log(hit2D.transform.gameObject.name);
            previewObj.transform.position = transform.position + transform.up * hit2D.distance;
        }
    }
}
