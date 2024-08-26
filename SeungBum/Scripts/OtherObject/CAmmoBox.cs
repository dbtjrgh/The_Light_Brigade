using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAmmoBox : MonoBehaviour
{
    #region private ����
    [SerializeField]
    List<AudioClip> soundEffect;

    int nActionNumber;
    #endregion

    /// <summary>
    /// źâ �ڽ� �׷� ����
    /// </summary>
    public AudioClip SoundGrab
    {
        get
        {
            return soundEffect[0];
        }
    }

    /// <summary>
    /// źâ �ڽ� �μ����� ����
    /// </summary>
    public AudioClip SoundBreak
    {
        get
        {
            return soundEffect[1];
        }
    }

    /// <summary>
    /// AmmoBox�� ����� Hand Animation�� Number��
    /// </summary>
    public int ActionNumber
    {
        get
        {
            return nActionNumber;
        }
    }

    void Start()
    {
        nActionNumber = 99;
    }
}