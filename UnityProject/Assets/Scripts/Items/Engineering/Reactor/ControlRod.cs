using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Engineering
{
	public class ControlRod : ReactorChamberRod
	{
		public decimal AbsorptionPower = 4;

		public override RodType GetRodType()
		{
			return RodType.Control;
		}

	}
}
