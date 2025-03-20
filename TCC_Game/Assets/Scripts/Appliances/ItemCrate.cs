using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCrate : Interactable
{
    [SerializeField] private Recursos itemPrefab;

    protected override void Awake()
    {
        base.Awake();
    }

    public override bool TryToDropIntoSlot(IPickable pickableToDrop)
    {
            if (CurrentPickable != null) return false;
            
            CurrentPickable = pickableToDrop;
            CurrentPickable.gameObject.transform.SetParent(Slot);
            pickableToDrop.gameObject.transform.SetPositionAndRotation(Slot.position, Quaternion.identity);
            return true;
    }

     public override IPickable TryToPickUpFromSlot(IPickable playerHoldPickable)
     {
            if (CurrentPickable == null)
            {
                return Instantiate(itemPrefab, Slot.transform.position, Quaternion.identity);
            }

            var output = CurrentPickable;
            var interactable = CurrentPickable as Interactable;
            // interactable?.ToggleHighlightOff();
            CurrentPickable = null;
            return output;
     }
     public override void PickUpItems(){}

     public override void DropItems(){}
}
