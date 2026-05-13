using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementNew : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private Transform capsuleModel;
    private InputSystem_Actions controls; // Die generierte Klasse aus Schritt 1
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isSprinting;
    private float sprintMultiplier=2f;
    private bool isCrouching;
    public float normalHeight = 2.0f;  
    public float crouchHeight = 1.32f; // 0.66 von 2.0
    public float crouchSpeed = 10f;    // Für ein sanftes Schrumpfen
    public float normalHeadY = 0.73f; // Höhe des Kopfes im Stehen
    public float crouchHeadY = 0.1f;
    private Transform headObject;

    private float jumpHeight = 2f;
    
    private float speed = 5f;
    private float gravity = 80f;
    private float groundedPressForce = -2f;

    private void Awake()
    {
        headObject = transform.Find("Head");
        controller = GetComponent<CharacterController>();
        controls = new InputSystem_Actions();

        // Das Event abonnieren: Wenn sich der Stick/WASD bewegt, Wert speichern
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // Wenn losgelassen wird, Input zurücksetzen
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Sprinten (In image_1e04ad.png ganz unten zu sehen)
        controls.Player.Sprint.performed += ctx => isSprinting = true;
        controls.Player.Sprint.canceled += ctx => isSprinting = false;

        //jump
        controls.Player.Jump.performed += ctx => OnJump();

        controls.Player.Crouch.performed += ctx => isCrouching = true;
        controls.Player.Crouch.canceled += ctx => isCrouching = false;

        
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    void Update()
    {
        ApplyMovement();
        ApplyGravity();
        HandleCrouch();
    }

    private void ApplyMovement()
{

    
    // 1. Die Richtungsvektoren der Kamera holen
    Vector3 forward = Camera.main.transform.forward;
    Vector3 right = Camera.main.transform.right;

    // 2. Die Y-Komponente nullen, damit man nicht "fliegt" oder im Boden versinkt
    forward.y = 0f;
    right.y = 0f;

    // 3. Die Vektoren wieder normalisieren (wichtig für die gleichmäßige Geschwindigkeit)
    forward.Normalize();
    right.Normalize();

    // 4. Den gewünschten Bewegungsvektor basierend auf dem Input berechnen
    // moveInput.y ist W/S, moveInput.x ist A/D
    Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;

    // 5. Sprint-Check (wie wir es vorhin besprochen haben)
    float currentSpeed = speed;
    if (isCrouching) 
    {
        currentSpeed *= 0.5f; // Halbe Geschwindigkeit beim Ducken
    }
    if (isSprinting && moveInput.y > 0) 
    {
        currentSpeed *= sprintMultiplier;
    }

    // 6. Den CharacterController bewegen
    controller.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
}

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = groundedPressForce;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        // Bewegung auf der Y-Achse ausführen
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnJump()
    {
        // Wir dürfen nur springen, wenn wir den Boden berühren
        if (controller.isGrounded)
        {
            // Die Formel setzt die vertikale Geschwindigkeit nach oben
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }
    private void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : normalHeight;
        float targetHeadY = isCrouching ? crouchHeadY : normalHeadY;

    

        // 1. CharacterController Höhe glätten
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSpeed);
        
        // 2. Center IMMER synchron zur aktuellen Höhe halten
        // Das sorgt dafür, dass die physikalische Kapsel unten am Player-Pivot (Y=0) klebt.
        controller.center = new Vector3(0, controller.height / 2f, 0);

        // 3. Grafik-Kapsel (Capsule Model) anpassen
        if (capsuleModel != null)
        {
            // Skalierung: Unity-Kapsel ist 2m hoch bei Scale 1.
            Vector3 modelScale = capsuleModel.localScale;
            modelScale.y = controller.height / 2f; 
            capsuleModel.localScale = modelScale;

            // Position: Die Grafik muss ebenfalls mit ihrer Mitte auf der halben Höhe sitzen
            Vector3 modelPos = capsuleModel.localPosition;
            modelPos.y = controller.center.y; 
            capsuleModel.localPosition = modelPos; 
        }

        // 4. Kopf/Kamera-Position glätten
        if (headObject != null)
        {
            Vector3 newHeadPos = headObject.localPosition;
            newHeadPos.y = Mathf.Lerp(newHeadPos.y, targetHeadY, Time.deltaTime * crouchSpeed);
            headObject.localPosition = newHeadPos;
        } 
    }   
}