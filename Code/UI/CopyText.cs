using Godot;
using System;

public partial class CopyText : RichTextLabel {
  [Export] private RichTextLabel toCopy;
  public override void _Process(double delta) => Text = toCopy.Text;
}
