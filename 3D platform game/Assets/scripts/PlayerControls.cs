using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Vector3 playerVelocity;
    [SerializeField] bool groundedPlayer;
    [SerializeField] float playerSpeed;
    [SerializeField] float gravityValue;
    [SerializeField] GameObject activeChar;
    [SerializeField] float moveHorizontal;
    [SerializeField] float moveVertical;
    [SerializeField] float speed = 4;
    [SerializeField] float rotateSpeed = 2;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] bool isJumping;
    
    // Double jump variables
    [SerializeField] int maxJumps = 2;          // Allows for double jump (2 jumps total)
    [SerializeField] int jumpsRemaining;        // Tracks how many jumps are left
    [SerializeField] bool canDoubleJump = true; // Enable/disable double jump feature

    void Start()
    {
        playerSpeed = 4;
        gravityValue = -20;
        jumpsRemaining = maxJumps; // Initialize jumps
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        
        // Reset jumps when grounded
        if (groundedPlayer && playerVelocity.y <= 0)
        {
            playerVelocity.y = 0f;
            jumpsRemaining = maxJumps; // Reset jumps when touching ground
        }

        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = speed * Input.GetAxis("Vertical");
        controller.SimpleMove(forward * curSpeed);

        // Jump input handling with double jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            Jump();
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.001f;
            if(isJumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Standard Run");
            }
        }
        else
        {
            this.gameObject.GetComponent<CharacterController>().minMoveDistance = 0.901f;
            if (isJumping == false)
            {
                activeChar.GetComponent<Animator>().Play("Idle");
            }
        }
    }
    
    void Jump()
    {
        isJumping = true;
        jumpsRemaining--; // Decrease available jumps
        
        // Play jump animation
        activeChar.GetComponent<Animator>().Play("Jump");
        
        // Calculate jump velocity based on jump height and gravity
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue * 2);
        
        StartCoroutine(ResetJump());
    }
    
    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.8f);
        isJumping = false;
    }
    
    // Optional: Public methods to control double jump behavior
    public void EnableDoubleJump(bool enable)
    {
        canDoubleJump = enable;
        maxJumps = enable ? 2 : 1;
    }
    
    public void SetMaxJumps(int maxJumps)
    {
        this.maxJumps = Mathf.Max(1, maxJumps); // Ensure at least 1 jump
        jumpsRemaining = Mathf.Min(jumpsRemaining, this.maxJumps);
    }
}