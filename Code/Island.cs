using Godot;
using System;
using System.Linq;

public abstract class Pixel {
  public enum PixelType : byte {
    NONE,
    DIRT,
    GRASS,
    TALL_GRASS
  }

  public PixelType pixelType;
  public Color color;

  public abstract void ExecuteBehaviour(byte[] tilemap);
}

public partial class Island : Sprite2D {
  private Image image;
  private RandomNumberGenerator random = new();
  private byte[] tilemap = new byte[53 * 36];
  [Export] private float tickRate = 0.5f;
  private float tick;
  public override void _Ready() {
    image = Texture.GetImage();
    random.Randomize();

    Array.Fill(tilemap, (byte)1);
    tilemap[355] = 2;
    tilemap[356] = 3;
  }
  public override void _Process(double delta) {
    tick -= (float)delta;
    
    if (tick > 0) return;
    ManagePixels();
    tick = tickRate;
  }

  private void ManagePixels() {
    HandleTilemapLogic();
    for(int x = 2; x <= 53; x++) for(int y = 1; y <= 36; y++) switch (x, y) {
      case (2, 1): case(2,36): case(53, 1): case(53, 36): break;
      
      default: 
        image.SetPixel(x, y, FindPixelColor(tilemap[53 * (y-1) + (x-2)])); 
        break;
    }
    for(int x = 2; x <= 53; x++) for(int y = 1; y <= 36; y++) switch (x, y) {
      case (2, 1): case(2,36): case(53, 1): case(53, 36): break;
      
      case var (a, b) when y == 36 || (y == 35 && x is 2 or 53): 
        image.SetPixel(x, y, FindPixelColor(tilemap[53 * (y-1) + (x-2)]) / 1.5f + new Color(0,0,0,255)); 
        break;
      
      default: 
        image.SetPixel(x, y, FindPixelColor(tilemap[53 * (y-1) + (x-2)])); 
        break;
    }
    
    ImageTexture texture = new();
    texture.SetImage(image);
    Texture = texture;
  }

  private byte[] HandleTilemapLogic() {
    byte[] newMap = tilemap;
    for(int x = 0; x <= 52; x++) for(int y = 0; y <= 35; y++) switch (x, y) {
      case (2, 1): case(2,36): case(53, 1): case(53, 36): break;
      
      default:
        byte[] temp = HandlePixelLogic(x, y); 
        for(int x2 = 0; x2 <= 2; x2++) for(int y2 = 0; y2 <= 2; y2++)
          if (temp[3 * y2 + x2] != 0) newMap[53 * (y + y2 - 1) + (x + x2 - 1)] = temp[3 * y2 + x2];
        break;
    }
    return newMap;
  }

  private byte[] HandlePixelLogic(int x, int y) {
    byte[] temp = new byte[9];
    Array.Fill(temp, (byte)0);
    
    switch (tilemap[53 * y + x]) {
      case 2:
        for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
          if ((x2 is not (0 or 2) || y2 != 1) && (y2 is not (0 or 2) || x2 != 1)) continue;
          try { temp[y2 * 3 + x2] = (byte)(random.RandiRange(0, 3) == 0 && x + x2 - 1 < 53  && tilemap[53 * (y + y2 - 1) + (x + x2 - 1)] == 1 ? 2 : 0); }
          catch { temp[y2 * 3 + x2] = 0; }
        }
        break;
      
      case 3: 
        for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
          if ((x2 is not (0 or 2) || y2 != 1) && (y2 is not (0 or 2) || x2 != 1)) continue;
          try { temp[y2 * 3 + x2] = (byte)(random.RandiRange(0, 6) == 0 && x + x2 - 1 < 53  && tilemap[53 * (y + y2 - 1) + (x + x2 - 1)] == 2 ? 3 : 0); }
          catch { temp[y2 * 3 + x2] = 0; }

          if (temp[y2 * 3 + x2] != 0) continue;
          try { temp[y2 * 3 + x2] = (byte)(random.RandiRange(0, 12) == 0 && x + x2 - 1 < 53 && tilemap[53 * (y + y2 - 1) + (x + x2 - 1)] == 1 ? 2 : 0); }
          catch { temp[y2 * 3 + x2] = 0; }
        }
        break;
    }
    return temp;
  }

  private Color FindPixelColor(byte tile) => tile switch {  
    1 => new Color(0xc46833ff),
    2 => new Color(0x66aa5dff),
    3 => new Color(0x2a8379ff),
    _ => new Color(0x00000000)
  };
}

