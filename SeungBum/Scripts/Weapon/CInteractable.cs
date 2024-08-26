using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CInteractable : MonoBehaviour
{
    #region protected ����
    protected int nActionNumber;
    #endregion

    /// <summary>
    /// Interaction�� ������ ��ü�� ������ �ִ� ���� ActionNumber�� �� (Hand Animation�� �����ϱ� ���Ѱ�)
    /// </summary>
    public virtual int ActionNumber
    {
        get
        {
            return nActionNumber;
        }

        set
        {
            nActionNumber = value;
        }
    }
}
