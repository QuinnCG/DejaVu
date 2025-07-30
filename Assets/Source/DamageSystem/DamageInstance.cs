namespace Quinn.DamageSystem
{
	public record DamageInstance
	{
		public DamageInfo OriginalInfo;
		/// <summary>
		/// The actual damage applied. Not, the damage whose application was attempted.
		/// </summary>
		public float RealDamage;
		public bool IsLethal;
	}
}
