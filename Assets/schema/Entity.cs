// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.25
// 

using Colyseus.Schema;

public class Entity : Schema {
	[Type(0, "string")]
	public string uuid = "";

	[Type(1, "ref", typeof(Position))]
	public Position position = new Position();

	[Type(2, "boolean")]
	public bool override_angle = false;

	[Type(3, "number")]
	public float angle = 0;

	[Type(4, "boolean")]
	public bool bullet_invulnerable = false;

	[Type(5, "boolean")]
	public bool collision_invulnerable = false;

	[Type(6, "boolean")]
	public bool invisible = false;
}

