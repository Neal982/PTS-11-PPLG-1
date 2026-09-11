using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] public float Speed = 5f;
    private Vector2 MoveDirection;


    void OnMove(InputValue value)
    {
        MoveDirection = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 Direction = new Vector3(MoveDirection.x, MoveDirection.y, 0);
        transform.position += Direction * Speed * Time.deltaTime;
    }
    private void PlayerDie()
    {
        Debug.Log("Player Die");
        Destroy(gameObject);
    }
}