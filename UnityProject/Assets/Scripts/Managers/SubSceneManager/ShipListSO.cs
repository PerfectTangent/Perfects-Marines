using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;


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

		// Check that we can actually load the scene.
		return Ships.Where(scene => SceneUtility.GetBuildIndexByScenePath(scene) > -1).PickRandom();
	}
}
