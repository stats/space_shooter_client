// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.33
// 

using Colyseus.Schema;

public class ShipList : Schema {
	[Type(0, "map", typeof(MapSchema<Ship>))]
	public MapSchema<Ship> ships = new MapSchema<Ship>();
}

