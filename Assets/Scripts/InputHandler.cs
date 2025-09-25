using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController CharacterController;

    private InputAction _moveAction, _lookAction, _jumpAction, _interactAction, _dropAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _interactAction = InputSystem.actions.FindAction("Attack");
        _dropAction = InputSystem.actions.FindAction("Drop");
        _jumpAction.performed += OnJumpPerformed;
        _interactAction.performed += OnInteractPerformed;
        _dropAction.performed += OnDropPerformed;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementVector = _moveAction.ReadValue<Vector2>();
        CharacterController.Move(movementVector);

        Vector2 lookVector = _lookAction.ReadValue<Vector2>();
        CharacterController.Rotate(lookVector);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        CharacterController.Jump();
    }
 private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        CharacterController.Grab();
    }
 private void OnDropPerformed(InputAction.CallbackContext context)
    {
        CharacterController.Drop();
    }
}
