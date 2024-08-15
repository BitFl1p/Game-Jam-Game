using System;
using Godot;

public partial class MouseCursor : Sprite2D {
  [Export] private Vector2 mouseOffset;
  private PlayerMaster.Mode playMode => PlayerMaster.Instance.playMode;
  public override void _Ready() {
    Input.MouseMode = Input.MouseModeEnum.Hidden;
  }

  public override void _Input(InputEvent @event) {
    if (@event is not InputEventMouseMotion eventMouseMotion) return;
    GlobalPosition = eventMouseMotion.Position + mouseOffset;
  }

  public override void _Process(double delta) {
    Frame = (int)playMode;
    Offset = playMode switch {
      PlayerMaster.Mode.IDLE => Vector2.Zero,
      PlayerMaster.Mode.PLACING => new Vector2I(-3, -2),
      PlayerMaster.Mode.INFO => new Vector2I(-2, -5),
      _ => Vector2.Zero
    };
    
    
  }
}
