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

	void Awake()
	{
		currentHealth = maxHealth;
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
		currentHealth = maxHealth;
	}

	void OnHealthChanged(int health)
	{
		damaged = true;
		healthSlider.value = health;
	}
}
