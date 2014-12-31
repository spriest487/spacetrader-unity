using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class HitpointsTest
{
	private Hitpoints hitpoints;

	[TestInitialize]
	public void setup()
	{
		hitpoints = new Hitpoints(2, 100, 100);
	}
	
	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void GetShieldInvalidSector()
	{
		hitpoints.GetShield(-1);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void ZeroSectorsThrows()
	{
		hitpoints = new Hitpoints(0, 1, 1);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void NegativeSectorsThrows()
	{
		hitpoints = new Hitpoints(-1, 1, 1);
	}

	[TestMethod]
	public void InitialShieldsEvenlyDistributed()
	{
		Assert.AreEqual(50, hitpoints.GetShield(0));
		Assert.AreEqual(50, hitpoints.GetShield(1));
		Assert.AreEqual(100, hitpoints.GetArmor());
	}

	[TestMethod]
	public void SimpleDamageEquallyDistributed()
	{
		hitpoints.TakeDamage(50);

		Assert.AreEqual(25, hitpoints.GetShield(0));
		Assert.AreEqual(25, hitpoints.GetShield(1));
		Assert.AreEqual(100, hitpoints.GetArmor());
	}

	[TestMethod]
	public void SimpleDamageAllShieldEquallyDistributed()
	{
		hitpoints.TakeDamage(100);

		Assert.AreEqual(0, hitpoints.GetShield(0));
		Assert.AreEqual(0, hitpoints.GetShield(1));
		Assert.AreEqual(100, hitpoints.GetArmor());
	}

	[TestMethod]
	public void SimpleDamageArmorEquallyDistributed()
	{
		hitpoints.TakeDamage(150);

		Assert.AreEqual(0, hitpoints.GetShield(0));
		Assert.AreEqual(0, hitpoints.GetShield(1));
		Assert.AreEqual(50, hitpoints.GetArmor());
	}

	[TestMethod]
	public void DamageSingleSector()
	{
		hitpoints.TakeDamageInSector(50, 1);

		Assert.AreEqual(50, hitpoints.GetShield(0));
		Assert.AreEqual(0, hitpoints.GetShield(1));
		Assert.AreEqual(100, hitpoints.GetArmor());
	}

	[TestMethod]
	public void DamageAfterShieldHitsArmor()
	{
		hitpoints.TakeDamageInSector(50, 1);
		hitpoints.TakeDamageInSector(50, 1);

		Assert.AreEqual(50, hitpoints.GetShield(0));
		Assert.AreEqual(0, hitpoints.GetShield(1));
		Assert.AreEqual(50, hitpoints.GetArmor());
	}

	[TestMethod]
	public void DamageAfterShieldInOtherSector()
	{
		hitpoints.TakeDamageInSector(50, 0); 
		hitpoints.TakeDamageInSector(50, 1);		

		Assert.AreEqual(0, hitpoints.GetShield(0));
		Assert.AreEqual(100, hitpoints.GetArmor()); 
		
		Assert.AreEqual(0, hitpoints.GetShield(1));
		Assert.AreEqual(100, hitpoints.GetArmor());
	}

	[TestMethod]
	public void OverwhelmingShieldSectorDamagesArmor()
	{
		hitpoints.TakeDamageInSector(150, 0);

		Assert.AreEqual(0, hitpoints.GetShield(0));
		Assert.AreEqual(0, hitpoints.GetArmor());
	}

	[TestMethod]
	public void UnequalRemainingShieldDistributedFairly()
	{
		hitpoints.TakeDamageInSector(25, 0);
		hitpoints.TakeDamage(100);

		Assert.AreEqual(0, hitpoints.GetShield(0));
		Assert.AreEqual(0, hitpoints.GetShield(1));
		Assert.AreEqual(75, hitpoints.GetArmor());
	}
}
