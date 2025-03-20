using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android.LowLevel;
using static UnityEngine.UIElements.UxmlAttributeDescription;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private IPickable pickDropScript;
    private CharacterController controller;

    private Vector2 movementInput = Vector2.zero;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Player Paramters")]
    [SerializeField] private Transform slot;

    [Header("Movement Parameters")]
    [SerializeField] private float playerSpeed;

    [Header("Dash Parameters")]
    [SerializeField] private float dashSpeed = 20.0f;
    [SerializeField] private float dashDuration = 2.0f;
    [SerializeField] private float dashCooldown = 3.0f;
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;

    [SerializeField]
    private InteractableController _interactableController;
    private IPickable _currentPickable;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
            lastDashTime = Time.time;
        }
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            PerformPickUp();
        }
    }

    private void PerformPickUp()
    {
        Interactable interactable = _interactableController.CurrentInteractable;

        if(interactable == null)
            return;

        if(_currentPickable == null)
        {
            _currentPickable = interactable as IPickable;

            if(_currentPickable != null)
            {
                _currentPickable.PickUpItems();
                _interactableController.Remove(_currentPickable as Interactable);
                _currentPickable.gameObject.transform.SetPositionAndRotation(
                    slot.transform.position, Quaternion.identity);
                _currentPickable.gameObject.transform.SetParent(slot);
                return;
            }

            _currentPickable = interactable?.TryToPickUpFromSlot(_currentPickable);
            if (_currentPickable != null)
            {
                //Anima��o + Sound
            }

            _currentPickable?.gameObject.transform.SetPositionAndRotation(slot.position,
                Quaternion.identity);
            _currentPickable?.gameObject.transform.SetParent(slot);
            return;
        }

        if(interactable == null || interactable is IPickable)
        {
            _currentPickable.DropItems();
            _currentPickable = null;
            return;
        }

        bool dropSuccess = interactable.TryToDropIntoSlot(_currentPickable);
        if(!dropSuccess) return;

        _currentPickable = null;
    }

    private IEnumerator Dash()
    {
        isDashing = true;

        float startTime = Time.time;
        Vector3 dashDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        while (Time.time < startTime + dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            Debug.Log("Dash ativado");
            yield return null;
        }

        isDashing = false;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (!isDashing)
        {
            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
            transform.position += move * playerSpeed * Time.deltaTime;
        }

    }
}
