using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInput : DroneInputBase, GameInput.IPlayerActions
{
    private GameInput gameInput;

    // cached values updated via callbacks
    private Vector2 cachedLook = Vector2.zero;
    private Vector2 cachedMove = Vector2.zero;
    private float cachedLock = 0f;
    private float cachedDash = 0f;
    private float cachedUp = 0f;
    private float cachedDown = 0f;

    private void Awake()
    {
        gameInput = new GameInput();
    }

    private void OnEnable()
    {
        if (gameInput == null) gameInput = new GameInput();
        gameInput.Player.SetCallbacks(this);
        gameInput.Enable();
        Debug.Log("DroneInput OnEnable: GameInput enabled and callbacks set");
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.Player.SetCallbacks(null);
            gameInput.Disable();
            Debug.Log("DroneInput OnDisable: GameInput disabled and callbacks cleared");
        }
    }

    private void OnDestroy()
    {
        if (gameInput != null)
        {
            try
            {
                gameInput.Player.SetCallbacks(null);
                gameInput.Disable();
            }
            catch { }
            gameInput.Dispose();
            gameInput = null;
            Debug.Log("DroneInput OnDestroy: GameInput disposed");
        }
    }

    // DroneInputBase overrides - return cached values updated by callbacks
    public override Vector2 Look() => cachedLook;
    public override Vector2 Move() => cachedMove;
    public override float Lock() => cachedLock;
    public override float DashInput() => cachedDash;
    public override float Up() => cachedUp;
    public override float Down() => cachedDown;

    // IPlayerActions implementations - update caches on start/performed/canceled, log for debug
    public void OnMove(InputAction.CallbackContext context)
    {
        cachedMove = context.ReadValue<Vector2>();
        Debug.Log($"DroneInput OnMove: phase={context.phase} value={cachedMove}");
        if (context.canceled) cachedMove = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cachedLook = context.ReadValue<Vector2>();
        // Mouse delta may be frequent; keep log lightweight
        if (cachedLook.sqrMagnitude > 0.00001f)
            Debug.Log($"DroneInput OnLook: phase={context.phase} value={cachedLook}");
        if (context.canceled) cachedLook = Vector2.zero;
    }

    public void OnLock(InputAction.CallbackContext context)
    {
        cachedLock = context.ReadValue<float>();
        Debug.Log($"DroneInput OnLock: phase={context.phase} value={cachedLock}");
        if (context.canceled) cachedLock = 0f;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        cachedDash = context.ReadValue<float>();
        Debug.Log($"DroneInput OnDash: phase={context.phase} value={cachedDash}");
        if (context.canceled) cachedDash = 0f;
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        cachedUp = context.ReadValue<float>();
        Debug.Log($"DroneInput OnUp: phase={context.phase} value={cachedUp}");
        if (context.canceled) cachedUp = 0f;
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        cachedDown = context.ReadValue<float>();
        Debug.Log($"DroneInput OnDown: phase={context.phase} value={cachedDown}");
        if (context.canceled) cachedDown = 0f;
    }
}