using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBoltSlide : MonoBehaviour
{
    #region private ����
    [SerializeField]
    CWeaponController weaponController;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BoltSlide"))
        {
            weaponController.Slide();
        }
    }
}