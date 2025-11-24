using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerControl : MonoBehaviour
{
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
        if (!GameManager.Instance.isStartGame) { return; }
        if (!Input.GetMouseButtonUp(0)) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 9f) { return; }
        if (!GameManager.Instance.CanShoot) { return; }
        if (GameManager.Instance.isPause) { return; }
        GameManager.Instance.ActivateBalls();
        UIManager.Instance.DisableBallCountUI();
        canShoot = false;
        StartCoroutine(_ShootBalls());

    }
    IEnumerator _ShootBalls()
    {
        Ball.ballHitGroundExist = false;
        for (int i = 0; i < GameManager.Instance.balls.Count; i++)
        {
            Ball ball = GameManager.Instance.balls[i];
            ball.gameObject.GetComponent<Rigidbody2D>().velocity = guideLine.up * GameManager.Instance.ballSpeed;
            ball.isShooted = true;
            yield return new WaitForSeconds(ballDelay);
        }
        
    }
    void SetGuideLine()
    {
        if (!GameManager.Instance.isStartGame) { return; }
        if (!GameManager.Instance.CanShoot) { return; }
        if (!Input.GetMouseButton(0)) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < -12f) { return; }
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 9f) { return; }
        if (GameManager.Instance.isPause) { return; }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector2 dir = (mousePos - guideLine.position).normalized;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        if (targetAngle > 180f)
            targetAngle -= 360f;
        else if (targetAngle < -180f)
            targetAngle += 360f;

        float clampedAngle = Mathf.Clamp(targetAngle, -80, 80);

        guideLine.rotation = Quaternion.Euler(0, 0, clampedAngle);
        guideLine.gameObject.SetActive(true);
    }
}
