using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private static SettingManager instance;
    
    public bool useGuideLine;
    public bool useVolume;
    public float volume;
    public float ballSpeed;

    private void Awake()
    {
        //Singleton 패턴
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //저장된 설정 불러오기
        if (PlayerPrefs.HasKey("useGuideLine"))
        {
            useGuideLine = System.Convert.ToBoolean(PlayerPrefs.GetInt("useGuideLine"));
            useVolume = System.Convert.ToBoolean(PlayerPrefs.GetInt("useVolume"));
            volume = PlayerPrefs.GetFloat("volume");
            ballSpeed = PlayerPrefs.GetFloat("ballSpeed");
        }
        else
        {
            SetDefault();
        }
    }

    //게임 종료시 설정값 저장
    private void OnApplicationQuit()
    {
        SaveSetting();
    }

    public void SetDefault()
    {
        volume = 1;
        useVolume = true;
        ballSpeed = 30f;
        useGuideLine = true;

        SaveSetting();
    }

    /// <summary>
    /// 설정 저장
    /// </summary>
    public void SaveSetting()
    {
        PlayerPrefs.SetInt("useGuideLine", System.Convert.ToInt16(useGuideLine));
        PlayerPrefs.SetInt("useVolume", System.Convert.ToInt16(useVolume));
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("ballSpeed", ballSpeed);
        
    }

    //Singleton 패턴
    public static SettingManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
