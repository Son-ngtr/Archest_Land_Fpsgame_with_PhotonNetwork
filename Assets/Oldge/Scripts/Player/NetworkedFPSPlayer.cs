using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonView))]
public class NetworkedFPSPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    CharacterController cc;
    Animator anim;
    PhotonView PV;

    [System.Serializable]
    public class AnimationStrings
    {
        public string forward = "forward";
        public string strafe = "strafe";
        public string sprint = "sprint";
        public string aim = "aim";
        public string pull = "pullString";
        public string fire = "fire";
    }
    [SerializeField]
    public AnimationStrings animStrings;

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

    [Header("Movement Settings")]
    public float speed = 6.0f;
    public float sprintMultiplier = 1.5f;

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

    public Bow bowScript;
    bool isAiming;

    public bool testAim;

    bool hitDetected;

    Animator playerAnim;
    CharacterController chc;

    // Start is called before the first frame update
    void Start()
    {
        chc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            camCenter = Camera.main.transform.parent;
            mainCam = Camera.main.transform;
        }
        else
        {
            // Disable components that should only be active for the local player
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            HandleInput();
            HandleMovement();
            HandleActions();
        }
    }

    void HandleInput()
    {
        if (Input.GetAxis(input.forwardInput) != 0 || Input.GetAxis(input.strafeInput) != 0)
            RotateToCamView();

        if (!chc.isGrounded)
        {
            chc.Move(new Vector3(0, -gravityValue, 0) * Time.deltaTime);
        }

        isAiming = Input.GetButton(input.aim);

        if (testAim)
            isAiming = true;

        if (bowScript.bowSettings.arrowCount < 1)
            isAiming = false;
    }

    void HandleMovement()
    {
        float forward = Input.GetAxis(input.forwardInput);
        float strafe = Input.GetAxis(input.strafeInput);
        bool isSprinting = Input.GetButton(input.sprintInput);

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        Vector3 move = transform.right * strafe + transform.forward * forward;
        chc.Move(move * currentSpeed * Time.deltaTime);

        anim.SetFloat(animStrings.forward, forward);
        anim.SetFloat(animStrings.strafe, strafe);
        anim.SetBool(animStrings.sprint, isSprinting);
        anim.SetBool(animStrings.aim, isAiming);
    }

    void HandleActions()
    {
        if (isAiming)
        {
            Aim();
            bowScript.EquipBow();

            if (bowScript.bowSettings.arrowCount > 0)
                anim.SetBool(animStrings.pull, Input.GetButton(input.fire));

            if (Input.GetButtonUp(input.fire))
            {
                anim.SetTrigger(animStrings.fire);
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

    private void OnAnimatorIK(int layerIndex)
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
    }

    // Synchronize player data across the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
