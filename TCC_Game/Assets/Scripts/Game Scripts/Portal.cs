using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destinationPad; // Ponto de destino do teleporte
    public float teleportHeightOffset = 1.0f; // Para evitar sobreposição de colisores
    public float teleportOffset = 2.0f; // Para evitar que o jogador fique dentro do novo portal
    public float cooldownTime = 1.5f; // Tempo de espera para evitar múltiplos teleportes rápidos

    private HashSet<Transform> cooldownObjects = new HashSet<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (destinationPad == null) return; // Se não tem destino, não faz nada
        if (cooldownObjects.Contains(other.transform)) return; // Se o objeto está em cooldown, ignora

        // Coloca o objeto em cooldown
        cooldownObjects.Add(other.transform);

        // Calcula a nova posição (levemente afastada para evitar bug)
        Vector3 newPosition = destinationPad.position + (destinationPad.forward * teleportOffset) + new Vector3(0, teleportHeightOffset, 0);

        // Teleporta o objeto
        other.transform.position = newPosition;
        Debug.Log($"{other.name} teleportado para {destinationPad.name}");

        // Inicia cooldown para esse objeto
        StartCoroutine(RemoveCooldown(other.transform));
    }

    private IEnumerator RemoveCooldown(Transform obj)
    {
        yield return new WaitForSeconds(cooldownTime);
        cooldownObjects.Remove(obj); // Remove do cooldown após o tempo
    }
}
