using UnityEngine;

public class BombController : MonoBehaviour
{
	// used for checking the collision with sidewise ground element
	public LayerMask whatIsBlocker;
	public LayerMask whatIsPlayer;
	public Transform blockCheck;
	public Transform edgeCheck;

	public GameObject explosionPrefab;
	
	[Range(0.0f, 6.0f)]
	public float moveSpeed = 3.0f;
	public int direction = 1;

	private Rigidbody2D rigidbody;
	private bool isBlocked;
	private bool isAtEdge;
	private bool isNearPlayer;
	private bool canMove = false;
	private int velX = 0;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();

		// randomize bomb direction
		float rand = Random.Range(-1, 1);
		direction = (rand < 0) ? -1 : 1;

		var localScale = transform.localScale;
		localScale.x *= direction;
		transform.localScale = localScale;

		Invoke("Move", 1.0f);
		Invoke("Xplode", 5.0f);
	}

	void Update()
	{
		if(canMove)
		{
			velX = 1;

			isNearPlayer = Physics2D.Linecast(transform.position, blockCheck.position, whatIsPlayer);
			isBlocked = Physics2D.Linecast(transform.position, blockCheck.position, whatIsBlocker);
			// check if bomb is at the edge of platform
			isAtEdge = !(Physics2D.Linecast(blockCheck.position, edgeCheck.position, whatIsBlocker));

			if(isBlocked || isAtEdge)
			{
				var localScale = transform.localScale;
				localScale.x *= - 1;
				transform.localScale = localScale;

				direction *= -1;
			}
			else if(isNearPlayer)
			{
				velX = 0;
			}
		}
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		rigidbody.velocity = new Vector2(velX * moveSpeed * direction, rigidbody.velocity.y);
	}

	void Move()
	{
		canMove = true;
	}

	void Xplode()
	{
		Destroy(gameObject);
		var explosion = (GameObject)Instantiate(explosionPrefab, transform.position, transform.rotation);
		Destroy(explosion, 1.0f);
	}
}
