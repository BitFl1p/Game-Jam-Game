using Godot;

public partial class TimePanel : Node2D {
  private Vector2 mousePos;
  [Export] private Sprite2D pause, play, fastForward;
  private int speed = 60;

  public int Speed {
    get => speed;
    set {
      if(speed == value) return;
      speed = value;
      pause.Modulate = new Color(.5f, .5f, .5f);
      play.Modulate = new Color(.5f, .5f, .5f);
      fastForward.Modulate = new Color(.5f, .5f, .5f);
      PlayerMaster.Instance.tickRateMultiplier = speed;
      switch (speed) {
        case 0: pause.Modulate = new Color(1, 1, 1); break;
        case 1: play.Modulate = new Color(1, 1, 1); break;
        case 2: fastForward.Modulate = new Color(1, 1, 1); break;
      }
    }
  }

  public override void _Input(InputEvent @event) {
    switch (@event) {
      case InputEventMouseMotion mouseMotion: mousePos = mouseMotion.Position; break;
      case InputEventMouseButton mouseButton: if(mousePos.Y < 5 && mouseButton.ButtonIndex == MouseButton.Left) Speed = mousePos.X switch {
        > 0 and < 8 => 0,
        > 8 and < 16 => 1,
        > 16 and < 23 => 2,
        _ => Speed
      };
      break;
    }
  }
  

  public override void _Process(double delta) {
    Position = Position.Lerp(mousePos is { X: < 25, Y: < 5 } ? new Vector2(10, 4) : new Vector2(10, -5), PlayerMaster.Instance.animSpeed);
  }
}
