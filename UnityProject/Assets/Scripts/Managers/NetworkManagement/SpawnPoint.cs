using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Mirror;

namespace Systems.Spawns
{
	public class SpawnPoint : NetworkStartPosition
	{
		[SerializeField, FormerlySerializedAs("Department")]
		private SpawnPointCategory category = default;

		[SerializeField]
		private SpawnPointType type = SpawnPointType.Unlimited;

		[SerializeField]
		[Range(0, 10)]
		[Tooltip("Higher number means higher priority")]
		private int priority = 0;

		private bool used;

		public static IEnumerable<Transform> GetPointsForCategory(SpawnPointCategory category)
		{
			return NetworkManager.startPositions.Select(x => x.transform)
				.Where(x => x.TryGetComponent<SpawnPoint>(out var spawn) && spawn.category == category && spawn.used == false);
		}

		public static Transform GetRandomPointForLateSpawn()
		{
			return GetPointsForCategory(SpawnPointCategory.LateJoin).ToList().PickRandom();
		}

		public static Transform GetRandomPointForJob(JobType job, bool isGhost = false)
		{
			if (categoryByJob.ContainsKey(job))
			{
				//Get all available points and order by priority, higher numbers picked first
				var points = GetPointsForCategory(categoryByJob[job]).OrderBy(x => x.GetComponent<SpawnPoint>().priority).ToList();

				if (points.Any() == false)
				{
					// Default to arrivals if there is no mapped spawn point for this job!
					// Will still return null if there is no arrivals spawn points set (and people will just not spawn!).
					return GetRandomPointForLateSpawn();
				}

				//Get last point as that should have biggest priority
				var last = points.Last();
				if (last != null && last.TryGetComponent<SpawnPoint>(out var spawn))
				{
					//If the priority isnt 0 then we use this one else choose random
					if (spawn.priority != 0)
					{
						//If this point is only allowed once then set it to used, dont allow ghosts to use up a spawn point
						if (spawn.type == SpawnPointType.Once && isGhost == false)
						{
							spawn.used = true;
						}

						return last;
					}

					//Pick random as all points will be 0
					last = points.PickRandom();

					//If this point is only allowed once then set it to used, dont allow ghosts to use up a spawn point
					if (spawn.type == SpawnPointType.Once && isGhost == false)
					{
						spawn.used = true;
					}

					return last;
				}
			}

			// Default to arrivals if there is no mapped spawn point for this job!
			// Will still return null if there is no arrivals spawn points set (and people will just not spawn!).
			return GetRandomPointForLateSpawn();
		}

		private const string DEFAULT_SPAWNPOINT_ICON = "Mapping/mapping_x2.png";
		private string iconName => iconNames.ContainsKey(category) ? iconNames[category] : DEFAULT_SPAWNPOINT_ICON;

		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, iconName);
		}

		private static readonly Dictionary<JobType, SpawnPointCategory> categoryByJob = new Dictionary<JobType, SpawnPointCategory>
		{
			{ JobType.COMMANDING_OFFICER, SpawnPointCategory.CommandingOfficer },
			{ JobType.EXECUTIFE_OFFICER, SpawnPointCategory.ExecutifeOfficer },
			{ JobType.STAFF_OFFICER, SpawnPointCategory.StaffOfficer },
			{ JobType.INTELLIGATE_OFFICER, SpawnPointCategory.IntelligateOfficer },
			{ JobType.PILOT_OFFICER, SpawnPointCategory.PilotOfficer },
			{ JobType.VEHICLE_CREWMAN, SpawnPointCategory.VehicleCrewman},
			{ JobType.SENIOR_ENLISTED, SpawnPointCategory.SeniorEnlisted },
			{ JobType.ADVISOR, SpawnPointCategory.Advisor },

			{ JobType.SQUAD_MARINE, SpawnPointCategory.SquadMarine},
		};

		private static readonly Dictionary<SpawnPointCategory, string> iconNames = new Dictionary<SpawnPointCategory, string>()
		{
			{SpawnPointCategory.SquadMarine, "Mapping/mapping_SquadMarine.png"},
		};

	}




	public enum SpawnPointCategory
	{
		SquadMarine,
		SquadEngineer,
		SquadMedic,
		SquadSmartgunner,
		SquadSpecialist,
		SquadLeader,
		Nurse,
		Doctor,
		Researcher,
		ChiefMedicalOfficer,
		RequisitionsOfficer,
		MaintenanceTechnician,
		OrdanceTechician,
		ChiefEngineer,
		MilitaryPolice,
		MilitaryWarden,
		ChiefMP,
		Synthentic,
		CorporateLiasion,
		Advisor,
		SeniorEnlisted,
		VehicleCrewman,
		IntelligateOfficer,
		PilotOfficer,
		StaffOfficer,
		ExecutifeOfficer,
		CommandingOfficer,
		Survivor,
		LateJoin
	}

	public enum SpawnPointType
	{
		Unlimited,
		Once
	}
}
