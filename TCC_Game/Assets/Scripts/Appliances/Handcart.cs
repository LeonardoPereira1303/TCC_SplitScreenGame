using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Handcart : Interactable, IPickable
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Transform soup;

    private const int MaxNumberIngredients = 4;
        
    private Material _soupMaterial;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private readonly List<Recursos> _recursos = new List<Recursos>(MaxNumberIngredients);

    public bool IsClean { get; private set; } = true;
    public List<Recursos> Recursos => _recursos;
    public bool IsEmpty() =>_recursos.Count == 0;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();         
         
        Setup();
    }

    private void Setup()
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        _soupMaterial = soup.gameObject.GetComponent<MeshRenderer>()?.material;
    }

    public bool AddIngredients(List<Recursos> recursos)
        {
            // check for soup (3 equal cooked ingredients, mushroom, onion or tomato)
            if (!IsEmpty()) return false;
            _recursos.AddRange(recursos);
            
            foreach (var recurso in _recursos)
            {
                recurso.transform.SetParent(Slot);
                recurso.transform.SetPositionAndRotation(Slot.transform.position, Quaternion.identity);
            }
            // UpdateIconsUI();
            
            if (CheckSoupIngredients(recursos))
            {
                EnableSoup(recursos[0]);
            }
            // not a soup
            return true;
        }

        public void RemoveAllIngredients()
        {
            DisableSoup();
            _recursos.Clear();
            // UpdateIconsUI();
        }

        /// <summary>
        /// Check for exactly 3 equals ingredients, being onions, tomatoes or mushrooms.
        /// </summary>
        public static bool CheckSoupIngredients(IReadOnlyList<Recursos> recursos)
        {
            if (recursos == null || recursos.Count != 3)
            {
                return false;
            }

            if (recursos[0].Type != RecursosType.Parafusos &&
                recursos[0].Type != RecursosType.Pregos &&
                recursos[0].Type != RecursosType.Baterias)
            {
                Debug.Log("[Plate] Soup only must contain onion, tomato or mushroom");
                return false;
            }
            
            if (recursos[0].Type != recursos[1].Type ||
                recursos[1].Type != recursos[2].Type ||
                recursos[0].Type != recursos[2].Type)
            {
                Debug.Log("[Plate] Soup with mixed ingredients! You must thrash it away! What a waste!");
                return false;
            }
            
            return true;
        }
        
        private void EnableSoup(in Recursos recursoSample)
        {
            soup.gameObject.SetActive(true); 
            // _soupMaterial.color = recursoSample.BaseColor;
        }

        private void DisableSoup()
        {
            soup.gameObject.SetActive(false);
        }

        // [ContextMenu("SetClean")]
        // public void SetClean()
        // {
        //     meshRenderer.material = cleanMaterial;
        //     IsClean = true;
        // }
        
        // [ContextMenu("SetDirty")]
        // public void SetDirty()
        // {
        //     meshRenderer.material = dirtyMaterial;
        //     IsClean = false;
        //     DisableSoup();
        // }
        
        public void Pick()
        {
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
        }

        public void Drop()
        {
            gameObject.transform.SetParent(null);
            _rigidbody.isKinematic = false;
            _collider.enabled = true;
        }
        
        public override bool TryToDropIntoSlot(IPickable pickableToDrop)
        {
            if (pickableToDrop == null) return false;
            
            //we can drop soup from plate to plate AND from plate to CookingPot (and vice-versa)
            switch (pickableToDrop)
            {
                case CookingPot cookingPot:
                    if (cookingPot.IsCookFinished &&
                        cookingPot.IsBurned == false &&
                        CheckSoupIngredients(cookingPot.Ingredients))
                    {
                        AddIngredients(cookingPot.Ingredients);
                        cookingPot.EmptyPan();
                        return false;
                    }
                    break;
                case Recursos recursos:
                    Debug.Log("[Plate] Trying to dropping Ingredient into Plate! Not implemented");
                    break;
                case Handcart handcart:
                    //Debug.Log("[Plate] Trying to drop something from a plate into other plate! We basically swap contents");
                    if (this.IsEmpty() == false || this.IsClean == false) return false;
                    this.AddIngredients(handcart.Recursos);
                    handcart.RemoveAllIngredients();
                    return false;
                default:
                    Debug.LogWarning("[Plate] Drop not recognized", this);
                    break;
            }
            return false;
        }

        public override IPickable TryToPickUpFromSlot(IPickable playerHoldPickable)
        {
            // We can pickup Ingredients from plates with other plates (effectively swapping content) or from Pans
            
            if (playerHoldPickable == null) return null;
            
            switch (playerHoldPickable)
            {
                // we just pick the soup ingredients, not the CookingPot itself
                case CookingPot cookingPot:
                    Debug.Log("[Plate] Trying to pick from a plate with a CookingPot", this);
                    break;
                case Recursos recursos:
                    //TODO: we can pickup some ingredients into plate, not all of them.
                    break;
                // swap plate ingredients
                case Handcart handcart:
                    if (handcart.IsEmpty())
                    {
                        if (this.IsEmpty()) return null;
                        handcart.AddIngredients(this._recursos);
                    }
                    break;
            }
            return null;
        }

    public override void PickUpItems(){

    }

    public override void DropItems(){
        
    }
}
