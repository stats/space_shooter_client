// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.27
// 

using Colyseus.Schema;

public class UnlockItem : Schema {
	[Type(0, "boolean")]
	public bool unlocked = false;

	[Type(1, "string")]
	public string key = "";

	[Type(2, "number")]
	public float count = 0;
}

