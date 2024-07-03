using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    private float timer = 0f;
    private bool isTimerRunning = false;
    private float lobbyTime = 300f;

    private void Awake()
    {
        // Kiểm tra nếu có RoomManager khác có tồn tại
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    // Đăng ký sự kiện OnSceneLoad cho đối tượng RoomManager
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // Kiểm tra Scene hiện tại, trạng thái 1 <-> Game Scene 
        if (scene.buildIndex == 1)
        {
            // Tạo đối tượng Prefabs - PlayerManager trên máy chủ Photon
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            StartTimer();
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            if (timer >= lobbyTime)
            {
                // Log out every player
                PhotonNetwork.Disconnect();
                isTimerRunning = false;
            }
        }
    }

    void StartTimer()
    {
        timer = 0f;
        isTimerRunning = true;
    }
}
