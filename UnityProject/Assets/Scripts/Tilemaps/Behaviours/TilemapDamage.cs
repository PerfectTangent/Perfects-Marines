﻿using UnityEngine;

public class TilemapDamage : MonoBehaviour, IFireExposable
{
	private TileChangeManager tileChangeManager;
	private MetaDataLayer metaDataLayer;
	private MetaTileMap metaTileMap;
	public Layer Layer { get; private set; }

	private Matrix matrix;

	private void Awake()
	{
		tileChangeManager = transform.GetComponentInParent<TileChangeManager>();
		metaDataLayer = transform.GetComponentInParent<MetaDataLayer>();
		metaTileMap = transform.GetComponentInParent<MetaTileMap>();

		Layer = GetComponent<Layer>();
		matrix = GetComponentInParent<Matrix>();

		tileChangeManager.OnFloorOrPlatingRemoved.RemoveAllListeners();
		tileChangeManager.OnFloorOrPlatingRemoved.AddListener(cellPos =>
		{ //Poke items when both floor and plating are gone
			//As they might want to change matrix
			if (!metaTileMap.HasTile(cellPos, LayerType.Floors, true)
			    && !metaTileMap.HasTile(cellPos, LayerType.Base, true)
			    && metaTileMap.HasTile(cellPos, LayerType.Objects, true)
			)
			{
				foreach (var customNetTransform in matrix.Get<CustomNetTransform>(cellPos, true))
				{
					customNetTransform.CheckMatrixSwitch();
				}
			}
		});
	}

	public void OnCollisionEnter2D(Collision2D coll)
	{
		if (!CustomNetworkManager.Instance._isServer)
		{
			return;
		}
		ContactPoint2D firstContact = coll.GetContact(0);
		DetermineAction(coll.gameObject, coll.relativeVelocity.normalized, firstContact.point);
	}

	private void DetermineAction(GameObject objectColliding, Vector2 forceDirection, Vector3 hitPos)
	{
		BulletBehaviour bulletBehaviour = objectColliding.transform.parent.GetComponent<BulletBehaviour>();
		if (bulletBehaviour != null)
		{
			DoBulletDamage(bulletBehaviour, forceDirection, hitPos);
		}
	}

	private void DoBulletDamage(BulletBehaviour bullet, Vector3 forceDir, Vector3 hitPos)
	{
		forceDir.z = 0;
		Vector3 bulletHitTarget = hitPos + (forceDir * 0.2f);
		Vector3Int cellPos = metaTileMap.WorldToCell(Vector3Int.RoundToInt(bulletHitTarget));

		var basicTile = metaTileMap.GetTile(cellPos, Layer.LayerType) as BasicTile;

		if (basicTile == null) return;

		if (bullet.isMiningBullet)
		{
			if (Layer.LayerType == LayerType.Walls)
			{
				if (Validations.IsMineableAt(bulletHitTarget, metaTileMap))
				{
					SoundManager.PlayNetworkedAtPos("BreakStone", bulletHitTarget);
					Spawn.ServerPrefab(basicTile.SpawnOnDeconstruct, bulletHitTarget,
						count: basicTile.SpawnAmountOnDeconstruct);
					tileChangeManager.RemoveTile(cellPos, LayerType.Walls);
					return;
				}
			}
		}

		MetaDataNode data = metaDataLayer.Get(cellPos);
		basicTile.AddDamage(bullet.damage, AttackType.Bullet, cellPos, hitPos, data, tileChangeManager);
	}

	public void DoMeleeDamage(Vector2 worldPos, GameObject originator, int dmgAmt)
	{
		Vector3Int cellPos = metaTileMap.WorldToCell(worldPos);
		DealDamageAt(dmgAmt, AttackType.Melee, cellPos, worldPos);
	}

	public float ApplyDamage(Vector3Int cellPos, float dmgAmt, Vector3Int worldPos, AttackType attackType = AttackType.Melee)
	{
		Vector3Int worldToCellcellPos = metaTileMap.WorldToCell(worldPos);
		return DealDamageAt(dmgAmt, attackType, worldToCellcellPos, worldPos);
	}

	public float Integrity(Vector3Int pos)
	{
		var layerTile = metaTileMap.GetTile(pos, Layer.LayerType) as BasicTile;
		if (layerTile == null)
		{
			return 0;
		}

		return Mathf.Clamp(layerTile.MaxHealth - metaDataLayer.Get(pos).Damage, 0, float.MaxValue);
	}

	public void RepairWindow(Vector3Int cellPos)
	{
		var data = metaDataLayer.Get(cellPos);
		tileChangeManager.RemoveTile(cellPos, LayerType.Effects);
		data.Damage = 0;
	}

	public void OnExposed(FireExposure exposure)
	{
		var cellPos = exposure.ExposedLocalPosition;
		DealDamageAt(exposure.StandardDamage(), AttackType.Fire, cellPos, exposure.ExposedWorldPosition);
	}

	private float DealDamageAt(float damage, AttackType attackType, Vector3Int cellPos, Vector3 worldPosition)
	{
		var basicTile = metaTileMap.GetTile(cellPos, Layer.LayerType) as BasicTile;

		if (basicTile == null) return 0;

		MetaDataNode data = metaDataLayer.Get(cellPos);
		return basicTile.AddDamage(damage, attackType, cellPos, worldPosition, data, tileChangeManager);
	}
}