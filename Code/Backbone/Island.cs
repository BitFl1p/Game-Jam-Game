using Godot;
using System;

public partial class Island : Sprite2D {
  
  public enum PixelType : byte {
    NONE,
    DIRT, GRASS, TALL_GRASS,
    BLOOMING_GRASS,
    FIRE, ASH, FERTILIZED_DIRT, WATER,
    STONE
  }
  
  private Image image;
  private RandomNumberGenerator random = new();
  private byte[] tilemap = new byte[53 * 36];
  
  public override void _Ready() {
    image = Texture.GetImage();
    random.Randomize();
    
    Array.Fill(tilemap, (byte)1);
    for(int i = 0; i < 100; i++) tilemap[i+100] = 2;

    PlayerMaster.Tick += OnTick;
  }
  private void OnTick() {
    tilemap = HandleTilemapLogic();
    RedrawPixels();
  }

  #region TileLogic
  private byte[] GrassLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte grass = 0;
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      try {
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];
        if (x2 == 1 && y2 == 1) continue;

        switch (tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) {
          case (byte)PixelType.GRASS or 
            (byte)PixelType.TALL_GRASS or 
            (byte)PixelType.BLOOMING_GRASS:
            grass++;
            break;
        }
      }
      catch {
        temp[y2 * 3 + x2] = 0;
      }
    }

    if (grass < 4 && random.RandiRange(0, 10) == 0) {
      temp[4] = (byte)PixelType.DIRT;
      return temp;
    }
    for (int i = 0; i < 9; i++) {
      if(temp[i]== (byte)PixelType.DIRT && random.RandiRange(0, 12) == 0) temp[i] = (byte)PixelType.GRASS;
    }

    return temp;
  }
  private byte[] TallGrassLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte grass = 0;
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      try {
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];
        if (x2 == 1 && y2 == 1) continue;

        switch (tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) {
          case (byte)PixelType.GRASS or 
            (byte)PixelType.TALL_GRASS or 
            (byte)PixelType.BLOOMING_GRASS:
            grass++;
            break;
        }
      }
      catch {
        temp[y2 * 3 + x2] = 0;
      }
    }

    if (grass < 4 && random.RandiRange(0, 10) == 0) {
      temp[4] = (byte)PixelType.DIRT;
      return temp;
    }
    for (int i = 0; i < 9; i++) {
      if(temp[i]== (byte)PixelType.DIRT && random.RandiRange(0, 12) == 0) temp[i] = (byte)PixelType.GRASS;
    }

    return temp;
  }
  private byte[] BloomingGrassLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte tallGrass = 0;
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      if (tilemap.Length <= 53 * (y + y2 - 1) + (x + x2 - 1)) {
        
        continue;
      }
      temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];
      if (x2 == 1 && y2 == 1) continue;
      
      switch (tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) {
        case (byte)PixelType.TALL_GRASS: tallGrass++; break;
      }
    }
    if (tallGrass < 6 && random.RandiRange(0, 12) == 0) temp[4] = (byte)PixelType.FERTILIZED_DIRT;
    
    if (random.RandiRange(0, 12) != 0 || tallGrass > 4) return temp;
    int i = random.RandiRange(0, 9);
    while(i == 4 && temp[i] is (byte)PixelType.GRASS or (byte)PixelType.FERTILIZED_DIRT) i = random.RandiRange(0, 9);
    temp[i] = temp[i] switch {
      (byte)PixelType.GRASS => (byte)PixelType.TALL_GRASS,
      (byte)PixelType.FERTILIZED_DIRT => (byte)PixelType.BLOOMING_GRASS,
      _ => temp[i]
    };
    return temp;
  }
  private byte[] FireLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    return temp;
  }
  private byte[] WaterLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    return temp;
  }
  
  private byte[] HandlePixelLogic(int x, int y) => tilemap[53 * y + x] switch {
    (byte)PixelType.GRASS => GrassLogic(x, y),
    (byte)PixelType.TALL_GRASS => TallGrassLogic(x, y),
    (byte)PixelType.BLOOMING_GRASS => TallGrassLogic(x, y),
    (byte)PixelType.FIRE => TallGrassLogic(x, y),
    (byte)PixelType.WATER => TallGrassLogic(x, y),
    _ => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
  };
  
  private static Color FindPixelColor(byte tile) => tile switch {  
    (byte)PixelType.DIRT => new Color(0xc46833ff),
    (byte)PixelType.GRASS => new Color(0x66aa5dff),
    (byte)PixelType.TALL_GRASS => new Color(0x2a8379ff),
    (byte)PixelType.BLOOMING_GRASS => new Color(0xe687c5ff),
    (byte)PixelType.FIRE => new Color(0xdd867dff),
    (byte)PixelType.WATER => new Color(0x589ffcff),
    (byte)PixelType.STONE => new Color(0xc4c7eeff),
    (byte)PixelType.ASH => new Color(0x635d96ff),
    _ => new Color(0x00000000)
  };
  #endregion

  #region TilemapHandling
  private byte[] HandleTilemapLogic() {
    byte[] newMap = new byte[53 * 36];
    Array.Copy(tilemap, newMap, 53 * 36);
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
  
  private void RedrawPixels() {
    for(int x = 2; x <= 53; x++) for(int y = 1; y <= 36; y++) switch (x, y) {
      case (2, 1): case(2,36): case(53, 1): case(53, 36): break;
      
      case var _ when y == 36 || (y == 35 && x is 2 or 53): 
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

  public bool SetTile(int i, PixelType tile) {
    try {
      tilemap[i] = (byte)tile;
      return true;
    }
    catch {
      return false;
    }
  }
  
  public PixelType GetTile(int i) {
    try {
      return (PixelType)tilemap[i];
    }
    catch {
      return 0;
    }
  }
  #endregion
  
}

