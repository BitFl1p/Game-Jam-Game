using Godot;

public partial class MouseCursor : Node2D
{
  public override void _Ready() {
    Input.MouseMode = Input.MouseModeEnum.Hidden;

  }

  public override void _Input(InputEvent @event) {
    if (@event is not InputEventMouseMotion eventMouseMotion) return;
    Position = eventMouseMotion.Position;
  }
}
