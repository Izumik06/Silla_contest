using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private static SettingManager instance;
    public bool useGuideLine;
    public bool useVolume;
    public float volume;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (PlayerPrefs.HasKey("useGuideLine"))
        {
            useGuideLine = System.Convert.ToBoolean(PlayerPrefs.GetInt("useGuideLine"));
            useVolume = System.Convert.ToBoolean(PlayerPrefs.GetInt("useVolume"));
            volume = PlayerPrefs.GetFloat("volume");
        }
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("useGuideLine", System.Convert.ToInt16(useGuideLine));
        PlayerPrefs.SetInt("useVolume", System.Convert.ToInt16(useVolume));
        PlayerPrefs.SetFloat("volume", volume);
    }
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
