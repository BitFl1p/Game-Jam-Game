using Godot;

public partial class CardVisual : Sprite2D {
  public Card card;
  private bool zoomed;
  [Export] private Number number;
  public int manaModifier;
  public bool Zoomed {
    get => zoomed;
    set {
      if(value == zoomed) return;
      zoomed = value;
      Frame = zoomed ? card.SpriteID : card.MiniSpriteID;
      number.Position = zoomed ?  new Vector2(-13, -22.5f) : new Vector2(-6, -11.5f);
    }
  }

  public override void _Ready() {
    Frame = zoomed ? card.SpriteID : card.MiniSpriteID;
    number.Position = zoomed ?  new Vector2(-13, -22.5f) : new Vector2(-6, -11.5f);
  }

  public override void _Process(double delta) {
    number.Frame = card.ManaCost + manaModifier;
  }
}
