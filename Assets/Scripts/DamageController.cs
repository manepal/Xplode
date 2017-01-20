using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour 
{
	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		GameObject obj = other.gameObject;

		if(obj.tag == "Player")
		{
			obj.GetComponent<PlayerHealth>().TakeDamage(10);
		}
		// explode bombs nearby
		if((obj.tag == "Explosive") && (transform.parent != obj))
		{
			obj.GetComponent<BombController>().Xplode();
		}
	}
}
