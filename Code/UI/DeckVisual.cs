using Godot;

public partial class DeckVisual : Sprite2D {
  [Export] private Number manaCost;
  public int mana;

  public override void _Process(double delta) {
    manaCost.Frame = mana;
  }
}
