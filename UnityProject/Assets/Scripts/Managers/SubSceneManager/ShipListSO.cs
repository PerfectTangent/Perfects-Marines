using System.Collections;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipListSO", menuName = "ScriptableObjects/ShipList", order = 1)]
public class ShipListSO : ScriptableObject
{
	[Header("Provide the exact name of the scene in the fields below:")]
	[InfoBox("Remember to also add your scene to " +
	         "the build settings list",EInfoBoxType.Normal)]
	[Scene]
	public List<string> Ships = new List<string>();

	public string GetRandomShip()
	{
		var mapConfigPath = Path.Combine(Application.streamingAssetsPath, "ships.json");

		if (File.Exists(mapConfigPath))
		{
			var maps = JsonUtility.FromJson<MapList>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath,
				"ships.json")));

			return maps.GetRandomMap();
		}

		return Ships[Random.Range(0, Ships.Count)];
	}
}
