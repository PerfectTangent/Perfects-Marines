using UnityEngine;
using System.Collections;
using Items;

/// <summary>
/// Used to tell the player the rough damage of a weapon when they examine it.
/// </summary>
[RequireComponent(typeof(ItemAttributesV2))]
public class WeaponStrengthMessage : MonoBehaviour, IExaminable
{

	private ItemAttributesV2 itemAttributes;

	private float hitDamage;

	private void Awake()
	{
		itemAttributes = GetComponent<ItemAttributesV2>();
	}

	private void Start()
	{
		hitDamage = itemAttributes.ServerHitDamage;
	}

	private string StrengthText
	{
	get
		{
		switch(hitDamage)
			{
				default: //if damage is 10 and below.
					return "weak";
				case float damage when damage.IsBetween(11, 25):
					return "normal";
				case float damage when damage.IsBetween(25, 35):
					return "strong";
				case float damage when damage.IsBetween(35, 45):
					return "very strong";
				case float damage when damage.IsBetween(45, Mathf.Infinity):
					return "inhumanely strong";
			}
		}
	}

	public string Examine(Vector3 worldPos = default)
	{
		return $"This weapon looks {StrengthText}.";
	}
	
}
