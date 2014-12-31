using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class FormationManager
{
	[SerializeField]
	private HashSet<int> followRequests;

	[SerializeField]
	private IDictionary<int, int> followerPositions;

	public ICollection<int> followers { get { return followerPositions.Keys; } }

	public FormationManager()
	{
		followRequests = new HashSet<int>();
		followerPositions = new Dictionary<int, int>();
	}

	public void Update()
	{
		//remove any followers that didn't renew their follow status
		var currentFollowers = new LinkedList<int>(followerPositions.Keys);

		foreach (int currentFollower in currentFollowers)
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
		currentFollowers = new LinkedList<int>(followerPositions.Keys);

		//followRequests now only contains unique new followers
		var newFollowers = new List<int>(followRequests);
		followRequests.Clear();

		/* add new followers alternately to the "left" and "right" (back and front of
		 the list) */
		var newFollowerCount = newFollowers.Count();
		for (int newFollowerIt = 0; newFollowerIt < newFollowerCount; ++newFollowerIt)
		{
			if (newFollowerIt % 2 == 0)
			{
				currentFollowers.AddLast(newFollowers[newFollowerIt]);
			}
			else
			{
				currentFollowers.AddFirst(newFollowers[newFollowerIt]);
			}
		}

		followerPositions = new Dictionary<int, int>();

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
