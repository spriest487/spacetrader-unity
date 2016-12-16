using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class FormationManager
{
	[SerializeField]
	private List<int> followRequests;

	[SerializeField]
	private Dictionary<int, int> followerPositions;

	public ICollection<int> followers { get { return followerPositions.Keys; } }

	public FormationManager()
	{
		followRequests = new List<int>();
		followerPositions = new Dictionary<int, int>();
	}

	public void Update()
	{
		//remove any followers that didn't renew their follow status
		foreach (int currentFollower in followerPositions.Keys)
		{
			if (!followRequests.Contains(currentFollower))
			{
				followerPositions.Remove(currentFollower);
			}
			else
			{
				followRequests.Remove(currentFollower);
			}
		}

		//update to only include old
		var currentFollowers = new LinkedList<int>(followerPositions.Keys);

		//followRequests now only contains unique new followers
		/* add new followers alternately to the "left" and "right" (back and front of
		 the list) */
		var newFollowerCount = followRequests.Count();
		for (int newFollowerIt = 0; newFollowerIt < newFollowerCount; ++newFollowerIt)
		{
			if (newFollowerIt % 2 == 0)
			{
				currentFollowers.AddLast(followRequests[newFollowerIt]);
			}
			else
			{
				currentFollowers.AddFirst(followRequests[newFollowerIt]);
			}
		}

		followerPositions.Clear();

		var followerIds = currentFollowers.ToArray();
		var count = currentFollowers.Count();
		var halfOffset = count / 2;
		for (int followerIt = 0; followerIt < count; ++followerIt)
		{
			var pos = followerIt - halfOffset;
			if (pos >= 0)
			{
				pos += 1;
			}

			followerPositions.Add(followerIds[followerIt], pos);
		}
	}

	public int IncludeFollower(int followerId)
	{
        Debug.Assert(!followRequests.Contains(followerId), "should not receive duplicate follow requests");

		followRequests.Add(followerId);

		//return the current position if this id is already registered
		return GetFollowerPosition(followerId);
	}

	public int GetFollowerPosition(int followerId)
	{
		if (followerPositions.ContainsKey(followerId))
		{
			return followerPositions[followerId];
		}
		else
		{
			return 0;
		}
	}
}
