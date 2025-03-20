using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace TCC_Game.Appliances{

// }
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


//Ver isso depois, pede pra implementar método abstrato, mas não tem utilidade pra nós agora
    public override void PickUpItems()
{
    // Aqui você pode deixar o corpo vazio, ou se necessário, apenas um comentário
}

public override void DropItems()
{
    // Aqui você também pode deixar o corpo vazio, ou um comentário
}
}
