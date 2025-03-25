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
	Money = 9
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
}

public enum MaterialTypes
{
	Wood = 1,
	Carpet = 2,
	Tile = 3,
	Glass = 4,
	Metal = 5,
	Dirt = 6,
	Stone = 7
}
}