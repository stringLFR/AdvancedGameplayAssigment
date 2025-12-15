using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class CRPGCamera : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    private InputSystem_Actions _actions;

    [SerializeField]
    private LayerMask _layerMask;

    [Header("Camera settings")]
    [SerializeField]
    private float zMaxDistanceFromHolder = 10f, zDistanceSpeed = 0.5f, holderMinDistanceGround = 0.5f, holderMaxDistanceGround = 10.5f, 
        heightSpeed = 0.5f, rotationSpeedSide = 0.5f, rotationSpeedUpDown = 0.5f, moveSpeed = 1f;

    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private float currentZCamDist = 10f;
    bool isLooking = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _actions.Enable();
        _actions.Player.Look.performed += OnLook;
        _actions.Player.Look.canceled += OnLook;

        _actions.Player.Move.performed += OnMove;
        _actions.Player.Move.canceled += OnMove;

        _actions.UI.RightClick.performed += OnRightClick;
        _actions.UI.RightClick.canceled += OnRightClick;

        _actions.UI.ScrollWheel.performed += ScrollWhell;
        _actions.UI.ScrollWheel.canceled += ScrollWhell;
    }

    private void OnDisable()
    {
        _actions.Player.Look.performed -= OnLook;
        _actions.Player.Look.canceled -= OnLook;

        _actions.Player.Move.performed -= OnMove;
        _actions.Player.Move.canceled -= OnMove;

        _actions.UI.RightClick.performed -= OnRightClick;
        _actions.UI.RightClick.canceled -= OnRightClick;

        _actions.UI.ScrollWheel.performed -= ScrollWhell;
        _actions.UI.ScrollWheel.canceled -= ScrollWhell;

        _actions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        RotateHolder();

        MoveHolder();

        UpdateCameraDistance();

        UpdateHeight();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    private void UpdateCameraDistance()
    {
        Vector3 cameraDist = new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, -currentZCamDist);

        _camera.transform.localPosition = Vector3.LerpUnclamped(_camera.transform.localPosition, cameraDist, Time.deltaTime * zDistanceSpeed);
    }
    private void UpdateHeight()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out RaycastHit hit, 1000f, ~_layerMask))
        {
            Vector3 newHeight = transform.position;
            float hitDist = Vector3.Distance(transform.position, hit.point);
            float dist = 0;

            if (hitDist > holderMaxDistanceGround) //DOWN
            {
                dist = hitDist - holderMaxDistanceGround;

                newHeight.y = Mathf.LerpUnclamped(newHeight.y, transform.position.y - dist, Time.deltaTime * heightSpeed);

            }
            else if (hitDist < holderMinDistanceGround) //UP
            {
                dist = holderMinDistanceGround - hitDist;

                newHeight.y = Mathf.LerpUnclamped(newHeight.y, transform.position.y + dist, Time.deltaTime * heightSpeed);
            }

            transform.position = newHeight;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    private void RotateHolder()
    {
        if (isLooking == true)
        {
            transform.Rotate(new Vector3(0, lookDirection.x * Time.deltaTime * rotationSpeedSide, 0), Space.World);
            transform.Rotate(new Vector3(-lookDirection.y * Time.deltaTime * rotationSpeedUpDown, 0, 0), Space.Self);
        }

    }

    private void MoveHolder()
    {
        Vector3 newDirection = new Vector3(moveDirection.x, 0, moveDirection.y);

        float oldYPos = transform.position.y;

        transform.Translate(newDirection * Time.deltaTime * moveSpeed, Space.Self);

        transform.position = new Vector3(transform.position.x, oldYPos, transform.position.z);//TO MAKE SURE THAT Y IS NOT CHANGED BY TRANSLATE!

    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnMove(InputAction.CallbackContext context) => moveDirection = context.ReadValue<Vector2>();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnLook(InputAction.CallbackContext context) => lookDirection = context.ReadValue<Vector2>();

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            isLooking = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            isLooking = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void ScrollWhell(InputAction.CallbackContext context)
    {
        currentZCamDist -= context.ReadValue<Vector2>().y;

        currentZCamDist = Mathf.Clamp(currentZCamDist, 0, zMaxDistanceFromHolder);
    }
}
