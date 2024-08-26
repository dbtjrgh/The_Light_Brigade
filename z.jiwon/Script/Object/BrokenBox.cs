using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ���� �ۼ� ��ũ��Ʈ
public class BrokenBox : MonoBehaviour
{
    public GameObject fragmentedVersion;
    public GameObject originulVersion;
    bool isGrab = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && isGrab)
        {
            CPlayerSoundManager.Instance.PlaySoundOneShot(GetComponent<CAmmoBox>().SoundBreak);

            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            originulVersion.SetActive(false);
            fragmentedVersion.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrab = true;
        }
    }
}
