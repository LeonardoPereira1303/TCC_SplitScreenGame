using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterTop : Interactable
{
    public override void DropItems()
    {
    }

    public override void PickUpItems()
    {
    }

    public override bool TryToDropIntoSlot(IPickable pickableToDrop)
        {
            if (CurrentPickable == null) return TryDropIfNotOccupied(pickableToDrop);

            return CurrentPickable switch
            {
                CookingPot cookingPot => cookingPot.TryToDropIntoSlot(pickableToDrop),
                Recursos recursos => recursos.TryToDropIntoSlot(pickableToDrop),
                Handcart handcart => handcart.TryToDropIntoSlot(pickableToDrop),
                _ => false
            };
        }

        public override IPickable TryToPickUpFromSlot(IPickable playerHoldPickable)
        {
            if (CurrentPickable == null) return null;

            var output = CurrentPickable;
            var interactable = CurrentPickable as Interactable;
            // interactable?.ToggleHighlightOff();
            CurrentPickable = null;
            return output;
        }

        private bool TryDropIfNotOccupied(IPickable pickable)
        {
            if (CurrentPickable != null) return false;
            
            CurrentPickable = pickable;
            CurrentPickable.gameObject.transform.SetParent(Slot);
            CurrentPickable.gameObject.transform.SetPositionAndRotation(Slot.position, Quaternion.identity);
            return true;
        }
}
