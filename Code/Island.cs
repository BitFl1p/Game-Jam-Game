using Godot;
using System;

public partial class Island : Sprite2D {
  [Export] private Image image;
  
  public override void _Ready() {
    image = Texture.GetImage();
    ManagePixels();
  }

  private void ManagePixels() {
    for(int x = 2; x < 10; x++) for(int y = 1; y < 10; y++) image.SetPixel(x, y, Colors.White);
    
    ImageTexture texture = new();
    texture.SetImage(image);
    Texture = texture;
  }
}
