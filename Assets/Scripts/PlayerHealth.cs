using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour
{
	public int maxHealth = 100;
	// network synchronized variable
	[SyncVar(hook="OnHealthChanged")]
	public int currentHealth;

	public Slider healthSlider;
	public Image damageImage;
	public float flashSpeed = 5f;
	public Color flashColor = new Color(1.0f, 0.0f, 0.0f, 0.2f);

	private bool isDead = false;
	private bool damaged = false;
	private NetworkStartPosition[] spawnPoints;

	void Start()
	{
		if(!isLocalPlayer)
			return;

		currentHealth = maxHealth;

		spawnPoints = FindObjectsOfType<NetworkStartPosition>();
	}

	void Update()
	{
		if(damaged)
		{
			damageImage.color = flashColor;
		}
		else
		{
			damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;
	}

	public void TakeDamage(int amount)
	{	
		if(isLocalPlayer)
		{
			damaged = true;
		}
		if(!isServer)
		{
			return;
		}

		currentHealth -= amount;

		if(currentHealth <= 0)
		{
			OnDeath();
		}
	}

	void OnDeath()
	{
		isDead = true;
		//called on the server, run on the clients
		RpcRespawn();
		currentHealth = maxHealth;
	}

	void OnHealthChanged(int health)
	{
		healthSlider.value = health;
	}

	[ClientRpc]
	void RpcRespawn()
	{
		if(isLocalPlayer)
		{
			// Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
		}
	}
}
