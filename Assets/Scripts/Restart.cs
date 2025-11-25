using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    /// <summary>
    /// FadeOut 애니메이션에서 실행할 함수
    /// </summary>
    public void RestartGame()
    {
        GameManager.Instance.ReloadMain();
    }
}
