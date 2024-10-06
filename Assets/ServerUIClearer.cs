using UnityEngine;
using Mirror;

public class ServerUIClearer : MonoBehaviour
{
    private void Awake()
    {
        if(NetworkServer.active)
        {
            var uis = FindObjectsOfType<Canvas>();
            foreach(var ui in uis)
            {
                Destroy(ui.gameObject);
            }
        }
    }
}
