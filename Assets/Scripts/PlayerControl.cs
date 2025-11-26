using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] Transform guideLine; //���̵���� ������Ʈ
    [SerializeField] Transform ballPosition; //�� ��ġ
    [SerializeField] float angle; //�ִ�, �ּ� ����
    [SerializeField] float ballDelay; //�� �߻� ����

    void Update()
    {
        SetGuideLine();
        ShootBalls();
    }

    /// <summary>
    /// �� �߻�
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
    /// �������� �� �߻� ����
    /// </summary>
    /// <returns></returns>
    IEnumerator _ShootBalls()
    {
        Ball.ballHitGroundExist = false;
        float ballSpeed = SettingManager.Instance.ballSpeed; //�߻� �� ���ӵ� ���� ����
        for (int i = 0; i < GameManager.Instance.balls.Count; i++)
        {
            Ball ball = GameManager.Instance.balls[i];
            ball.gameObject.GetComponent<Rigidbody2D>().linearVelocity = guideLine.up * ballSpeed; 
            ball.isShooted = true;
            yield return new WaitForSeconds(ballDelay);
        }
        GameManager.Instance.isAnyBallShoot = true;
    }

    /// <summary>
    /// �߻� ����, �� ���� ���� ǥ��
    /// </summary>
    void SetGuideLine()
    {
        if (!GameManager.Instance.isStartGame) { return; }
        if (!GameManager.Instance.CanShoot) { return; }
        if (!Input.GetMouseButton(0)) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < -12f) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 9f) { return; }
        if (GameManager.Instance.isPause) { return; }

        //���콺 ��ġ ��������
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector2 dir = (mousePos - guideLine.position).normalized;

        //���͸� ������ ��ȯ
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        //������ -180 ~ 180������ ������ ǥ��
        if (targetAngle > 180f)
            targetAngle -= 360f;
        else if (targetAngle < -180f)
            targetAngle += 360f;

        //������ -80 ~ 80������ ������ ����
        float clampedAngle = Mathf.Clamp(targetAngle, -angle, angle);

        //���� ����
        guideLine.rotation = Quaternion.Euler(0, 0, clampedAngle);
        guideLine.gameObject.SetActive(true);
    }
}
