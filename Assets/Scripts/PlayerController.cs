using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalController;

    [SerializeField] private float _interactRange;
    [SerializeField] private LayerMask _interactable;
    [SerializeField] private LayerMask _cannonballPickup;

    public bool IsCaptain { get; private set; }
    public IControllable CurrentControllable { get; private set; }

    private IControllable _defaultControllable;
    private ControlInteractor _usedInteractor;

    [SyncVar] private bool _hasCannonball;

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

    [Server]
    public void Unpossess()
    {
        if (!_usedInteractor) { return; }
        _usedInteractor.IsCurrentlyControlled = false;

        if (_usedInteractor.BoundControllable.GetComponent<IControllable>().RequiresAuthority)
        {
            _usedInteractor.BoundControllable.GetComponent<IControllable>().OnReleaseControl();
        }

        //if(_usedInteractor.BoundControllable.TryGetComponent(out NetworkTransformReliable networkTransform))
        //{
        //    networkTransform.interpolatePosition = false;
        //}

        _usedInteractor.Unpossess();
        _usedInteractor = null;
        IsCaptain = false;
    }
    [Command]
    public void CmdTryPossess(NetworkConnectionToClient conn = null)
    {
        if (_usedInteractor) // Already possessed, so unpossess
        {
            Unpossess();
            TargetResetControllable(conn);
            return;
        }

        // Check if trying to pick up a cannon
        if(!_hasCannonball && Physics2D.OverlapCircle(transform.position, _interactRange, _cannonballPickup))
        {
            _hasCannonball = true;
            return;
        }

        // Not currently possessing something
        var nearby = Physics2D.OverlapCircleAll(transform.position, _interactRange, _interactable);
        var closest = GetClosestCollider(nearby);

        if (closest != null && closest.TryGetComponent(out ControlInteractor interactor))
        {
            if (_hasCannonball && interactor.BoundControllable.TryGetComponent(out CannonMovement cannon))
            {
                _hasCannonball = false;
                cannon.LoadCannon();
                return;
            }

            if (interactor.IsCurrentlyControlled == false)
            {
                if (interactor.BoundControllable.GetComponent<IControllable>().RequiresAuthority)
                {
                    interactor.BoundControllable.GetComponent<NetworkIdentity>().RemoveClientAuthority();
                    interactor.BoundControllable.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
                }

                if(interactor.BoundControllable.GetComponent<ShipMovement>())
                {
                    IsCaptain = true;
                }

                //if (_usedInteractor.BoundControllable.TryGetComponent(out NetworkTransformReliable networkTransform))
                //{
                //    networkTransform.interpolatePosition = true;
                //}

                interactor.IsCurrentlyControlled = true;
                _usedInteractor = interactor;

                _usedInteractor.Possess(this);
                

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