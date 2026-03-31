using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerMovementPhysics movement;
    public static InputManager instance;
    public InputSystem_Actions actions { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Enable();

        movement.initializeClientInput(actions.Player);

        instance = this;
    }

    private void OnDestroy()
    {
        actions.Disable();
    }
}
