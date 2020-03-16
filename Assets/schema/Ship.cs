// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.33
// 

using Colyseus.Schema;

public class Ship : Entity {
	[Type(6, "string")]
	public string name = "";

	[Type(7, "boolean")]
	public bool connected = false;

	[Type(8, "boolean")]
	public bool justDamaged = false;

	[Type(9, "string")]
	public string shipType = "";

	[Type(10, "string")]
	public string shipMaterial = "";

	[Type(11, "string")]
	public string primaryWeapon = "";

	[Type(12, "string")]
	public string specialWeapon = "";

	[Type(13, "number")]
	public float primaryCooldownMax = 0;

	[Type(14, "number")]
	public float primaryCooldown = 0;

	[Type(15, "number")]
	public float specialCooldownMax = 0;

	[Type(16, "number")]
	public float specialCooldown = 0;

	[Type(17, "number")]
	public float kills = 0;

	[Type(18, "number")]
	public float killScore = 0;

	[Type(19, "number")]
	public float currentKills = 0;

	[Type(20, "int32")]
	public int shield = 0;

	[Type(21, "number")]
	public float damage = 0;

	[Type(22, "number")]
	public float fireRate = 0;

	[Type(23, "number")]
	public float range = 0;

	[Type(24, "int32")]
	public int maxShield = 0;

	[Type(25, "number")]
	public float shieldRechargeCooldown = 0;

	[Type(26, "number")]
	public float shieldRechargeTime = 0;

	[Type(27, "number")]
	public float speed = 0;

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
	public int upgradeSpeed = 0;

	[Type(38, "int32")]
	public int upgradeShield = 0;

	[Type(39, "int32")]
	public int upgradeShieldRecharge = 0;

	[Type(40, "array", typeof(ArraySchema<TempUpgrade>))]
	public ArraySchema<TempUpgrade> tempUpgrades = new ArraySchema<TempUpgrade>();

	[Type(41, "number")]
	public float tempUpgradeTimer = 0;
}

