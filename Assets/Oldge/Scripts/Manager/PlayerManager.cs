using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    // SpawnPoint
    Vector3 p1 = new Vector3 (0, 0, 0);
    Vector3 p2 = new Vector3(8, 0, 5);
    Vector3 p3 = new Vector3(5, 0, 8);

    int kills;
    int deaths;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        // true if PV owned by local player
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    // Tạo "Playercontroller"
    void CreateController()
    {
        Debug.Log("Instantiated Player Controller");

        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
        Vector3 zeroRotation = Vector3.zero;

        //Random controller
        Vector3 randomP;
        int randomIndex = Random.Range(0, 3);

        Vector3[] positions = { p1, p2, p3 };

        switch (randomIndex)
        {
            case 0:
                randomP = p1;
                break;
            case 1:
                randomP = p2;
                break;
            case 2:
                randomP = p3;
                break;
            default:
                randomP = Vector3.zero;
                break;
        }

        // PhotonNetwork.Instantiate - tạo một bản sao mới của một prefab trên máy chủ Photon 
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID });

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), positions[Random.Range(0, positions.Length)], Quaternion.Euler(zeroRotation), 0, new object[] { PV.ViewID });

    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        // Finding playermanager related to a player
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
