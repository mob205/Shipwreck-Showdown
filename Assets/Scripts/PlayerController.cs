using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _interactRange;
    [SerializeField] private LayerMask _interactable;

    private IControllable _curControllable;
    private IControllable _defaultControllable;
    private ControlInteractor _usedInteractor;

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

    [Command]
    public void CmdTryPossess(NetworkConnectionToClient conn = null)
    {
        if(_usedInteractor) // Already possessed, so unpossess
        {
            _usedInteractor.IsCurrentlyControlled = false;
            _usedInteractor.netIdentity.RemoveClientAuthority();
            _usedInteractor = null;
            return;
        }

        // Not currently possessing something

        var nearby = Physics2D.OverlapCircleAll(transform.position, _interactRange, _interactable);
        var closest = GetClosestCollider(nearby);

        if(closest != null && closest.TryGetComponent(out ControlInteractor interactor))
        {
            if(interactor.IsCurrentlyControlled == false)
            {
                interactor.netIdentity.AssignClientAuthority(conn);
                interactor.IsCurrentlyControlled = true;
                _usedInteractor = interactor;
            }
        }
    }

    [TargetRpc]
    public void TargetSwitchControllable(NetworkConnectionToClient conn, uint controlInteractId)
    {
        _curControllable = ControlInteractor.Interactors[controlInteractId].BoundControllable;
    }

    [TargetRpc]
    public void TargetResetControllable(NetworkConnectionToClient conn)
    {
        _curControllable = _defaultControllable;
    }

    public Collider2D GetClosestCollider(Collider2D[] colliders)
    {
        Collider2D best = null;
        float closestDist = Mathf.Infinity;

        foreach(Collider2D collider in colliders)
        {
            var dist = (collider.transform.position - transform.position).sqrMagnitude;
            if(dist < closestDist)
            {
                best = collider;
                closestDist = dist;
            }
        }

        return best;
    }
}


