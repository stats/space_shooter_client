// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.19
// 

using Colyseus.Schema;

public class Ship : Entity {
	[Type(2, "string")]
	public string name = "";

	[Type(3, "string")]
	public string ship_type = "";

	[Type(4, "string")]
	public string ship_material = "";

	[Type(5, "string")]
	public string primary_weapon = "";

	[Type(6, "string")]
	public string special_weapon = "";

	[Type(7, "number")]
	public float primary_cooldown_max = 0;

	[Type(8, "number")]
	public float primary_cooldown = 0;

	[Type(9, "number")]
	public float special_cooldown_max = 0;

	[Type(10, "number")]
	public float special_cooldown = 0;

	[Type(11, "number")]
	public float kills = 0;

	[Type(12, "number")]
	public float kill_score = 0;

	[Type(13, "number")]
	public float current_kills = 0;

	[Type(14, "int32")]
	public int shields = 0;

	[Type(15, "number")]
	public float damage = 0;

	[Type(16, "number")]
	public float fire_rate = 0;

	[Type(17, "number")]
	public float range = 0;

	[Type(18, "int32")]
	public int max_shields = 0;

	[Type(19, "number")]
	public float shields_recharge_cooldown = 0;

	[Type(20, "number")]
	public float shields_recharge_time = 0;

	[Type(21, "number")]
	public float speed = 0;

	[Type(22, "number")]
	public float accelleration = 0;

	[Type(23, "number")]
	public float rank = 0;

	[Type(24, "number")]
	public float highest_wave = 0;

	[Type(25, "number")]
	public float level = 0;

	[Type(26, "number")]
	public float previous_level = 0;

	[Type(27, "number")]
	public float next_level = 0;

	[Type(28, "int32")]
	public int upgrade_points = 0;

	[Type(29, "int32")]
	public int upgrade_damage = 0;

	[Type(30, "int32")]
	public int upgrade_range = 0;

	[Type(31, "int32")]
	public int upgrade_fire_rate = 0;

	[Type(32, "int32")]
	public int upgrade_accelleration = 0;

	[Type(33, "int32")]
	public int upgrade_speed = 0;

	[Type(34, "int32")]
	public int upgrade_shields_max = 0;

	[Type(35, "int32")]
	public int upgrade_shields_recharge = 0;
}

