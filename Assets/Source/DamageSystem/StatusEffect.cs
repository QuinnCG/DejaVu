namespace Quinn.DamageSystem
{
	[System.Flags]
	public enum StatusEffect
	{
		None = 0,
		Burning = 1 << 0,
		Freezing = 1 << 1,
	}
}
