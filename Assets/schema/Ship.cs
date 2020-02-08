// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.26
// 

using Colyseus.Schema;

public class Ship : Entity {
	[Type(7, "string")]
	public string name = "";

	[Type(8, "string")]
	public string ship_type = "";

	[Type(9, "string")]
	public string ship_material = "";

	[Type(10, "string")]
	public string primary_weapon = "";

	[Type(11, "string")]
	public string special_weapon = "";

	[Type(12, "number")]
	public float primary_cooldown_max = 0;

	[Type(13, "number")]
	public float primary_cooldown = 0;

	[Type(14, "number")]
	public float special_cooldown_max = 0;

	[Type(15, "number")]
	public float special_cooldown = 0;

	[Type(16, "number")]
	public float kills = 0;

	[Type(17, "number")]
	public float kill_score = 0;

	[Type(18, "number")]
	public float current_kills = 0;

	[Type(19, "int32")]
	public int shields = 0;

	[Type(20, "number")]
	public float damage = 0;

	[Type(21, "number")]
	public float fire_rate = 0;

	[Type(22, "number")]
	public float range = 0;

	[Type(23, "int32")]
	public int max_shields = 0;

	[Type(24, "number")]
	public float shields_recharge_cooldown = 0;

	[Type(25, "number")]
	public float shields_recharge_time = 0;

	[Type(26, "number")]
	public float speed = 0;

	[Type(27, "number")]
	public float accelleration = 0;

	[Type(28, "number")]
	public float rank = 0;

	[Type(29, "number")]
	public float highest_wave = 0;

	[Type(30, "number")]
	public float level = 0;

	[Type(31, "number")]
	public float previous_level = 0;

	[Type(32, "number")]
	public float next_level = 0;

	[Type(33, "int32")]
	public int upgrade_points = 0;

	[Type(34, "int32")]
	public int upgrade_damage = 0;

	[Type(35, "int32")]
	public int upgrade_range = 0;

	[Type(36, "int32")]
	public int upgrade_fire_rate = 0;

	[Type(37, "int32")]
	public int upgrade_accelleration = 0;

	[Type(38, "int32")]
	public int upgrade_speed = 0;

	[Type(39, "int32")]
	public int upgrade_shields_max = 0;

	[Type(40, "int32")]
	public int upgrade_shields_recharge = 0;
}

