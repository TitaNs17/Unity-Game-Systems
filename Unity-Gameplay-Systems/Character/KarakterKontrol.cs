using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class KarakterKontrol : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpHeight = 2f;

    [Header("Yerçekimi ve Ground Check (ESKİ SİSTEM GERİ GELDİ)")]
    public float gravity = -20f; 
    public float fallMultiplier = 1.5f; 
    public Transform groundCheck;
    public float groundDistance = 0.4f; 
    public LayerMask groundMask;

    [Header("Mouse Ayarları")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    public Transform playerBody;

    [Header("Head Bob Ayarları")]
    public float headBobAmplitude = 0.05f; 
    public float headBobFrequency = 10f;   

    [Header("Landing ve Jump Bob Ayarları")]
    public float jumpBobMultiplier = 0.02f; 
    public float landingBobMultiplier = 0.1f;  
    public float smoothSpeed = 8f;             

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;

    private Vector3 cameraStartPos;
    private float headBobTimer = 0f;
    private bool wasGroundedLastFrame = true;
    private float landingOffset = 0f; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (playerBody == null)
            playerBody = transform;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        cameraStartPos = cameraTransform.localPosition;
    }

    void Update()
    {
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 

            if (!wasGroundedLastFrame)
            {
               
                landingOffset = -landingBobMultiplier;
                headBobTimer = 0f;
            }
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);

        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (velocity.y < 0 && !isGrounded)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

       
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

       
        HandleHeadBob(move.magnitude > 0.1f ? currentSpeed : 0f);

        wasGroundedLastFrame = isGrounded;
    }

    private void HandleHeadBob(float speed)
    {
        float targetY = cameraStartPos.y;

        landingOffset = Mathf.Lerp(landingOffset, 0f, Time.deltaTime * smoothSpeed);

        if (speed > 0f && isGrounded)
        {
            headBobTimer += Time.deltaTime * headBobFrequency * (currentSpeed / walkSpeed);
            targetY += Mathf.Sin(headBobTimer) * headBobAmplitude;
        }
        else if (!isGrounded)
        {
            float clampedVelocity = Mathf.Clamp(velocity.y, -15f, 15f);
            targetY += clampedVelocity * jumpBobMultiplier; 
        }
        else
        {
            headBobTimer = 0f;
        }

        targetY += landingOffset;

        
        targetY = Mathf.Max(targetY, cameraStartPos.y - landingBobMultiplier - 0.05f);

        Vector3 targetPosition = new Vector3(cameraStartPos.x, targetY, cameraStartPos.z);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}