using UnityEngine;

public class PlayerMovementHandler : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float verticalLookLimit = 80f;

    [SerializeField] private CharacterController characterController;
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Vypočítá směr pohybu na základě natočení hráče (transform)
        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        // Normalizuje vektor, aby se zabránilo rychlejšímu pohybu po diagonále
        characterController.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertikální rotace (nahoru/dolů)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontální rotace (doleva/doprava)
        transform.Rotate(Vector3.up * mouseX);
    }
}