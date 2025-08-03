using System;
using System.Linq;
using System.Text;
using QFSW.QC;
using QFSW.QC.Suggestors.Tags;
using UnityEngine;

namespace Quinn
{
	public class ConsoleManager : MonoBehaviour
	{
		[SerializeField]
		private QuantumConsole Console;

		public static bool IsOpen { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void StaticReset() => IsOpen = false;

		private CursorStateHandle _cursorState;

		private void Start()
		{
			Console.OnActivate += OnOpen;
			Console.OnDeactivate += OnClose;

			_cursorState = InputManager.Instance.GetCursorStateHandle(false);
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.F12))
			{
				Capture_Cmd();
			}
#endif
		}

		private void OnOpen()
		{
			_cursorState.ForceShowCursor = true;
			InputManager.Instance.BlockInput(this);

			IsOpen = true;
		}

		private void OnClose()
		{
			_cursorState.ForceShowCursor = false;
			InputManager.Instance.UnblockInput(this);

			IsOpen = false;
		}

		[Command("help", "Lists all the commands that the console supports.")]
		protected void Help_Cmd()
		{
			var builder = new StringBuilder();
			builder.AppendLine("Available commands:");

			foreach (var command in QuantumConsoleProcessor.GetUniqueCommands())
			{
				string description = string.Empty;
				if (command.HasDescription)
				{
					description = $": {command.CommandDescription}";
				}

				builder.AppendLine($"    - {command.CommandName}{description}");
			}

			Debug.Log(builder.ToString());
		}
		[Command("help", "Lists information about a specific command.")]
		protected void Help_Cmd([CommandName]string command)
		{
			CommandData foundCommand = QuantumConsoleProcessor.GetUniqueCommands().First(cmd => cmd.CommandName.Equals(command, System.StringComparison.CurrentCultureIgnoreCase));

			string description = string.Empty;
			if (foundCommand.HasDescription)
			{
				description = $": {foundCommand.CommandDescription}";
			}

			Debug.Log($"{foundCommand.CommandName}{description}");
		}
		[Command("cls", "Clears the console of all text.")]
		protected void Clear_Cmd()
		{
			Console.ClearConsole();
		}

		[Command("capture", "Takes a screenshot.")]
		protected void Capture_Cmd()
		{
			Console.Deactivate();
			ScreenCapture.CaptureScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
		}
	}
}
