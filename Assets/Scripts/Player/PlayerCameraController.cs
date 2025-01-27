using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 85.0f;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerInputActions;

    [SerializeField] private Camera playerCamera;

    private InputAction lookAction;
    private float verticalRotation;

    private void Awake()
    {
        var playerMap = playerInputActions.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
    }

    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
    }

    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        Vector2 input = lookAction.ReadValue<Vector2>();

        // Horizontal rotation (player body)
        transform.Rotate(0, input.x * mouseSensitivity, 0);

        // Vertical rotation (camera)
        verticalRotation -= input.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
