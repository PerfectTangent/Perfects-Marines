using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public enum JobType
{
	//NOTE: To ensure safety of things, like scriptable objects, that are referencing this enum, you must NOT change
	//the ordinals and any new value you add must specify a new ordinal value

	NULL = 0,
	SQUAD_MARINE = 1,
	SQUAD_ENGINEER = 2,
	SQUAD_MEDIC = 3,
	SQUAD_SMARTGUNNER = 4,
	SQUAD_SPECIALIST = 5,
	SQUAD_LEADER = 6,
	NURSE = 7,
	DOCTOR = 8,
	RESEARCHER = 9,
	CHIEF_MEDICAL_OFFICER = 10,
	REQUISITIONS_OFFICER = 11,
	MAINTENANCE_TECHNICIAN = 12,
	ORDANCE_TECHICIAN = 13,
	CHIEF_ENGINEER = 14,
	MILITARY_POLICE = 15,
	MILITARY_WARDEN = 16,
	CHIEF_MP = 17,
	SYNTHENTIC = 18,
	CORPORATE_LIASION = 19,
	ADVISOR = 20,
	SENIOR_ENLISTED = 21,
	VEHICLE_CREWMAN = 22,
	INTELLIGATE_OFFICER = 23,
	PILOT_OFFICER = 24,
	STAFF_OFFICER = 25,
	EXECUTIFE_OFFICER = 26,
	COMMANDING_OFFICER = 27,
	SURVIVOR = 28,
	WIZARD = 29, //DELETE THIS IN FUTURE
}

public static class JobCategories
{
	public static readonly List<JobType> CentCommJobs = new List<JobType>()
	{
		//TO DO
	};
}
