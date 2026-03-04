using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
	
	public float MovementSpeed = 10f, RotationSpeed = 5f;
	
	private float _roationY;
	
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
	
    }

    
    public void Move(Vector2 movementVector)
    {
        Vector3 move = transform.forward * movementVector.y + transform.right * movementVector.x;
		move = move * MovementSpeed * Time.deltaTime;
		_characterController.Move(move);
    }
	
	public void Rotate(Vector2 rotationVector)
	{
		_rotationY += rotationVector.x * RotationSpeed * Time.detlaTime;
		transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
	}
}
