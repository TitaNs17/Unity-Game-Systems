using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class KarakterKontrol : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public float fallMultiplier = 1.5f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Look")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    public Transform playerBody;

    [Header("Head Bob")]
    public float headBobAmplitude = 0.05f;
    public float headBobFrequency = 10f;
    public float jumpBobMultiplier = 0.02f;
    public float landingBobMultiplier = 0.1f;
    public float smoothSpeed = 8f;

    private CharacterController controller;
    private float xRotation;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private Vector3 cameraStartPos;
    private float headBobTimer;
    private bool wasGroundedLastFrame = true;
    private float landingOffset;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerBody == null) playerBody = transform;
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (cameraTransform != null)
            cameraStartPos = cameraTransform.localPosition;
    }

    private void Update()
    {
        if (controller == null || groundCheck == null || cameraTransform == null)
            return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            if (!wasGroundedLastFrame)
            {
                landingOffset = -landingBobMultiplier;
                headBobTimer = 0f;
            }
        }

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        var move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * (velocity.y < 0f && !isGrounded ? fallMultiplier : 1f) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        HandleHeadBob(move.magnitude > 0.1f ? currentSpeed : 0f);
        wasGroundedLastFrame = isGrounded;
    }

    private void HandleHeadBob(float speed)
    {
        var targetY = cameraStartPos.y;
        landingOffset = Mathf.Lerp(landingOffset, 0f, Time.deltaTime * smoothSpeed);

        if (speed > 0f && isGrounded)
        {
            headBobTimer += Time.deltaTime * headBobFrequency * (currentSpeed / Mathf.Max(0.01f, walkSpeed));
            targetY += Mathf.Sin(headBobTimer) * headBobAmplitude;
        }
        else if (!isGrounded)
        {
            targetY += Mathf.Clamp(velocity.y, -15f, 15f) * jumpBobMultiplier;
        }
        else
        {
            headBobTimer = 0f;
        }

        targetY += landingOffset;
        targetY = Mathf.Max(targetY, cameraStartPos.y - landingBobMultiplier - 0.05f);
        var targetPosition = new Vector3(cameraStartPos.x, targetY, cameraStartPos.z);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
