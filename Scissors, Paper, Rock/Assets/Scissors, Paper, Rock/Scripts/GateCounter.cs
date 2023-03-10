using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MoreMountains.InfiniteRunnerEngine
{
    /// <summary>
    /// Add this class to an object and it'll add points when collected.
    /// Note that you'll need a trigger boxcollider on it
    /// </summary>
    public class GateCounter : PickableObject
    {
		/// The amount of points to add when collected
		public int PointsToAdd = 1;

		protected override void ObjectPicked()
		{
			// We pass the specified amount of points to the game manager
			GameManager.Instance.AddPoints(PointsToAdd);

		}
	}
}
