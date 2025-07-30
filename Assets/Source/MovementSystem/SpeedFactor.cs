namespace Quinn.MovementSystem
{
	public class SpeedFactor
	{
		public float Factor { get; set; } = 1f;

		/// <summary>
		/// Sets the interval <see cref="Factor"/> to <c>1f</c>.
		/// </summary>
		public void Reset()
		{
			Factor = 1f;
		}
	}
}
