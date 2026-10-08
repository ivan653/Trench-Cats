using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public static UIEvents Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public static event Action<float> OnFundsUpdated;
    public static event Action<float> OnCommandPointsUpdated;
    

    public static void TriggerFundsUpdated(float value)
    {
        OnFundsUpdated?.Invoke(value);
    }

    public static void TriggerCommandResourceUpdated(float value)
    {
        OnCommandPointsUpdated?.Invoke(value);
    }
}