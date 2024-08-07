using Godot;

public partial class StarSpawner : Node2D {
  private double timer = 10;
  private RandomNumberGenerator random = new();
  private PackedScene star;
  [Export] private float baseTime, deviation;
  public override void _Ready() {
    random.Randomize();
    timer = random.Randf() % 5;
    star = GD.Load<PackedScene>("res://Prefabs/star.tscn");
  }

  public override void _Process(double delta) {
    timer -= delta;
    if (timer > 0) return;
    
    AnimatedSprite2D node = star.Instantiate<AnimatedSprite2D>();
    node.Position = new Vector2(random.Randf() * 64 - 32, random.Randf() * 64 - 32);
    AddChild(node);
    
    timer = random.Randf() * deviation + baseTime;
  }
}
