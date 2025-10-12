using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Components")]
    private Rigidbody2D rb2D;
    private Camera mainCamera;
    
    [Header("Input")]
    private Vector2 moveInput;
    private Vector2 mouseLookInput;
    private Vector2 stickLookInput;
    private Vector2 lookDirection = Vector2.zero;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.gravityScale = 0f; // Garantir que não há gravidade
        mainCamera = Camera.main;

        // Garantir que existe um Rigidbody2D
        if (rb2D == null)
        {
            Debug.LogError("PlayerMovement: Rigidbody2D component not found!");
        }

        // Garantir que existe uma camera principal
        if (mainCamera == null)
        {
            Debug.LogError("PlayerMovement: Main Camera not found!");
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void Update()
    {
        HandleLook();
    }
    // Input System Callbacks
    public void OnMove(Vector2 value)
    {
        moveInput = value;
    }
    public void OnLook(Vector2 value)
    {
        mouseLookInput = value;
    }
    public void OnAim(Vector2 value)
    {
        stickLookInput = value;
    }
    #region Movement Logic

    private void HandleMovement()
    {
        if (rb2D == null) return;

        Vector2 movement = moveInput * moveSpeed;
        rb2D.linearVelocity = movement;
    }
    
    private void HandleLook()
    {
        if (mainCamera == null) return;

        // Se o analógico direito estiver sendo usado, prioriza ele
        if (stickLookInput.sqrMagnitude > 0.04f) // 0.2*0.2
        {
            lookDirection = stickLookInput.normalized;
        }
        // Caso contrário, usa o mouse
        else if (mouseLookInput.x > 0f && mouseLookInput.y > 0f)
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseLookInput.x, mouseLookInput.y, mainCamera.nearClipPlane));
            mouseWorldPosition.z = 0f;
            lookDirection = (mouseWorldPosition - transform.position).normalized;
        }

        if (lookDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
    }
    
    #endregion
}
