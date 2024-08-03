using Godot;

public partial class ManaBar : TextureProgressBar
{
  public override void _Ready() {
    this.Connect("mouse_entered", Callable.From(OnMouseEnter));
    this.Connect("mouse_exited", Callable.From(OnMouseExit)); 
  }
  void OnMouseEnter() => ZIndex = 11;
  void OnMouseExit() => ZIndex = 9;
}
