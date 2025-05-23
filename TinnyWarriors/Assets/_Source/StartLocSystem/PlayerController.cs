using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    private CharacterController _characterController;
    private Quaternion targetrotaion;
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if(input != Vector3.zero)
        {
            targetrotaion = Quaternion.LookRotation(input);
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetrotaion.eulerAngles.y,rotationSpeed * Time.deltaTime);
        }
        Vector3 motion = input * speed;
        motion += Vector3.up * -8;
        _characterController.Move(motion * Time.deltaTime);
    }
}
