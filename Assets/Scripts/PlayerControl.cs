using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform guideLine;
    [SerializeField] Transform ballPosition;
    [SerializeField] float angle;
    [SerializeField] float ballDelay;
    public bool canShoot;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetGuideLine();
        ShootBalls();
    }
    void ShootBalls()
    {
        if (!Input.GetMouseButtonUp(0)) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 12f) { return; }
        if (!GameManager.Instance.CanShoot) { return; }

        canShoot = false;
        StartCoroutine(_ShootBalls());

    }
    IEnumerator _ShootBalls()
    {
        Ball.ballHitGroundExist = false;
        for (int i = 0; i < GameManager.Instance.balls.Count; i++)
        {
            GameObject ball = GameManager.Instance.balls[i];
            ball.GetComponent<Rigidbody2D>().velocity = guideLine.up * GameManager.Instance.ballSpeed;
            ball.GetComponent<Ball>().isShooted = true;
            yield return new WaitForSeconds(ballDelay);
        }
        
    }
    void SetGuideLine()
    {
        if (!GameManager.Instance.CanShoot) { return; }
        if (!Input.GetMouseButton(0)) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < -12f) { return; }
        guideLine.gameObject.SetActive(true);

        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePos.z = 0;

        //guideLine.up = (mousePos - guideLine.position).normalized;

        //if (guideLine.eulerAngles.z > angle && guideLine.eulerAngles.z < 180f)
        //{
        //    guideLine.eulerAngles = new Vector3(0, 0, 80);
        //}
        //else if (guideLine.eulerAngles.z < 360 - angle && guideLine.eulerAngles.z > 180)
        //{
        //    guideLine.eulerAngles = new Vector3(0, 0, -80);
        //}
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 가이드라인의 기준 방향(예: 위쪽)
        Vector2 dir = (mousePos - guideLine.position).normalized;

        // 현재 마우스가 만드는 각도(라디안 → 도)
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        if (targetAngle > 180f)
            targetAngle -= 360f;
        else if (targetAngle < -180f)
            targetAngle += 360f;

        // -angle ~ +angle 범위로 제한
        float clampedAngle = Mathf.Clamp(targetAngle, -85, 85);

        // 최종 회전 적용
        guideLine.rotation = Quaternion.Euler(0, 0, clampedAngle);
    }
}
