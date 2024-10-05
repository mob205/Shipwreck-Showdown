using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private IControllable _curControllable;
    private IControllable _defaultControllable;

    private void Awake()
    {
        _defaultControllable = GetComponent<IControllable>();
        _curControllable = _defaultControllable;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("TEST");
        _curControllable.Move(context.ReadValue<Vector2>());
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            _curControllable.Fire();
        }
    }
    public void OnPossess(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("possession stuff");
        }
    }
}


