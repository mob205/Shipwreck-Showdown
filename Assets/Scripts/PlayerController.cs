using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalController;

    [SerializeField] private float _interactRange;
    [SerializeField] private LayerMask _interactable;

    public IControllable CurrentControllable { get; private set; }

    private IControllable _defaultControllable;
    private ControlInteractor _usedInteractor;

    private void Awake()
    {
        _defaultControllable = GetComponent<IControllable>();
        CurrentControllable = _defaultControllable;
    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        GetComponent<PlayerInput>().enabled = true;
        LocalController = this;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        CurrentControllable.Move(context.ReadValue<Vector2>());
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CurrentControllable.Fire();
        }
    }
    public void OnPossess(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CmdTryPossess();
        }
    }

    [Command]
    public void CmdTryPossess(NetworkConnectionToClient conn = null)
    {
        if (_usedInteractor) // Already possessed, so unpossess
        {
            _usedInteractor.IsCurrentlyControlled = false;
            _usedInteractor.BoundControllable.GetComponent<NetworkIdentity>().RemoveClientAuthority();
            _usedInteractor = null;
            TargetResetControllable(conn);
            return;
        }

        // Not currently possessing something

        var nearby = Physics2D.OverlapCircleAll(transform.position, _interactRange, _interactable);
        var closest = GetClosestCollider(nearby);

        if (closest != null && closest.TryGetComponent(out ControlInteractor interactor))
        {
            if (interactor.IsCurrentlyControlled == false)
            {
                interactor.BoundControllable.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
                interactor.IsCurrentlyControlled = true;
                _usedInteractor = interactor;
                TargetSwitchControllable(conn, interactor.netId);
            }
        }
    }

    [TargetRpc]
    public void TargetSwitchControllable(NetworkConnectionToClient conn, uint controlInteractId)
    {
        CurrentControllable.OnReleaseControl();
        CurrentControllable = ControlInteractor.Interactors[controlInteractId].BoundControllable.GetComponent<IControllable>();
    }

    [TargetRpc]
    public void TargetResetControllable(NetworkConnectionToClient conn)
    {
        CurrentControllable.OnReleaseControl();
        CurrentControllable = _defaultControllable;
    }

    public Collider2D GetClosestCollider(Collider2D[] colliders)
    {
        Collider2D best = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            var dist = (collider.transform.position - transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                best = collider;
                closestDist = dist;
            }
        }

        return best;
    }
}