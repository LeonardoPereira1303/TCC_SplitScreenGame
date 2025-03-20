using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BatteryCharger : Interactable, IPickable
{
    // Timers
    private float _totalCookTime;
    private float _currentCookTime;

    // We hold a timer, based on the ingredients cookTime
    private float _currentBurnTime;

    private Coroutine _cookCoroutine;
    private Coroutine _burnCoroutine;

    private const int MaxNumberRecursos = 3;

    // Flags
    private bool _onHob;
    private bool _isCooking;
    private bool _inBurnProcess;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private CanvasGroup _canvasGroup;

    public bool IsCookFinished { get; private set; }
    public bool IsBurned { get; private set; }
    public bool IsEmpty() => Recursos.Count == 0;
    public List<Recursos> Recursos { get; } = new List<Recursos>(MaxNumberRecursos);

    // [GameDesign]
    // if there is a mixed soup (e.g. 2x onions 1x tomato) we can't pickup the soup (it's locked),
    // the only option is to trash it
    // we only deliver single ingredient soups

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _canvasGroup = GetComponent<CanvasGroup>();

        Setup();
    }

    private void Setup()
    {
        // Rigidbody is kinematic almost all the time, except when we drop it on the floor
        // re-enabling when picked up.
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        _canvasGroup.alpha = 0f;
    }

    public override bool TryToDropIntoSlot(IPickable pickableToDrop)
    {
        switch (pickableToDrop)
        {
            case Recursos recursos:
                if (recursos.status != Re.Processed)
                {
                    Debug.Log("[CookingPot] Only accept chopped/processed ingredients");
                    return false;
                }
                if (recursos.Type == RecursosType.Parafusos ||
                    recursos.Type == RecursosType.Pregos ||
                    recursos.Type == RecursosType.Baterias)
                {
                    return TryDrop(pickableToDrop);
                }
                Debug.Log("[CookingPot] Only accept Onions, tomatoes or Mushrooms");
                return false;
            case Handcart handcart:

                if (IsEmpty())
                {
                    if (handcart.IsEmpty() == false && Handcart.CheckSoupIngredients(handcart.Recursos))
                    {
                        // Drop soup back into CookingPot
                        TryAddIngredients(handcart.Recursos);
                        handcart.RemoveAllIngredients();
                        return false;
                    }
                }

                if (IsCookFinished && !IsBurned)
                {
                    if (IsBurned) return false;
                    if (handcart.IsClean == false) return false;

                    bool isSoup = Handcart.CheckSoupIngredients(this.Recursos);

                    if (!isSoup) return false;

                    handcart.AddIngredients(this.Recursos);
                    EmptyPan();
                    return false;
                }
                break;

            default:
                Debug.LogWarning("[ChoppingBoard] Refuse everything else");
                return false;
        }
        return false;
    }

    public override IPickable TryToPickUpFromSlot(IPickable playerHoldPickable)
    {
        // we can only pick a soup when it's ready and Player has a Plate in hands, otherwise refuse
        if (!IsCookFinished || IsBurned) return null;

        // we "lock" a soup if there are different ingredients. Player has to trash it away
        if (Recursos[0].Type != Recursos[1].Type ||
            Recursos[1].Type != Recursos[2].Type ||
            Recursos[0].Type != Recursos[2].Type)
        {
            // Debug.Log("[CookingPot] Soup with mixed ingredients! You must thrash it away! What a waste!");
            return null;
        }

        if (!(playerHoldPickable is Handcart handcart)) return null;
        if (!handcart.IsClean || !handcart.IsEmpty()) return null;

        handcart.AddIngredients(Recursos);
        EmptyPan();
        return null;
    }

    private bool TryAddIngredients(List<Recursos> ingredients)
    {
        if (!IsEmpty()) return false;
        if (Handcart.CheckSoupIngredients(ingredients) == false) return false;
        Recursos.AddRange(ingredients);

        foreach (var ingredient in Recursos)
        {
            ingredient.transform.SetParent(Slot);
            ingredient.transform.SetPositionAndRotation(Slot.transform.position, Quaternion.identity);
        }

        SetLiquidLevelAndColor();

        IsCookFinished = true;
        UpdateIngredientsUI();
        return true;
    }
}
