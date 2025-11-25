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

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0)) { gameObject.SetActive(false); } //클릭을 그만둘 때 자신을 비활성화
        ShowLineRenderer();
    }

    /// <summary>
    /// 선, 도착 위치 표시
    /// </summary>
    void ShowLineRenderer()
    {
        //조준선을 사용하지 않도록 설정되있다면 비활성화
        if (!SettingManager.Instance.useGuideLine) 
        { 
            lineRenderer.enabled = false;
            previewObj.SetActive(false);
            return;
        }
      
        //Ray를 이용하여 도착지점 예상
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, 0.375f, transform.up,Mathf.Infinity,  (1 << 6)|(1 << 7));
        if(hit2D.collider != null && !hit2D.transform.CompareTag("Bottom"))
        {
            //선의 끝 위치 설정
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.up * 12.5f);

            //예상 도착지점 표시
            previewObj.transform.position = transform.position + transform.up * hit2D.distance;

            //오브젝트, 렌더러 활성화
            lineRenderer.enabled = true;
            previewObj.SetActive(true);
        }
    }
}
