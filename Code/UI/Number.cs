using Godot;

public partial class Number : Sprite2D {
  private byte number;
  public byte Num {
    get => number;
    set {
      if (number == value) return;
      number = value;
      Frame = number;
    }
  }
}
