using Godot;

public partial class Star : AnimatedSprite2D {
  [Export] private double timer;

  public override void _Process(double delta) {
    if(timer > 1) Scale += new Vector2((float)delta * 2, (float)delta * 2);
    else Scale -= new Vector2((float)delta * 2, (float)delta * 2);
    
    Scale = new Vector2(Mathf.Clamp(Scale.X, 0, 1),Mathf.Clamp(Scale.Y, 0, 1));
    
    timer -= delta;
    if(timer < 0) QueueFree();
  }
}
