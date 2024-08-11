using Godot;

public class Human : Mob {
  public override bool Place(Vector2 position) {
    GD.Print("Placed Human at " + position);
    return true;
  }

  public override void Ai() {
    GD.Print("Human AI Tick");
  }
}