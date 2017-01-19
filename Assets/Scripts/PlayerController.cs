﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	[Range(1.0f, 6.0f)]
	public float moveSpeed = 3f;
	public float jumpForce = 400f;

	public Transform groundCheckStart;
	public Transform groundCheckFinish;
	public LayerMask whatIsGround;
	public GameObject bombPrefab;
	public Transform bombSpawner;

	private Rigidbody2D rigidbody;
	private bool isGrounded = false;
	private float vx;
	private float vy;


	void Update()
	{
		if(!isLocalPlayer)
		{
			return;
		}

		vx = Input.GetAxisRaw("Horizontal");
		vy = rigidbody.velocity.y;

		isGrounded = Physics2D.Linecast(groundCheckStart.position, groundCheckFinish.position, whatIsGround);

		if(Input.GetButtonDown("Jump") && isGrounded)
		{
			DoJump();
		}
		else if(Input.GetButtonUp("Jump") && vy > 0f)
		{
			vy = 0;
		}

		if(Input.GetKeyDown(KeyCode.Z))
		{
			CmdLayBomb();
		}
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		if(!isLocalPlayer)
			return;

		rigidbody.velocity = new Vector2(vx * moveSpeed, vy);
	}

	public override void OnStartLocalPlayer()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		if(rigidbody == null)
		{
			Debug.LogError("Component Rigidbody2D is missing in gameObject!");
		}

		// color local player blue
		GetComponent<SpriteRenderer>().color = Color.blue;
	}

	void DoJump()
	{
		vy = 0;
		rigidbody.AddForce(new Vector2(0, jumpForce));
	}

	// this method is a network command
	// i.e. it is called on client but is run on the server
	[Command]
	void CmdLayBomb()
	{
		var bomb = (GameObject)Instantiate(bombPrefab, bombSpawner.position, bombSpawner.rotation);

		NetworkServer.Spawn(bomb);
	}
}
