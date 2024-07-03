using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.SetGroudedState(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.SetGroudedState(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.SetGroudedState(true);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.SetGroudedState(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.SetGroudedState(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
        {
            return;
        }
        playerController.SetGroudedState(true);
    }


}
