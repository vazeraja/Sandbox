using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "InputData/Input Reader")]
public class InputProvider : ScriptableObject, IInputProvider, GameInput.IGameplayActions {

    private Vector2 movementDirection;
    private bool isCrouching;
    public event UnityAction<float> onJump;
    public event UnityAction<float> onDash;
    public event UnityAction onHat;

    private GameInput GameInput;

    private void OnEnable()
    {
        GameInput ??= new GameInput();
        GameInput.Gameplay.SetCallbacks(this);
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    public InputState GetState()
    {
        return new InputState {
            movementDirection = movementDirection,
            isCrouching = isCrouching
        };
    }

    public static implicit operator InputState(InputProvider provider) => provider.GetState();

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) {
            isCrouching = context.ReadValue<float>() > 0.5f;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) {
            onJump?.Invoke(context.ReadValue<float>());
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            onDash?.Invoke(context.ReadValue<float>());
    }
    
    public void OnHat(InputAction.CallbackContext context)
    {
        if (onHat != null && context.phase == InputActionPhase.Started)
            onHat?.Invoke();
    }
    
    
    public void EnableInput()
    {
        GameInput.Gameplay.Enable();
    }
    
    public void DisableInput()
    {
        GameInput.Gameplay.Disable();
    }

}