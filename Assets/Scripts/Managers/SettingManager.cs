using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private static SettingManager instance;

    public List<AudioSource> audioSources = new List<AudioSource>();
    
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
        StartCoroutine(PeriodicClearAudioSource());
    }
    IEnumerator PeriodicClearAudioSource()
    {
        while (true)
        {
            ClearAudioSources();
            yield return new WaitForSeconds(10f);
        }
    }
    public void ApplyVolume()
    {
        ClearAudioSources();
        for(int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].mute = !useVolume;
            audioSources[i].volume = volume;
        }
    }
    void ClearAudioSources()
    {
        audioSources = audioSources.Where(_ => _ != null).ToList();
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
