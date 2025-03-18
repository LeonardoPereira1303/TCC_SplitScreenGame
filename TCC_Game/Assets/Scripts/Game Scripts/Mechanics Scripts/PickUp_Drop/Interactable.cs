using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour, IPickable
{
    [SerializeField] protected Transform slot;

    protected IPickable CurrentPickable { get; set; }
    protected PlayerController LastInteracting;

    public Transform Slot => slot;

    protected virtual void Awake()
    {

    }

    void CheckSlot()
    {
        if (Slot == null)
            return;

        foreach(Transform child in Slot)
        {
            CurrentPickable = child.GetComponent<IPickable>();
            if (CurrentPickable != null)
                return;
        }
    }

    public virtual void Interact(PlayerController playerController)
    {
        LastInteracting = playerController;
    }

    public abstract bool TryToDropIntoSlot(IPickable pickableToDrop);
    [CanBeNull] public abstract IPickable TryToPickUpFromSlot(IPickable playerHoldPickable);

    public abstract void PickUpItems();

    public abstract void DropItems();
}
