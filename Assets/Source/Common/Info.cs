using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// A simple container for some info to be provided in the editor.
	/// </summary>
	public class Info : MonoBehaviour
	{
		[SerializeField, Multiline, HideLabel]
		private string Message;
	}
}
