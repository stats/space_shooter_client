// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.33
// 

using Colyseus.Schema;

public class GameState : Schema {
	[Type(0, "map", typeof(MapSchema<Ship>))]
	public MapSchema<Ship> ships = new MapSchema<Ship>();

	[Type(1, "map", typeof(MapSchema<Enemy>))]
	public MapSchema<Enemy> enemies = new MapSchema<Enemy>();

	[Type(2, "map", typeof(MapSchema<Bullet>))]
	public MapSchema<Bullet> bullets = new MapSchema<Bullet>();

	[Type(3, "map", typeof(MapSchema<Drop>))]
	public MapSchema<Drop> drops = new MapSchema<Drop>();

	[Type(4, "number")]
	public float startGame = 0;

	[Type(5, "int32")]
	public int startWave = 0;

	[Type(6, "int32")]
	public int currentWave = 0;

	[Type(7, "int32")]
	public int enemiesSpawned = 0;

	[Type(8, "int32")]
	public int enemiesKilled = 0;
}

