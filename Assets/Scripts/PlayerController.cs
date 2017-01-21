using UnityEngine;
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

	[Range(1, 5)]
	public int bombLayInterval;

	public bool canLayBombs = true;
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

		if(Input.GetKeyDown(KeyCode.LeftControl))
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

		// temporary solution for camera follow
		var camPosition = Camera.main.transform.position;
		camPosition.x = transform.position.x;
		camPosition.x = Mathf.Clamp(camPosition.x, -5.0f, 5.0f);
		Camera.main.transform.position = camPosition;
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
		if(canLayBombs)
		{
			var bomb = (GameObject)Instantiate(bombPrefab, bombSpawner.position, bombSpawner.rotation);
			// randomize bomb direction
			bomb.GetComponent<BombController>().direction = (Random.value < 0.5f) ? -1 : 1;
			NetworkServer.Spawn(bomb);

			canLayBombs = false;
			Invoke("AllowToLayBombs", bombLayInterval);
		}
	}

	void AllowToLayBombs()
	{
		canLayBombs = true;
	}
}
