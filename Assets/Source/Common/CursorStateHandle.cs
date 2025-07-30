namespace Quinn
{
	/// <summary>
	/// Retrieve an instance of this class via <see cref="InputManager.GetCursorStateHandle(bool)"/>.<br/>
	/// Said instance can be used to ask for the cursor to be shown.
	/// </summary>
	public class CursorStateHandle
	{
		/// <summary>
		/// If any registered instance of <see cref="CursorStateHandle"/> has <see cref="ForceShowCursor"/> set to true, then the cursor will be shown.<br/>
		/// If this is set to false, another instance could still make it so the cursor is enabled.
		/// </summary>
		/// 
		/// <remarks>
		/// This is ignored if <see cref="InputManager.IsUsingGamepad"/> is true as the cursor will be forced hidden and locked.
		/// </remarks>
		public bool ForceShowCursor { get; set; } = true;
	}
}
