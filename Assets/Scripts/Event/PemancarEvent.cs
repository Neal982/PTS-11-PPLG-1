using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PemancarEvent : MonoBehaviour
{
    public static event Action OnTekanSpasi;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Pemancar: spasi ditekan, kirim event.");
            OnTekanSpasi?.Invoke();
        }
    }
}
