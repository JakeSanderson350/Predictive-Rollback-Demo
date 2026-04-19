using UnityEngine;
using UnityEngine.Events;

public class ClientEventManager : MonoBehaviour
{
    public static ClientEventManager instance;

    public UnityEvent<bool> InterpolationEnabled;
    public UnityEvent<bool> PredictionEnabled;
    public UnityEvent<bool> ReconciliationEnabled;

    public UnityEvent<bool> ServerShowerEnabled;
    public UnityEvent<bool> ClientShowerEnabled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    public void UpdateInterpolationEnabled(bool value)
    {
        InterpolationEnabled.Invoke(value);
    }

    public void UpdatePredictionEnabled(bool value)
    {

        PredictionEnabled.Invoke(value);
    }

    public void UpdateReconciliationEnabled(bool value)
    {
        ReconciliationEnabled.Invoke(value);
    }

    public void UpdateServerShowerEnabled(bool value)
    {
        ServerShowerEnabled.Invoke(value);
    }

    public void UpdateClientShowerEnabled(bool value)
    {
        ClientShowerEnabled.Invoke(value);
    }


}
