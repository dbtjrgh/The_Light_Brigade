using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class CHandAnimationController : MonoBehaviour
{
    #region public ����
    public Animator animator;
    public Transform tfHandOffsetNode;
    public Transform tfGloveMasterOffset;
    public Transform tfGrabPoseMovemonetNode;
    #endregion

    #region private ����
    XRRayInteractor rayInteractor;
    XRDirectInteractor directInteractor;

    InputActionProperty triggerAction;
    InputActionProperty selectAction;

    Vector3 v3GloveMasterOffset;
    Quaternion qMeshOffset;
    Quaternion qGloveMasterOffset;
    Quaternion qGrabPoseMovemonetNode;

    [SerializeField]
    bool isAnimatedHand;
    #endregion

    void Start()
    {
        //rayInteractor = transform.parent.GetComponentInChildren<XRRayInteractor>();
        //rayInteractor.selectEntered.AddListener(OnSelectEntered);
        //rayInteractor.selectExited.AddListener(OnSelectExited);

        //directInteractor = transform.parent.GetComponentInChildren<XRDirectInteractor>();
        //directInteractor.selectEntered.AddListener(OnSelectEntered);
        //directInteractor.selectExited.AddListener(OnSelectExited);

        if (!isAnimatedHand)
        {
            triggerAction = transform.parent.GetComponent<ActionBasedController>().activateAction;
            selectAction = transform.parent.GetComponent<ActionBasedController>().selectAction;
            InitAction();

            qMeshOffset = tfHandOffsetNode.localRotation;
            qGloveMasterOffset = tfGloveMasterOffset.localRotation;
            v3GloveMasterOffset = tfGloveMasterOffset.localPosition;
            qGloveMasterOffset = tfGrabPoseMovemonetNode.localRotation;
        }
    }

    /// <summary>
    /// RayInteractor, DirectInterator�� SelectEntered���� �� ����� �޼���
    /// </summary>
    /// <param name="args"></param>
    void OnSelectEntered(SelectEnterEventArgs args)
    {
        ResetPose();

        //animator.SetInteger("Action", args.interactableObject.transform.GetComponent<CInteractable>().ActionNumber);
    }

    /// <summary>
    /// RayInteractor, DirectInterator�� SelectExited���� �� ����� �޼���
    /// </summary>
    /// <param name="args"></param>
    void OnSelectExited(SelectExitEventArgs args)
    {
        InitPose();

        //animator.SetInteger("Action", 0);
    }

    /// <summary>
    /// Hand Action �ִϸ��̼��� ���
    /// </summary>
    /// <param name="actionNumber">����� �ִϸ��̼� Number</param>
    public void ActionAnimation(int actionNumber)
    {
        if (!isAnimatedHand)
        {
            if (actionNumber == 0)
            {
                InitPose();
            }

            else
            {
                ResetPose();
            }
        }

        animator.SetInteger("Action", actionNumber);
    }

    /// <summary>
    /// �÷��̾� Hand FBX ���� �θ���� Position, Rotation���� 0���� �ʱ�ȭ ��Ű�� ���� �޼��� 
    /// (Grab ���� �� Animation�� ���� Hand ��ġ�� ����ȭ�� ����)
    /// </summary>
    void ResetPose()
    {
        tfHandOffsetNode.localRotation = Quaternion.identity;

        tfGloveMasterOffset.localPosition = Vector3.zero;
        tfGloveMasterOffset.localRotation = Quaternion.identity;

        tfGrabPoseMovemonetNode.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// �÷��̾� Hand FBX ���� �θ���� Position, Rotation ���� �ٽ� �ʱⰪ���� �����ִ� �޼���
    /// (Grab�� Ǯ���� �� Hand ��ġ�� ����ȭ�� ����)
    /// </summary>
    void InitPose()
    {
        tfHandOffsetNode.localRotation = qMeshOffset;

        tfGloveMasterOffset.localPosition = v3GloveMasterOffset;
        tfGloveMasterOffset.localRotation = qGloveMasterOffset;

        tfGrabPoseMovemonetNode.localRotation = qGrabPoseMovemonetNode;
    }

    /// <summary>
    /// Trigger, Grab ��ư�� ������ �� Hand �ִϸ��̼��� �����ϴ� Action�� �߰��Ѵ�.
    /// </summary>
    void InitAction()
    {
        triggerAction.action.performed += (InputAction.CallbackContext context) => { animator.SetFloat("Grab", 1.0f); };
        triggerAction.action.canceled += (InputAction.CallbackContext context) => { animator.SetFloat("Grab", 0.0f); };
        selectAction.action.performed += (InputAction.CallbackContext context) => { animator.SetFloat("Grab", 2.0f); };
        selectAction.action.canceled += (InputAction.CallbackContext context) => { animator.SetFloat("Grab", 0.0f); };
    }
}
