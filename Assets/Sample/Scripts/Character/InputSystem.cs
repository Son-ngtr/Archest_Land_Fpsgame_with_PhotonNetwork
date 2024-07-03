using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Animator))]
public class InputSystem : MonoBehaviourPunCallbacks
{
    Movement moveScript;

    [System.Serializable]
    public class InputSettings
    {
        public string forwardInput = "Vertical";
        public string strafeInput = "Horizontal";
        public string sprintInput = "Sprint";
        public string aim = "Fire2";
        public string fire = "Fire1";
    }
    [SerializeField]
    public InputSettings input;

    [Header("Camera & Character Syncing")]
    public float lookDIstance = 5;
    public float lookSpeed = 5;

    [Header("Aiming Settings")]
    RaycastHit hit;
    public LayerMask aimLayers;
    Ray ray;

    [Header("Spine Settings")]
    public Transform spine;
    public Vector3 spineOffset;

    [Header("Head Rotation Settings")]
    public float lookAtPoint = 2.8f;

    [Header("Gravity Settings")]
    public float gravityValue = 1.2f;

    Transform camCenter;
    Transform mainCam;

    [SerializeField] GameObject camCenterObj;
    [SerializeField] GameObject mainCamObj;

    public Bow bowScript;
    bool isAiming;

    public bool testAim;

    bool hitDetected;

    Animator playerAnim;
    CharacterController cc;

    void Start()
    {
        if (!photonView.IsMine) return;

        moveScript = GetComponent<Movement>();

        /*camCenter = Camera.main.transform.parent;
        mainCam = Camera.main.transform;*/

        camCenter = camCenterObj.transform;
        mainCam = mainCamObj.transform;

        playerAnim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }


    void Update()
    {
        if (!photonView.IsMine) return;

        // ... rest of your Update method ...
        if (Input.GetAxis(input.forwardInput) != 0 || Input.GetAxis(input.strafeInput) != 0)
            RotateToCamView();

        if (!cc.isGrounded)
        {
            cc.Move(new Vector3(transform.position.x, transform.position.y - gravityValue, transform.position.z));
        }

        isAiming = Input.GetButton(input.aim);

        if (testAim)
            isAiming = true;

        if (bowScript.bowSettings.arrowCount < 1)
            isAiming = false;

        moveScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.strafeInput));
        moveScript.SprintCharacter(Input.GetButton(input.sprintInput));
        moveScript.CharacterAim(isAiming);

        if (isAiming)
        {
            Aim();
            bowScript.EquipBow();

            if (bowScript.bowSettings.arrowCount > 0)
                moveScript.CharacterPullString(Input.GetButton(input.fire));

            if (Input.GetButtonUp(input.fire))
            {
                moveScript.CharacterFireArrow();
                if (hitDetected)
                {
                    bowScript.Fire(hit.point);
                }
                else
                {
                    bowScript.Fire(ray.GetPoint(300f));
                }
            }

        }
        else
        {
            bowScript.UnEquipBow();
            bowScript.RemoveCrosshair();
            DisableArrow();
            Release();
        }
    }


    void LateUpdate()
    {
        if (!photonView.IsMine) return;
     
        if (isAiming)
            RotateCharacterSpine();
    }

    void RotateToCamView()
    {
        Vector3 camCenterPos = camCenter.position;

        Vector3 lookPoint = camCenterPos + (camCenter.forward * lookDIstance);
        Vector3 direction = lookPoint - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation.x = 0;
        lookRotation.z = 0;

        Quaternion finalRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        transform.rotation = finalRotation;
    }

    //Does the aiming and sends a raycast to a target
    void Aim()
    {
        Vector3 camPosition = mainCam.position;
        Vector3 dir = mainCam.forward;

        ray = new Ray(camPosition, dir);
        if (Physics.Raycast(ray, out hit, 500f, aimLayers))
        {
            hitDetected = true;
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            bowScript.ShowCrosshair(hit.point);
        }
        else
        {
            hitDetected = false;
            bowScript.RemoveCrosshair();
        }
    }

    void RotateCharacterSpine()
    {
        RotateToCamView();
        spine.LookAt(ray.GetPoint(50));
        spine.Rotate(spineOffset);
    }


    public void Pull()
    {
        bowScript.PullString();
    }

    public void EnableArrow()
    {
        bowScript.PickArrow();
    }

    public void DisableArrow()
    {
        bowScript.DisableArrow();
    }

    public void Release()
    {
        bowScript.ReleaseString();
    }

    public void PlayPullSound()
    {
        bowScript.PullAudio();
    }

    /*private void OnAnimatorIK(int layerIndex)
    {
        if (isAiming)
        {
            playerAnim.SetLookAtWeight(1f);
            playerAnim.SetLookAtPosition(ray.GetPoint(lookAtPoint));
        }
        else
        {
            playerAnim.SetLookAtWeight(0);
        }
    }*/

    [PunRPC]
    public void FireArrow(Vector3 hitPoint)
    {
        moveScript.CharacterFireArrow();
        if (hitDetected)
        {
            bowScript.Fire(hitPoint);
        }
        else
        {
            bowScript.Fire(ray.GetPoint(300f));
        }
    }

    // Call this method instead of directly firing the arrow
    public void RequestFireArrow(Vector3 hitPoint)
    {
        photonView.RPC("FireArrow", RpcTarget.All, hitPoint);
    }

}
