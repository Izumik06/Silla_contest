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

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        guideLine.up = (mousePos - guideLine.position).normalized;

        if (guideLine.eulerAngles.z > angle && guideLine.eulerAngles.z < 180f)
        {
            guideLine.eulerAngles = new Vector3(0, 0, 80);
        }
        else if (guideLine.eulerAngles.z < 360 - angle && guideLine.eulerAngles.z > 180)
        {
            guideLine.eulerAngles = new Vector3(0, 0, -80);
        }
        
    }
}
