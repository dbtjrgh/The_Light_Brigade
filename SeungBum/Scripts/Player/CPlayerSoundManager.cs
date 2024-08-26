using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerSoundManager : MonoBehaviour
{
    #region private ����
    static CPlayerSoundManager instance;
    AudioSource audioSource;
    #endregion

    public static CPlayerSoundManager Instance
    {
        get
        {
            if (instance is null)
            {
                instance = FindObjectOfType<CPlayerSoundManager>();

                if (instance is null)
                {
                    instance = new CPlayerSoundManager();
                    DontDestroyOnLoad(instance);
                }
            }

            return instance;
        }
    }

    void Awake()
    {
        if (instance is null)
        {
            instance = this;

            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(this);
        }

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// ȿ������ ����ϴ� �޼���
    /// </summary>
    /// <param name="audioClip">����� ȿ����</param>
    public void PlaySoundOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
