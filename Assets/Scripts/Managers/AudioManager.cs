using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();

    static bool isFirstRun = true;

    // Start is called before the first frame update
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
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ApplyVolumeSetting(); //시작시 볼륨, 음소거 설정

        //게임을 최초 실행한 뒤 딱 한번 소리가 안나오는 버그 존재
        //최초 실행이라면 미리 소리 실행
        if (isFirstRun) 
        {
            audioSource.Play();
            isFirstRun = false;
        }
    }

    /// <summary>
    /// SettingManager을 기반으로 볼륨, 음소거 설정
    /// </summary>
    public void ApplyVolumeSetting()
    {
        audioSource.volume = SettingManager.Instance.volume;
        audioSource.mute = !SettingManager.Instance.useVolume;
    }

    /// <summary>
    /// soundType에 해당하는 사운드 실행
    /// </summary>
    /// <param name="soundType"></param>
    public void PlaySound(SoundType soundType)
    {
        audioSource.PlayOneShot(audioClips[(int)soundType]);
    }

    //Singleton 패턴
    public static AudioManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
public enum SoundType
{
    Button, Block, GetItem, GetBall, GameOver
}