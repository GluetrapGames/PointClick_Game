namespace GlueTrap.Utilities
{
public enum ItemDamageStates
{
	Undamaged = 0,
	Damaged = 1,
	Broken = 2
}

public enum ItemTypes
{
	None = 0,
	Plant = 1,
	Poems = 2,
	Journal = 3,
	Pills = 4,
	Medicine = 5,
	Crowbar = 6,
	Tv = 7,
	Mirror = 8,
	Money = 9,
	Cigarettes = 10,
	FrontdoorKey = 11,
	TaxidermyKey = 12,
	Coins = 13,
	Record = 14
}
public enum LockedDoors
{
	Frontdoor = 0,
	TaxidermyHallway = 1
}
public enum BreakMaterialTypes
{
	None = 0,
	BigGlass = 1,
	Ceramic = 2,
	Electronic = 3,
	Glass = 4,
	Metal = 5,
	Plant = 6,
	Taxidermy = 7,
	Wood = 8
}

public enum EventTypes
{
	None = 0,
	Material = 1,
	bug_shelf = 2,
	record_player = 3,
	taxi_animal = 4,
	glass_cupboard = 5,
	table_ceramic = 6,
	grandfather_clock = 7
}

public enum MaterialTypes
{
	Wood = 1,
	Carpet = 2,
	Tile = 3,
	Glass = 4,
	Metal = 5,
	Grass = 6,
	Stone = 7
}

public enum RoomEntryPoints
{
	None = 0,
	DownHallwayA = 1,
	DownHallwayB = 2,
	DownHallwayC = 3,
	DownHallwayD = 4,
	DownHallwayE = 5,
	LivingRoomA = 6,
	LivingRoomB = 7,
	LivingRoomC = 8,
	DiningRoomA = 9,
	DiningRoomB = 10,
	DiningRoomC = 11,
	KitchenA = 12,
	KitchenB = 13,
	DownBathroom = 14,
	UpHallwayA = 15,
	UpHallwayB = 16,
	UpHallwayC = 17,
	UpHallwayD = 18,
	UpHallwayE = 19,
	MasterBedroom = 20,
	SpareBedroom = 21,
	UpBathroom = 22,
	TaxHallwayA = 23,
	TaxHallwayB = 24,
	Taxidermy = 25,
	DownHallwayF = 26
}

public enum InteractionDir
{
	Left = 0,
	Right = 1,
	Top = 2,
	Bottom = 3
}
}