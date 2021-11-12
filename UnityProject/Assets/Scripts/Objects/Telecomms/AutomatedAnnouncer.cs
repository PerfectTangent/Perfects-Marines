using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects.Telecomms
{
	public class AutomatedAnnouncer : MonoBehaviour
	{
		private const string machineName = "Announcing Machine";

		private static readonly DateTime TIME_BEFORE_JOIN_ANNOUNCEMENTS = new DateTime().AddHours(12).AddSeconds(5);

		private static readonly Dictionary<JobType, ChatChannel> channelFromJob = new Dictionary<JobType, ChatChannel>()
	{
		{ JobType.COMMANDING_OFFICER, ChatChannel.Command },
		{ JobType.CHIEF_ENGINEER, ChatChannel.Engineering },
		{ JobType.CHIEF_MEDICAL_OFFICER, ChatChannel.Medical },
		{ JobType.CHIEF_MP, ChatChannel.Security }
	};

		private void OnEnable()
		{
			if (CustomNetworkManager.IsServer)
			{
				PlayerSpawn.SpawnEvent += ServerOnPlayerSpawned;
			}
		}

		private void OnDisable()
		{
			if (CustomNetworkManager.IsServer)
			{
				PlayerSpawn.SpawnEvent -= ServerOnPlayerSpawned;
			}
		}

		private void ServerOnPlayerSpawned(object sender, PlayerSpawn.SpawnEventArgs args)
		{
			if (GameManager.Instance.stationTime < TIME_BEFORE_JOIN_ANNOUNCEMENTS)
			{
				return;
			}

			AnnounceNewCrewmember(args.player);
		}

		private void AnnounceNewCrewmember(GameObject player)
		{
			PlayerScript playerScript = player.GetComponent<PlayerScript>();
			Occupation playerOccupation = playerScript.mind.occupation;
			string playerName = player.ExpensiveName();
			Loudness annoucementImportance = GetAnnouncementImportance(playerOccupation);

			ChatChannel chatChannels = ChatChannel.Common;
			string commonMessage = $"{playerName} has signed up as {playerOccupation.DisplayName}.";
			string deptMessage = $"{playerName}, {playerOccupation.DisplayName}, is the department head.";

			// Get the channel of the newly joined head from their occupation.
			if (channelFromJob.ContainsKey(playerOccupation.JobType))
			{
				BroadcastCommMsg(channelFromJob[playerOccupation.JobType], deptMessage, annoucementImportance);
			}

			// Announce the arrival on the CentComm channel if is a CentComm occupation.
			if (JobCategories.CentCommJobs.Contains(playerOccupation.JobType))
			{
				chatChannels = ChatChannel.CentComm;
			}
			else if (playerOccupation.IsCrewmember == false)
			{
				// Don't announce non-crewmembers like wizards, fugitives at all (they don't have their own chat channel).
				return;
			}

			BroadcastCommMsg(chatChannels, commonMessage, GetAnnouncementImportance(playerOccupation));
		}

		private Loudness GetAnnouncementImportance(Occupation job)
		{
			if (job.JobType == JobType.COMMANDING_OFFICER || job.JobType == JobType.EXECUTIFE_OFFICER || job.JobType == JobType.CHIEF_MP 
			|| job.JobType == JobType.CHIEF_ENGINEER || job.JobType == JobType.CHIEF_MEDICAL_OFFICER || job.JobType == JobType.REQUISITIONS_OFFICER 
			|| job.JobType == JobType.STAFF_OFFICER)
			{
				return Loudness.LOUD;
			}
			else
			{
				return Loudness.NORMAL;
			}
		}

		private void BroadcastCommMsg(ChatChannel chatChannels, string message, Loudness importance)
		{
			Chat.AddCommMsgByMachineToChat(gameObject, message, chatChannels, importance, ChatModifier.ColdlyState,  machineName);
		}
	}
}
