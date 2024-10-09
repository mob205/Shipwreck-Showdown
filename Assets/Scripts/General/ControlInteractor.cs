using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class ControlInteractor : NetworkBehaviour
{
    public static Dictionary<uint, ControlInteractor> Interactors { get; private set; }
    [field: SerializeField] public GameObject BoundControllable { get; private set; }

    public PlayerController BoundController { get; private set; }

    public bool IsCurrentlyControlled { get; set; }

    public static UnityEvent<ControlInteractor> OnPossessed;

    private void Awake()
    {
        OnPossessed = new UnityEvent<ControlInteractor>();
    }
    public override void OnStartClient()
    {
        // inefficient because it calls once on each interactor but i dont have time to fix it
        Interactors = new Dictionary<uint, ControlInteractor>();
        var interactors = FindObjectsOfType<ControlInteractor>();
        foreach (var interactor in interactors)
        {
            Interactors.Add(interactor.netId, interactor);
        }
    }

    [Server]
    public void Possess(PlayerController controller)
    {
        BoundController = controller;
        OnPossessed?.Invoke(this);
    }

    [Server]
    public void Unpossess()
    {
        BoundController = null;
    }
}
