using UnityEngine;
using UnityEngine.Networking;

public class BombController : NetworkBehaviour
{
	// used for checking the collision with sidewise ground element
	public LayerMask whatIsBlocker;
	public LayerMask whatIsPlayer;
	public Transform blockCheck;
	public Transform edgeCheck;
	// this object will be activated at the time of explosion to inflict damage on other objects
	public GameObject damage;

	public GameObject explosionPrefab;
	
	[Range(0.0f, 6.0f)]
	public float moveSpeed = 3.0f;
	[SyncVar]
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

	// this will also be called if the object collides with other exploding bombs
	public void Xplode()
	{
		gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
		damage.SetActive(true);
		Destroy(gameObject, 0.5f);
		var explosion = (GameObject)Instantiate(explosionPrefab, transform.position, transform.rotation);
		Destroy(explosion, 1.0f);
	}
}
