using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    MyControls inputAction;
    public InputAction moveAction;
    public InputAction jumpAction;

    private bool isGrounded = true;
    private Rigidbody rb;
    private Vector3 forceDirection = Vector3.zero;
    private float maxSpeed = 5f;
    [SerializeField]
    private float jump = 5f;
    [SerializeField]
    private float walkSpeed = 5f;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float distanceToGround = 5f;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        inputAction = new MyControls();
    }

    private void OnEnable()
    {
        inputAction.PlayerControls.Jump.started += OnJump;
        moveAction = inputAction.PlayerControls.Movement;
        inputAction.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        inputAction.PlayerControls.Jump.started -= OnJump;
        inputAction.PlayerControls.Disable();
    }

    private void FixedUpdate()
    {
        
        forceDirection += moveAction.ReadValue<Vector2>().x * GetCameraRight(playerCamera);
        forceDirection += moveAction.ReadValue<Vector2>().y * GetCameraForward(playerCamera);

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        distanceToGround = GetComponent<Collider>().bounds.extents.y;
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, distanceToGround);

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if(horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

        LookAt();
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (moveAction.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        } 
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Debug.Log("Movement Action performed with: " + context.control.name);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Debug.Log("Jump Action performed with: " + context.control.name);
        if(isGrounded)
        {
            forceDirection += Vector3.up * jump;
        }
    }
}
