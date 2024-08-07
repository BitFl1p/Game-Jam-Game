using Godot;

public partial class Parallax : Node {
  [Export] private float attenuation, contrast;
  [Export] private Vector2 offset;
  [Export] private StarSpawner[] objects;
  private Vector2 mousePos;
  public override void _Input(InputEvent @event) {
    if (@event is not InputEventMouseMotion eventMouseMotion) return;
    mousePos = eventMouseMotion.Position;
  }

  public override void _Process(double delta) {
    for (int i = 0; i < objects.Length; i++) {
      objects[i].Position = new Vector2(i + attenuation, i + attenuation) * contrast * mousePos +
                            new Vector2(Mathf.Sin(Time.GetTicksMsec() / 2000f) * 25, Mathf.Cos(Time.GetTicksMsec() / 2000f) * 25) * contrast * i + offset;
    }
  }
}
