using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController characterController;
	
	public float MovementSpeed = 10f;
	
    void Start()
    {
        characterController = GetComponent<CharacterController>();
	
    }

    
    public void Move(Vector2 movementVector)
    {
        Vector3 move = new Vector3(Input.GetAxis("horizontal"), 0, Input.GetAxis("Vertical"));
		
		characterController.Move(move * Time.deltaTime * MovementSpeed);
    }
	
	
}