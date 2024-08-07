using Godot;

public partial class ManaBar : TextureProgressBar
{
  public override void _Ready() {
    Connect("mouse_entered", Callable.From(OnMouseEnter));
    Connect("mouse_exited", Callable.From(OnMouseExit)); 
  }

  private void OnMouseEnter() => ZIndex = 11;
  private void OnMouseExit() => ZIndex = 9;
}
