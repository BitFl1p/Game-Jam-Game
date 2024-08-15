using Godot;

public partial class ManaParticle : Sprite2D {
  public override void _Process(double delta) {
    Scale -= new Vector2((float)delta, (float)delta) * .5f;
    Position += Vector2.Up * (float)delta * 5;
    if(Scale.X <= 0) QueueFree();
  }
}
