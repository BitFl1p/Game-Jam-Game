using Godot;

public partial class MouseCursor : Node2D {
  [Export] private Vector2 offset;
  public override void _Ready() {
    //Input.MouseMode = Input.MouseModeEnum.Hidden;

  }

  public override void _Input(InputEvent @event) {
    if (@event is not InputEventMouseMotion eventMouseMotion) return;
    GlobalPosition = eventMouseMotion.Position + offset;
  }
}
