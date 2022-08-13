using Godot;

namespace pdxpartyparrot.ssjAug2022
{
	public class MainMenu : Node
	{
		[Export]
		private PackedScene _initialLevelScene;

		#region Godot Lifecycle

		public override void _Ready()
		{
		}

		#endregion

		private void LoadInitialLevel()
		{
			GD.Print($"Loading initial level ${_initialLevelScene.ResourceName}...");
			GetTree().ChangeSceneTo(_initialLevelScene);
		}

		#region Signals

		private void _on_Play_pressed()
		{
			LoadInitialLevel();
		}

		#endregion
	}
}
