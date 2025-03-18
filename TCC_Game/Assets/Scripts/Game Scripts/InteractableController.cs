using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private Transform playerPivot;
    private readonly HashSet<Interactable> _interactables = new HashSet<Interactable>();

    private void Awake()
    {
        if (playerPivot == null)
            playerPivot = transform;
    }

    [CanBeNull]
    public Interactable CurrentInteractable => TryGetClosest();

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.gameObject.GetComponent<Interactable>();
        if(!interactable)
            return;

        if (_interactables.Contains(interactable))
        {
            return;
        }
        _interactables.Add(interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if(interactable)
            _interactables.Remove(interactable);
    }

    public void Remove(Interactable interactable)
    {
        _interactables.Remove(interactable);
    }

    private Interactable TryGetClosest()
    {
        var minDistance = float.MaxValue;
        Interactable closest = null;
        foreach (var interactable in _interactables)
        {
            var distance = Vector3.Distance(playerPivot.position, 
                interactable.gameObject.transform.position);

            if(distance > minDistance)
                continue;

            minDistance = distance;
            closest = interactable;
        }

        return closest;
    }
}
