// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.33
// 

using Colyseus.Schema;

public class Ship : Entity {
	[Type(7, "string")]
	public string name = "";

	[Type(8, "string")]
	public string shipType = "";

	[Type(9, "string")]
	public string shipMaterial = "";

	[Type(10, "string")]
	public string primaryWeapon = "";

	[Type(11, "string")]
	public string specialWeapon = "";

	[Type(12, "number")]
	public float primaryCooldownMax = 0;

	[Type(13, "number")]
	public float primaryCooldown = 0;

	[Type(14, "number")]
	public float specialCooldownMax = 0;

	[Type(15, "number")]
	public float specialCooldown = 0;

	[Type(16, "number")]
	public float kills = 0;

	[Type(17, "number")]
	public float killScore = 0;

	[Type(18, "number")]
	public float currentKills = 0;

	[Type(19, "int32")]
	public int shields = 0;

	[Type(20, "number")]
	public float damage = 0;

	[Type(21, "number")]
	public float fireRate = 0;

	[Type(22, "number")]
	public float range = 0;

	[Type(23, "int32")]
	public int maxShields = 0;

	[Type(24, "number")]
	public float shieldsRechargeCooldown = 0;

	[Type(25, "number")]
	public float shieldsRechargeTime = 0;

	[Type(26, "number")]
	public float speed = 0;

	[Type(27, "number")]
	public float accelleration = 0;

	[Type(28, "number")]
	public float rank = 0;

	[Type(29, "number")]
	public float highestWave = 0;

	[Type(30, "number")]
	public float level = 0;

	[Type(31, "number")]
	public float previousLevel = 0;

	[Type(32, "number")]
	public float nextLevel = 0;

	[Type(33, "int32")]
	public int upgradePoints = 0;

	[Type(34, "int32")]
	public int upgradeDamage = 0;

	[Type(35, "int32")]
	public int upgradeRange = 0;

	[Type(36, "int32")]
	public int upgradeFireRate = 0;

	[Type(37, "int32")]
	public int upgradeAccelleration = 0;

	[Type(38, "int32")]
	public int upgradeSpeed = 0;

	[Type(39, "int32")]
	public int upgradeShieldsMax = 0;

	[Type(40, "int32")]
	public int upgradeShieldsRecharge = 0;
}

