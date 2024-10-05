using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ControlInteractor : NetworkBehaviour
{
    public static Dictionary<uint, ControlInteractor> Interactors { get; private set; }
    [field: SerializeField] public GameObject BoundControllable { get; private set; }

    public bool IsCurrentlyControlled { get; set; }

    public override void OnStartClient()
    {
        if (Interactors == null)
        {
            Interactors = new Dictionary<uint, ControlInteractor>();
            var interactors = FindObjectsOfType<ControlInteractor>();
            foreach (var interactor in interactors)
            {
                Interactors.Add(interactor.netId, interactor);
            }
        }
    }
}
