/*using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ArchorControllerUI : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject ui;

    [SerializeField] TextMeshProUGUI timerText;
    private float remainingTime = 300f;

    //[SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpead, jumpForce, smoothTime;


    float verticalLookRotation;

    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    *//* Photon View
     * - Cung cấp các chức năng để đồng bộ hóa trạng thái của các đối tượng giữa các máy khách trong trò chơi
     *  + (boolean) IsMine - Kiểm tra thuộc máy khách hiện tại 
     *//*
    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        // Tao tuong quan toi lop playerManager
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            // Remove ui canvas from other clients
            Destroy(ui);
        }
        
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        // Update the remaining time
        remainingTime -= Time.deltaTime;

        // Check if the time has reached 0
        if (remainingTime <= 0f)
        {
            // Perform any actions you need when the time is up
            // For example, log out the player
            PhotonNetwork.Disconnect();
            return;
        }

        // Convert the remaining time to a string format
        string timeText = FormatTime(remainingTime);

        // Update the timer text
        timerText.text = timeText;


        *//* Look();
         Move();*//*
        Jump();


        if (transform.position.y < -15f) // Chet khi roi khoi san
        {
            Die();
        }

    }

    *//*void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }*//*

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // compact check if move with/ without shift walk
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpead), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }


    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        // Làm cho tốc độ chuyển động không phụ thuộc vào fps local
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    // Gọi một phương thức trên một đối tượng từ xa
    // + Sử dụng để tạo cơ chế mất máu
    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        Debug.Log("Took damage" + damage);

        currentHealth -= damage;

        healthBarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die()
    {
        playerManager.Die();
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

}
*/

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchorControllerUI : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject ui;
    [SerializeField] TextMeshProUGUI timerText;

    private float remainingTime = 300f;
    private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpead, jumpForce, smoothTime;
    private float verticalLookRotation;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;

    private Rigidbody rb;
    private PhotonView PV;
    private PlayerManager playerManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }

        // Example to retrieve PlayerManager via instantiation data
        if (PV.IsMine && PV.InstantiationData != null && PV.InstantiationData.Length > 0)
        {
            int playerId = (int)PV.InstantiationData[0];
            playerManager = PhotonView.Find(playerId).GetComponent<PlayerManager>();
        }

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            PhotonNetwork.Disconnect();
            return;
        }

        string timeText = FormatTime(remainingTime);
        timerText.text = timeText;

        Jump();

        if (transform.position.y < -15f)
        {
            Die();
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpead), ref smoothMoveVelocity, smoothTime);
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        Debug.Log("Took damage: " + damage);

        currentHealth -= damage;
        healthBarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();

            // Example: Notify the player who dealt the damage
            if (info.Sender != null)
            {
                PlayerManager player = info.Sender.TagObject as PlayerManager;
                if (player != null)
                {
                    player.GetKill();
                }
            }
        }
    }

    void Die()
    {
        playerManager.Die();
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
