/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    //This handles Camera Settings
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Camera Move Settings")]
        public float zoomSpeed = 5;
        public float moveSpeed = 5;
        public float rotationSpeed = 5;
        public float originalFieldofView = 70;
        public float zoomFieldofView = 20;
        public float MouseX_Sensitivity = 5;
        public float MouseY_Sensitivity = 5;
        public float MaxClampAngle = 90;
        public float MinClampAngle = -30;

        [Header("Camera Collision")]
        public Transform camPosition;
        public LayerMask camCollisionLayers;
    }
    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class CameraInputSettings
    {
        public string MouseXAxis = "Mouse X";
        public string MouseYAxis = "Mouse Y";
        public string AimingInput = "Fire2";
    }
    [SerializeField]
    public CameraInputSettings inputSettings;

    Transform center;
    Transform target;

    Camera mainCam;
    Camera UICam;

    float cameraXrotation = 0;
    float cameraYrotation = 0;

    Vector3 InitialCamPos;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;

        // hide cursor
        Cursor.visible = false;

        UICam = mainCam.GetComponentInChildren<Camera>();
        center = transform.GetChild(0);
        Debug.Log("center : " + center.name);
        FindPlayer();
        InitialCamPos = mainCam.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
            return;

        if (!Application.isPlaying)
            return;

        RotateCamera();
        ZoomCamera();
        HandleCamCollision();
    }

    void LateUpdate()
    {
        if (target)
        {
            FollowPlayer();
        }
        else
        {
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found. Ensure that there is a GameObject with the 'Player' tag in the scene.");
        }
    }

    void FollowPlayer()
    {
        Vector3 moveVector = Vector3.Lerp(transform.position, target.transform.position, cameraSettings.moveSpeed * Time.deltaTime);

        transform.position = moveVector;
    }

    void RotateCamera()
    {
        // camera direction
        cameraXrotation += Input.GetAxis(inputSettings.MouseYAxis) * cameraSettings.MouseY_Sensitivity;
        cameraYrotation -= Input.GetAxis(inputSettings.MouseXAxis) * cameraSettings.MouseX_Sensitivity;

        cameraXrotation = Mathf.Clamp(cameraXrotation, cameraSettings.MinClampAngle, cameraSettings.MaxClampAngle);

        cameraYrotation = Mathf.Repeat(cameraYrotation, 360);

        Vector3 rotatingAngle = new Vector3(-cameraXrotation, -cameraYrotation, 0);

        Quaternion rotation = Quaternion.Slerp(center.transform.localRotation, Quaternion.Euler(rotatingAngle), cameraSettings.rotationSpeed * Time.deltaTime);

        center.transform.localRotation = rotation;
    }
    

    void ZoomCamera()
    {
        if (Input.GetButton(inputSettings.AimingInput))
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.zoomFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
            UICam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.zoomFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.originalFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
            UICam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.originalFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
        }
    }

    void HandleCamCollision()
    {
        if (!Application.isPlaying)
            return;

        if (Physics.Linecast(target.transform.position + target.transform.up, cameraSettings.camPosition.position, out hit, cameraSettings.camCollisionLayers))
        {
            Vector3 newCamPos = new Vector3(hit.point.x + hit.normal.x * .2f, hit.point.y + hit.normal.y * .8f, hit.point.z + hit.normal.z * .2f);
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, newCamPos, Time.deltaTime * cameraSettings.moveSpeed);
        }
        else
        {
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, InitialCamPos, Time.deltaTime * cameraSettings.moveSpeed);
        }

        Debug.DrawLine(target.transform.position + target.transform.up, cameraSettings.camPosition.position, Color.blue);
    }
}

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[ExecuteInEditMode]
public class CameraController : MonoBehaviourPun
{
    //This handles Camera Settings
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Camera Move Settings")]
        public float zoomSpeed = 5;
        public float moveSpeed = 5;
        public float rotationSpeed = 5;
        public float originalFieldofView = 70;
        public float zoomFieldofView = 20;
        public float MouseX_Sensitivity = 5;
        public float MouseY_Sensitivity = 5;
        public float MaxClampAngle = 90;
        public float MinClampAngle = -30;

        [Header("Camera Collision")]
        public Transform camPosition;
        public LayerMask camCollisionLayers;
    }
    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class CameraInputSettings
    {
        public string MouseXAxis = "Mouse X";
        public string MouseYAxis = "Mouse Y";
        public string AimingInput = "Fire2";
    }
    [SerializeField]
    public CameraInputSettings inputSettings;

    Transform center;
    Transform target;

    Camera mainCam;
    Camera UICam;

    float cameraXrotation = 0;
    float cameraYrotation = 0;

    Vector3 InitialCamPos;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;

        // hide cursor
        Cursor.visible = false;

        UICam = mainCam.GetComponentInChildren<Camera>();
        center = transform.GetChild(0);
        Debug.Log("center : " + center.name);

        if (photonView.IsMine)
        {
            FindPlayer();
        }

        InitialCamPos = mainCam.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (target != null)
        {
            RotateCamera();
            ZoomCamera();
            HandleCamCollision();
        }
    }

    void LateUpdate()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            if (target != null)
            {
                FollowPlayer();
            }
            else
            {
                FindPlayer();
            }
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found. Ensure that there is a GameObject with the 'Player' tag in the scene.");
        }
    }

    void FollowPlayer()
    {
        Vector3 moveVector = Vector3.Lerp(transform.position, target.transform.position, cameraSettings.moveSpeed * Time.deltaTime);

        transform.position = moveVector;
    }

    void RotateCamera()
    {
        // camera direction
        cameraXrotation += Input.GetAxis(inputSettings.MouseYAxis) * cameraSettings.MouseY_Sensitivity;
        cameraYrotation -= Input.GetAxis(inputSettings.MouseXAxis) * cameraSettings.MouseX_Sensitivity;

        cameraXrotation = Mathf.Clamp(cameraXrotation, cameraSettings.MinClampAngle, cameraSettings.MaxClampAngle);

        cameraYrotation = Mathf.Repeat(cameraYrotation, 360);

        Vector3 rotatingAngle = new Vector3(-cameraXrotation, -cameraYrotation, 0);

        Quaternion rotation = Quaternion.Slerp(center.transform.localRotation, Quaternion.Euler(rotatingAngle), cameraSettings.rotationSpeed * Time.deltaTime);

        center.transform.localRotation = rotation;

        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("SyncCameraRotation", RpcTarget.Others, rotatingAngle);
        }
    }

    [PunRPC]
    void SyncCameraRotation(Vector3 rotatingAngle)
    {
        Quaternion rotation = Quaternion.Euler(rotatingAngle);
        center.transform.localRotation = rotation;
    }

    void ZoomCamera()
    {
        if (Input.GetButton(inputSettings.AimingInput))
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.zoomFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
            UICam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.zoomFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.originalFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
            UICam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.originalFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
        }

        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("SyncCameraZoom", RpcTarget.Others, mainCam.fieldOfView);
        }
    }

    [PunRPC]
    void SyncCameraZoom(float fov)
    {
        mainCam.fieldOfView = fov;
        UICam.fieldOfView = fov;
    }

    void HandleCamCollision()
    {
        if (Physics.Linecast(target.transform.position + target.transform.up, cameraSettings.camPosition.position, out hit, cameraSettings.camCollisionLayers))
        {
            Vector3 newCamPos = new Vector3(hit.point.x + hit.normal.x * .2f, hit.point.y + hit.normal.y * .8f, hit.point.z + hit.normal.z * .2f);
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, newCamPos, Time.deltaTime * cameraSettings.moveSpeed);
        }
        else
        {
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, InitialCamPos, Time.deltaTime * cameraSettings.moveSpeed);
        }

        Debug.DrawLine(target.transform.position + target.transform.up, cameraSettings.camPosition.position, Color.blue);

        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("SyncCameraPosition", RpcTarget.Others, mainCam.transform.position);
        }
    }

    [PunRPC]
    void SyncCameraPosition(Vector3 position)
    {
        mainCam.transform.position = position;
    }
}
