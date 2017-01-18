using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
	// used for checking the collision with sidewise ground element
	public LayerMask whatIsBlocker;
	public Transform blockCheck;
	
	[Range(0.0f, 6.0f)]
	public float moveSpeed = 3.0f;
	public int direction = 1;

	private Rigidbody2D rigidbody;
	private bool isBlocked;


	void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		isBlocked = Physics2D.Linecast(transform.position, blockCheck.position, whatIsBlocker);
		if(isBlocked)
		{
			var localScale = transform.localScale;
			localScale.x *= - 1;
			transform.localScale = localScale;

			direction *= -1;
		}

		rigidbody.velocity = new Vector2(moveSpeed * direction, rigidbody.velocity.y);
	}
}
