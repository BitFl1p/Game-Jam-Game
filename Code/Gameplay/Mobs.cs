using Godot;

public class Human : Mob {
  public override void Place(Vector2 position) {
    GD.Print("Placed Human at " + position);
  }

  public override void Ai() {
    GD.Print("Human AI Tick");
  }
}