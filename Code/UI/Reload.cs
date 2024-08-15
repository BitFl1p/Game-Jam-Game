using Godot;
using System;

public partial class Reload : TextureButton
{
  public override void _Pressed() => GetTree().Quit();
}
