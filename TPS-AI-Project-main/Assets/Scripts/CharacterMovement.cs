using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    CharacterStats stats;
    // [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float rotationSpeed = 720f;

    // [Header("Jump & Gravity")]
    public float gravity = 10;
    public float normalJumpHeight = 1.5f;
    public float highJumpHeight = 1.5f;
    public float doubleTapTime = 0.3f;
    private PlayerAnimationEvents playerAnimEvents;

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 velocity;
    public bool isGrounded;
    private Animator anim;

    private float lastSpacebarPressTime;
    private bool waitingForSecondPress;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<CharacterStats>();
        playerAnimEvents = GetComponentInChildren<PlayerAnimationEvents>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            anim.SetBool("IsJumping", false);
        }

        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        // Calculate magnitude BEFORE normalizing
        float inputMagnitude = Mathf.Clamp01(inputDirection.magnitude);

        // Determine if sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Determine speed multiplier for animation (0.5 for walk, 1.0 for sprint)
        float speedMultiplier = isSprinting ? 1f : 0.5f;
        anim.SetFloat("Speed", inputMagnitude * speedMultiplier);

        // Attack 1
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }
        // Attack 2
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("Attack2");
        }

        // Movement and rotation
        if (inputMagnitude >= 0.1f)
        {
            // Calculate movement direction relative to camera
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Project camera directions onto horizontal plane
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Calculate desired movement direction
            Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

            // Rotate character to face movement direction
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move character - use the correct speed based on sprint state
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }

        // Jump handling
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // FIRST JUMP - small, no transition
                velocity.y = Mathf.Sqrt(normalJumpHeight * -2f * gravity);
                Debug.Log("Normal Jump - No Transition");

                // ✅ Play jump sound
                if (playerAnimEvents != null)
                    playerAnimEvents.PlayJumpSound();

                // Allow one high jump
                waitingForSecondPress = true;
            }
            else if (waitingForSecondPress)
            {
                // SECOND JUMP - higher, with animation transition
                velocity.y = Mathf.Sqrt(highJumpHeight * -2f * 0.5f * gravity);
                anim.SetTrigger("Jump");
                Debug.Log("HIGH JUMP - With Transition");

                // ✅ Play jump sound again for double jump
                if (playerAnimEvents != null)
                    playerAnimEvents.PlayJumpSound();

                // Disable further jumps until grounded
                waitingForSecondPress = false;
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void DoAttack()
    {
        transform.Find("Collider").GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(HideCollider());
    }

    IEnumerator HideCollider()
    {
        yield return new WaitForSeconds(0.1f);
        transform.Find("Collider").GetComponent<BoxCollider>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Health"))
        {
            //Debug.Log("Health Increased!");
            GetComponent<CharacterStats>().ChangeHealth(20);
            Instantiate(LevelManager.instance.Particle[1],other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Coin"))
        {
            LevelManager.instance.coins++;
            Debug.Log("1 Coin Collected! Total Coins: " + LevelManager.instance.coins);
            Instantiate(LevelManager.instance.Particle[0], other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            LevelManager.instance.UpdateCoinUI();
        }

    }
}