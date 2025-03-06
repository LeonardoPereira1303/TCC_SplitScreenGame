using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    private CharacterController controller;

    private Vector2 movementInput = Vector2.zero;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Dash")]
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dashSpeed = 20.0f;
    [SerializeField] private float dashDuration = 2.0f;
    private bool isDashing = false;

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
        if (context.started && !isDashing)
        {
            StartCoroutine(Dash());                 
        }
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

        //Evita que se movimente e use dash ao mesmo tempo
        if (!isDashing)
        {
            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
            controller.Move(move * playerSpeed * Time.deltaTime);
        }

    }

}
