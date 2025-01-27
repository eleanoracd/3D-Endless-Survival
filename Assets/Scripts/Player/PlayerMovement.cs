using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerInputActions;

    private Vector3 movementVelocity;
    private CharacterController characterController;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        var playerMap = playerInputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
        runAction = playerMap.FindAction("Run");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
        characterController.Move(movementVelocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        float speedMultiplier = runAction.ReadValue<float>() > 0 ? runMultiplier : 1f;

        Vector3 movement = new Vector3(input.x, 0, input.y) * walkSpeed * speedMultiplier;
        movementVelocity.x = movement.x;
        movementVelocity.z = movement.z;
        movementVelocity = transform.TransformDirection(movementVelocity);
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            movementVelocity.y = -0.5f;

            if (jumpAction.triggered)
                movementVelocity.y = jumpForce;
        }
        else
        {
            movementVelocity.y -= gravity * Time.deltaTime;
        }
    }
}
