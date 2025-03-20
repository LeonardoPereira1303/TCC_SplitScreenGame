using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BatteryCharger : Interactable, IPickable
{
    // Timers
    private float _totalChargerTime;
    private float _currentChargerTime;

    // We hold a timer, based on the ingredients cookTime
    private float _currentBurnTime;

    private Coroutine _chargingCoroutine;
    private Coroutine _burnCoroutine;

    private const int MaxNumberRecursos = 3;

    // Flags
    private bool _onTable;
    private bool _isCharging;
    private bool _inBurnProcess;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private CanvasGroup _canvasGroup;

    public bool IsChargeFinished { get; private set; }
    public bool IsBurned { get; private set; }
    public bool IsEmpty() => Recursos.Count == 0;
    public List<Recursos> Recursos { get; } = new List<Recursos>(MaxNumberRecursos);

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
                if (recursos.status != RecursosStatus.Processed)
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
                    if (handcart.IsEmpty() == false && Handcart.CheckHandcartResources(handcart.Recursos))
                    {
                        // Drop soup back into CookingPot
                        TryAddIngredients(handcart.Recursos);
                        handcart.RemoveAllIngredients();
                        return false;
                    }
                }

                if (IsChargeFinished && !IsBurned)
                {
                    if (IsBurned) return false;
                    if (handcart.IsClean == false) return false;

                    bool isSoup = Handcart.CheckHandcartResources(this.Recursos);

                    if (!isSoup) return false;

                    handcart.AddIngredients(this.Recursos);
                    EmptyCharger();
                    return false;
                }
                break;

            default:
                Debug.LogWarning("[ChoppingBoard] Refuse everything else");
                return false;
        }
        return false;
    }

    private bool TryDrop(IPickable pickable)
    {
        if (Recursos.Count >= MaxNumberRecursos) return false;

        var ingredient = pickable as Recursos;
        if (ingredient == null)
        {
            Debug.LogWarning("[CookingPot] Can only drop ingredients into CookingPot", this);
            return false;
        }

        Recursos.Add(ingredient);

        _totalChargerTime += ingredient.CookTime;

        // hide ingredient mesh
        //ingredient.SetMeshRendererEnabled(false);
        ingredient.gameObject.transform.SetParent(Slot);

        // reset burnProcess, if any
        _currentBurnTime = 0f;
        if (_inBurnProcess && _burnCoroutine != null)
        {
            TryStopBurn();
            // resume cooking, because we are already burning
            _chargingCoroutine = StartCoroutine(Cook());
            return true;
        }

        // (re)start cooking
        if (_onTable && !_isCharging)
        {
            _chargingCoroutine = StartCoroutine(Cook());
        }

        //UpdateIngredientsUI();
        return true;
    }

    public override IPickable TryToPickUpFromSlot(IPickable playerHoldPickable)
    {
        // we can only pick a soup when it's ready and Player has a Plate in hands, otherwise refuse
        if (!IsChargeFinished || IsBurned) return null;

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
        EmptyCharger();
        return null;
    }

    public void EmptyCharger()
    {
        if (_chargingCoroutine != null) StopCoroutine(_chargingCoroutine);
        if (_burnCoroutine != null) StopCoroutine(_burnCoroutine);

        //slider.gameObject.SetActive(false);
        Recursos.Clear();

        _currentChargerTime = 0f;
        _currentBurnTime = 0f;
        _totalChargerTime = 0f;
        IsBurned = false;
        IsChargeFinished = false;
        _isCharging = false;
        //warningPopup.transform.localScale = Vector3.zero;
        //greenCheckPopup.transform.localScale = Vector3.zero;

        //UpdateIngredientsUI();

        // deactivate FX's
        //whiteSmoke.gameObject.SetActive(false);
        //blackSmoke.gameObject.SetActive(false);
    }

    private bool TryAddIngredients(List<Recursos> resources)
    {
        if (!IsEmpty()) return false;
        if (Handcart.CheckHandcartResources(resources) == false) return false;
        Recursos.AddRange(resources);

        foreach (var ingredient in Recursos)
        {
            ingredient.transform.SetParent(Slot);
            ingredient.transform.SetPositionAndRotation(Slot.transform.position, Quaternion.identity);
        }

        IsChargeFinished = true;
        //UpdateIngredientsUI();
        return true;
    }

    public override void PickUpItems(){}

    public override void DropItems(){}

    public void DroppedIntoHob()
    {
        _onTable = true;
        //warningPopup.enabled = false;

        if (Recursos.Count == 0 || IsBurned) return;

        // after cook, we burn
        if (IsChargeFinished)
        {
            _burnCoroutine = StartCoroutine(Burn());
            return;
        }

        // or restart cooking
        _chargingCoroutine = StartCoroutine(Cook());
    }

    private void TryStopBurn()
    {
        if (_burnCoroutine == null) return;

        StopCoroutine(_burnCoroutine);

        //warningPopup.enabled = false;
        //greenCheckPopup.enabled = false;
        _inBurnProcess = false;

        //UpdateIngredientsUI();
    }

    private void TryStopCook()
    {
        if (_chargingCoroutine == null) return;

        StopCoroutine(_chargingCoroutine);
        _isCharging = false;
    }

    public void RemovedFromHob()
    {
        _onTable = false;
        //warningPopup.enabled = false; ;

        if (_inBurnProcess)
        {
            TryStopBurn();
            return;
        }

        if (_isCharging)
        {
            TryStopCook();
        }
    }

    private IEnumerator Cook()
    {
        _isCharging = true;
        //slider.gameObject.SetActive(true);

        while (_currentChargerTime < _totalChargerTime)
        {
            //slider.value = _currentChargerTime / _totalChargerTime;
            _currentChargerTime += Time.deltaTime;
            yield return null;
        }

        _isCharging = false;

        if (Recursos.Count == MaxNumberRecursos)
        {
            TriggerSuccessfulCook();
            yield break;
        }

        // Debug.Log("[CookingPot] Finish partial cooking");

        _burnCoroutine = StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        float[] timeLine = { 0f, 4f, 6.3f, 9.3f, 12.3f, 13.3f };

        _inBurnProcess = true;
        //slider.gameObject.SetActive(false);

        // GreenCheck
        if (_currentBurnTime <= timeLine[0])
        {
            //AnimateGreenCheck();
        }
        //whiteSmoke.gameObject.SetActive(true);

        while (_currentBurnTime < timeLine[1])
        {
            _currentBurnTime += Time.deltaTime;
            yield return null;
        }

        //warningPopup.enabled = true;

        var internalCount = 0f;

        // pulsating at 2Hz
        while (_currentBurnTime < timeLine[2])
        {
            _currentBurnTime += Time.deltaTime;
            internalCount += Time.deltaTime;
            if (internalCount > 0.5f)
            {
                internalCount = 0f;
                //PulseAndBeep(1.15f);
            }
            yield return null;
        }

        // pulsating at 5Hz
        while (_currentBurnTime < timeLine[3])
        {
            _currentBurnTime += Time.deltaTime;
            internalCount += Time.deltaTime;
            if (internalCount > 0.2f)
            {
                internalCount = 0f;
                //PulseAndBeep(1.25f);
            }
            yield return null;
        }

        //var initialColor = liquidMaterial.color;
        var delta = timeLine[4] - timeLine[3];

        // pulsating at 10Hz
        while (_currentBurnTime < timeLine[4])
        {

            var interpolate = (_currentBurnTime - timeLine[3]) / delta;
            //liquidMaterial.color = Color.Lerp(initialColor, burnLiquid, interpolate);
            _currentBurnTime += Time.deltaTime;
            internalCount += Time.deltaTime;
            if (internalCount > 0.1f)
            {
                internalCount = 0f;
                //PulseAndBeep(1.35f);
            }
            yield return null;
        }
    }

    private void TriggerSuccessfulCook()
    {
        IsChargeFinished = true;
        _currentChargerTime = 0f;
        _burnCoroutine = StartCoroutine(Burn());
    }
}
