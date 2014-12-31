using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class FormationManagerTest
{
	private FormationManager formationManager;

	[TestInitialize]
	public void setup()
	{
		formationManager = new FormationManager();
	}

	[TestMethod]
	public void NotAFollower()
	{
		var pos = formationManager.GetFollowerPosition(12345);

		Assert.AreEqual(0, pos);
	}

	[TestMethod]
	public void FollowersGetSequentialPositions()
	{
		formationManager.IncludeFollower(123);
		formationManager.IncludeFollower(456);
		formationManager.IncludeFollower(789);
		formationManager.IncludeFollower(321);

		//should all be 0 until update
		Assert.AreEqual(0, formationManager.GetFollowerPosition(123));
		Assert.AreEqual(0, formationManager.GetFollowerPosition(456));
		Assert.AreEqual(0, formationManager.GetFollowerPosition(789));
		Assert.AreEqual(0, formationManager.GetFollowerPosition(321));

		//all added
		formationManager.Update();

		Assert.AreEqual(1, formationManager.GetFollowerPosition(123));
		Assert.AreEqual(-1, formationManager.GetFollowerPosition(456));
		Assert.AreEqual(2, formationManager.GetFollowerPosition(789));
		Assert.AreEqual(-2, formationManager.GetFollowerPosition(321));
	}

	[TestMethod]
	public void InactiveFollowersAreRemoved()
	{
		formationManager.IncludeFollower(123);
		formationManager.Update(); //added
		
		Assert.AreNotEqual(0, formationManager.GetFollowerPosition(123));

		formationManager.Update(); //removed - inactive

		Assert.AreEqual(0, formationManager.GetFollowerPosition(123));
	}
	
	[TestMethod]
	public void EmptySpacesAreCrunched()
	{
		formationManager.IncludeFollower(123); // 1
		formationManager.IncludeFollower(456); //-1
		formationManager.IncludeFollower(789); // 2
		formationManager.IncludeFollower(321); //-2
		formationManager.IncludeFollower(654); // 3
		formationManager.Update();

		Assert.AreEqual(-2, formationManager.GetFollowerPosition(321));
		Assert.AreEqual(3, formationManager.GetFollowerPosition(654));

		formationManager.IncludeFollower(321);
		formationManager.IncludeFollower(654);
		formationManager.Update();

		Assert.AreEqual(-1, formationManager.GetFollowerPosition(321));
		Assert.AreEqual(1, formationManager.GetFollowerPosition(654));
	}
}
