using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Portal destinationPortal;
    [SerializeField]
    private Transform portalTrans => transform; // Ponto de destino do teleporte
    public float teleportHeightOffset = 1.0f; // Para evitar sobreposi��o de colisores
    public float teleportOffset = 2.0f; // Para evitar que o jogador fique dentro do novo portal

    private GameObject playerTarget; //referência do player que está sendo enviado pelo teleporte

    public void NotifyTeleportingPlayer(GameObject teleportingPlayer)
    {
        playerTarget = teleportingPlayer;
        Debug.Log("New player target" + playerTarget.name +  " in " + gameObject.name);
    }

    public Vector3 GetDestinationPosition()
    {
        return portalTrans.position + (portalTrans.forward * teleportOffset) + new Vector3(0, teleportHeightOffset, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(playerTarget == other.gameObject)
        {
            Debug.Log(gameObject.name + " " + playerTarget.gameObject.name);
            return;
        }
            

        if(other.tag == "Player")
        {
            destinationPortal.NotifyTeleportingPlayer(other.gameObject);
            other.gameObject.transform.position = destinationPortal.GetDestinationPosition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerTarget = null;
    }
}
