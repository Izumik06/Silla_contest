using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] Transform guideLine; //가이드라인 오브젝트
    [SerializeField] Transform ballPosition; //공 위치
    [SerializeField] float angle; //최대, 최소 각도
    [SerializeField] float ballDelay; //공 발사 간격

    void Update()
    {
        SetGuideLine();
        ShootBalls();
    }

    /// <summary>
    /// 공 발사
    /// </summary>
    void ShootBalls()
    {
        if (!GameManager.Instance.isStartGame) { return; }
        if (!Input.GetMouseButtonUp(0)) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 9f) { return; }
        if (!GameManager.Instance.CanShoot) { return; }
        if (GameManager.Instance.isPause) { return; }

        GameManager.Instance.ActivateBalls();
        UIManager.Instance.DisableBallCountUI();
        StartCoroutine(_ShootBalls());
    }

    /// <summary>
    /// 직접적인 공 발사 구현
    /// </summary>
    /// <returns></returns>
    IEnumerator _ShootBalls()
    {
        Ball.ballHitGroundExist = false;
        float ballSpeed = SettingManager.Instance.ballSpeed; //발사 후 공속도 변경 방지
        for (int i = 0; i < GameManager.Instance.balls.Count; i++)
        {
            Ball ball = GameManager.Instance.balls[i];
            ball.gameObject.GetComponent<Rigidbody2D>().velocity = guideLine.up * ballSpeed; 
            ball.isShooted = true;
            yield return new WaitForSeconds(ballDelay);
        }
        GameManager.Instance.isAnyBallShoot = true;
    }

    /// <summary>
    /// 발사 각도, 공 도착 지점 표시
    /// </summary>
    void SetGuideLine()
    {
        if (!GameManager.Instance.isStartGame) { return; }
        if (!GameManager.Instance.CanShoot) { return; }
        if (!Input.GetMouseButton(0)) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < -12f) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 9f) { return; }
        if (GameManager.Instance.isPause) { return; }

        //마우스 위치 가져오기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector2 dir = (mousePos - guideLine.position).normalized;

        //벡터를 각도로 변환
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        //각도를 -180 ~ 180까지의 값으로 표기
        if (targetAngle > 180f)
            targetAngle -= 360f;
        else if (targetAngle < -180f)
            targetAngle += 360f;

        //각도를 -80 ~ 80사이의 값으로 변경
        float clampedAngle = Mathf.Clamp(targetAngle, -angle, angle);

        //각도 변경
        guideLine.rotation = Quaternion.Euler(0, 0, clampedAngle);
        guideLine.gameObject.SetActive(true);
    }
}
