using System.Linq;
using Godot;

public partial class Parallax : Node {
  [Export] private float attenuation, contrast;
  [Export] private Vector2 offset;
  [Export] private StarSpawner[] objects;

  public override void _Input(InputEvent @event) {
    if (@event is not InputEventMouseMotion eventMouseMotion) return;
    for (int i = 0; i < objects.Length; i++) {
      objects[i].Position = new Vector2(i + attenuation, i + attenuation) * contrast * eventMouseMotion.Position + offset;
    }
  }
}
