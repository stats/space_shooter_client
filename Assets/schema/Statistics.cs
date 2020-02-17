// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.27
// 

using Colyseus.Schema;

public class Statistics : Schema {
	[Type(0, "map", "number")]
	public MapSchema<float> stats = new MapSchema<float>();
}

