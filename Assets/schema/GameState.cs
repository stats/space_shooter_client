// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.26
// 

using Colyseus.Schema;

public class GameState : Schema {
	[Type(0, "map", typeof(MapSchema<Ship>))]
	public MapSchema<Ship> ships = new MapSchema<Ship>();

	[Type(1, "map", typeof(MapSchema<Enemy>))]
	public MapSchema<Enemy> enemies = new MapSchema<Enemy>();

	[Type(2, "map", typeof(MapSchema<Bullet>))]
	public MapSchema<Bullet> bullets = new MapSchema<Bullet>();

	[Type(3, "number")]
	public float start_game = 0;

	[Type(4, "int32")]
	public int current_wave = 0;

	[Type(5, "int32")]
	public int enemies_spawned = 0;

	[Type(6, "int32")]
	public int enemies_killed = 0;
}

