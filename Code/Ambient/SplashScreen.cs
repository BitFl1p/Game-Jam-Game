using Godot;

public partial class SplashScreen : AnimationPlayer
{
  private void _on_animation_finished() => GetTree().ChangeSceneToFile("res://Scenes/TestLevel.tscn");
}
