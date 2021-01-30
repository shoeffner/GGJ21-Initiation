using System;
using UnityEngine;
using Mirror;

public class RuneManager : NetworkBehaviour
{
    [SyncVar]
    bool active = false;

    public event Action<RuneManager> OnRuneActivation;

    void OnTriggerEnter(Collider collider)
    {
        if (!isServer || active || collider.gameObject.tag != "Player")
        {
            return;
        }
        SetActive();
    }

    void SetActive()
    {
        OnRuneActivation?.Invoke(this);
        active = true;
        RpcActivateLight();
    }

    [ClientRpc]
    public void RpcActivateLight()
    {
        GetComponentInChildren<Light>().enabled = true;
    }

}
