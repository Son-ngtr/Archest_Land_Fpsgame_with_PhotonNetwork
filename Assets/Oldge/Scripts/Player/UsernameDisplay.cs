using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] TMP_Text text;

    void Start()
    {
        // Disable local username text
        if (playerPV.IsMine)
        {
            gameObject.SetActive(false);
        }

        text.text = playerPV.Owner.NickName;
    }
}
