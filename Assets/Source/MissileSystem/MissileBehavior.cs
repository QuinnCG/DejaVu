namespace Quinn.MissileSystem
{
	[System.Serializable]
	public abstract class MissileBehavior
	{
		protected Missile Missile { get; private set; }

		public void Init(Missile missile)
		{
			Missile = missile;
		}

		public virtual void OnCreate() { }
		public virtual void OnUpdate() { }
		public virtual void OnDestroy() { }
	}
}
