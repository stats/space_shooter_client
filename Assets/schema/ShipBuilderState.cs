// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.33
// 

using Colyseus.Schema;

public class ShipBuilderState : Schema {
	[Type(0, "ref", typeof(Statistics))]
	public Statistics stats = new Statistics();

	[Type(1, "ref", typeof(UnlockMessage))]
	public UnlockMessage unlockMessage = new UnlockMessage();

	[Type(2, "ref", typeof(ErrorMessage))]
	public ErrorMessage error = new ErrorMessage();

	[Type(3, "ref", typeof(ShipList))]
	public ShipList shipList = new ShipList();
}

