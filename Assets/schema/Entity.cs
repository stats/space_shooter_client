// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.15
// 

using Colyseus.Schema;

public class Entity : Schema {
	[Type(0, "string")]
	public string uuid = "";

	[Type(1, "ref", typeof(Position))]
	public Position position = new Position();
}

