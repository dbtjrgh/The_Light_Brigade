using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINodeRotationController : MonoBehaviour
{
    #region private ����
    Transform tfTarget;
    #endregion

    void Awake()
    {
        tfTarget = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(tfTarget);
    }
}
