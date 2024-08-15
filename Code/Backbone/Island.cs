using Godot;
using System;

public partial class Island : Sprite2D {
  
  public enum PixelType : byte {
    NONE,
    DIRT, GRASS, TALL_GRASS,
    BLOOMING_GRASS,
    FIRE, ASH, FERTILIZED_DIRT, WATER, SIMMERING_WATER,
    STONE
  }
  
  private Image image;
  private RandomNumberGenerator random = new();
  private byte[] tilemap = new byte[53 * 36];
  
  public override void _Ready() {
    image = Texture.GetImage();
    random.Randomize();
    
    Array.Fill(tilemap, (byte)PixelType.DIRT);
    for(int i = 0; i < 53 * 4; i++) tilemap[i] = (byte)PixelType.GRASS;

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
        if (x2 == 1 && y2 == 1) continue;
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];

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

    if (grass < 4) {
      if(random.RandiRange(0, 10) == 0) temp[4] = (byte)PixelType.DIRT;
    }
    else {
      if (random.RandiRange(0, 4) == 0) {
        int i = random.RandiRange(0, 8);
        temp[i] = temp[i] switch {
          (byte)PixelType.DIRT => (byte)PixelType.GRASS,
          (byte)PixelType.FERTILIZED_DIRT => (byte)PixelType.TALL_GRASS,
          _ => temp[i]
        };
      }

      if (random.RandiRange(0, 32) == 0) {
        int i = random.RandiRange(0, 8);
        if (temp[i] == (byte)PixelType.WATER) temp[i] = (byte)PixelType.GRASS;
      }
    }
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
      try { if (temp[y2 * 3 + x2] == tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) temp[y2 * 3 + x2] = 0; }
      catch { temp[y2 * 3 + x2] = 0; }
    }
    return temp;
  }
  private byte[] TallGrassLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte grass = 0;
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      try {
        if (x2 == 1 && y2 == 1) continue;
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];

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

    if (grass < 4) {
      if(random.RandiRange(0, 10) == 0) temp[4] = (byte)PixelType.GRASS;
    }
    else {
      if (random.RandiRange(0, 20) == 0) {
        int i = random.RandiRange(0, 8);
        temp[i] = temp[i] switch {
          (byte)PixelType.DIRT => (byte)PixelType.GRASS,
          (byte)PixelType.FERTILIZED_DIRT => (byte)PixelType.BLOOMING_GRASS,
          _ => temp[i]
        };
      }

      if (random.RandiRange(0, 64) == 0) {
        int i = random.RandiRange(0, 8);
        if (temp[i] == (byte)PixelType.GRASS) temp[i] = (byte)PixelType.TALL_GRASS;
      }
    }
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
      try { if (temp[y2 * 3 + x2] == tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) temp[y2 * 3 + x2] = 0; }
      catch { temp[y2 * 3 + x2] = 0; }
    }
    
    return temp;
  }
  private byte[] BloomingGrassLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte grass = 0;
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      try {
        if (x2 == 1 && y2 == 1) continue;
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];

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

    if (grass < 4) {
      if(random.RandiRange(0, 10) == 0) temp[4] = (byte)PixelType.GRASS;
    }
    else {
      if (random.RandiRange(0, 20) == 0) {
        int i = random.RandiRange(0, 8);
        if (temp[i] == (byte)PixelType.DIRT) temp[i] = (byte)PixelType.GRASS;
      }

      if (random.RandiRange(0, 64) == 0) {
        int i = random.RandiRange(0, 8);
        if (temp[i] == (byte)PixelType.TALL_GRASS) temp[i] = (byte)PixelType.BLOOMING_GRASS;
      }
    }
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
      try { if (temp[y2 * 3 + x2] == tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) temp[y2 * 3 + x2] = 0; }
      catch { temp[y2 * 3 + x2] = 0; }
    }
    
    return temp;
  }
  private byte[] FireLogic(int x, int y) {    
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte grass = 0;
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      try {
        if (x2 == 1 && y2 == 1) continue;
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];

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

    if (grass < 4) {
      if(random.RandiRange(0, 10) == 0) temp[4] = (byte)PixelType.ASH;
    }
    else {
      if (random.RandiRange(0, 4) == 0) {
        int i = random.RandiRange(0, 8);
        if (temp[i] is (byte)PixelType.GRASS or (byte)PixelType.TALL_GRASS or (byte)PixelType.BLOOMING_GRASS) temp[i] = (byte)PixelType.FIRE;
      }

      if (random.RandiRange(0, 64) == 0) new Fire().Place(new Vector2I(x, y) + PlayerMaster.IslandOffset);
    }
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
      try { if (temp[y2 * 3 + x2] == tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) temp[y2 * 3 + x2] = 0; }
      catch { temp[y2 * 3 + x2] = 0; }
    }
    
    return temp;
  }
  private byte[] WaterLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      try {
        if (x2 == 1 && y2 == 1) continue;
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];
      }
      catch {
        temp[y2 * 3 + x2] = 0;
      }
    }

    if (random.RandiRange(0, 20) == 0) temp[4] = (byte)PixelType.DIRT;
    for (int i = 0; i < 8; i++) temp[i] = temp[i] switch { 
      (byte)PixelType.DIRT or (byte)PixelType.STONE => (byte)PixelType.WATER,
      (byte)PixelType.FIRE or (byte)PixelType.ASH => (byte)PixelType.SIMMERING_WATER,
      _ => temp[i]
    };
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
      try { if (temp[y2 * 3 + x2] == tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) temp[y2 * 3 + x2] = 0; }
      catch { temp[y2 * 3 + x2] = 0; }
    }
    return temp;
  }
  private byte[] SimmeringWaterLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {

      try {
        if (x2 == 1 && y2 == 1) continue;
        temp[y2 * 3 + x2] = tilemap[53 * (y + y2 - 1) + (x + x2 - 1)];
      }
      catch {
        temp[y2 * 3 + x2] = 0;
      }
    }

    if (random.RandiRange(0, 10) == 0) temp[4] = (byte)PixelType.FERTILIZED_DIRT;
    for (int i = 0; i < 8; i++) temp[i] = temp[i] switch {
      (byte)PixelType.FIRE or (byte)PixelType.ASH => (byte)PixelType.SIMMERING_WATER,
      _ => temp[i]
    };
    
    for (int x2 = 0; x2 <= 2; x2++) for (int y2 = 0; y2 <= 2; y2++) {
      try { if (temp[y2 * 3 + x2] == tilemap[53 * (y + y2 - 1) + (x + x2 - 1)]) temp[y2 * 3 + x2] = 0; }
      catch { temp[y2 * 3 + x2] = 0; }
    }
    return temp;
  }
  private byte[] StoneLogic(int x, int y) {
    byte[] temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    if (random.RandiRange(0, 1024) == 0) new Stone().Place(new Vector2I(x, y) + PlayerMaster.IslandOffset);
    return temp;
  }
  
  private byte[] HandlePixelLogic(int x, int y) => tilemap[53 * y + x] switch {
    (byte)PixelType.GRASS => GrassLogic(x, y),
    (byte)PixelType.TALL_GRASS => TallGrassLogic(x, y),
    (byte)PixelType.BLOOMING_GRASS => BloomingGrassLogic(x, y),
    (byte)PixelType.FIRE => FireLogic(x, y),
    (byte)PixelType.WATER => WaterLogic(x, y),
    (byte)PixelType.SIMMERING_WATER => SimmeringWaterLogic(x, y),
    (byte)PixelType.STONE => StoneLogic(x, y),
    _ => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
  };
  
  private static Color FindPixelColor(byte tile) => tile switch {  
    (byte)PixelType.DIRT => new Color(0xc46833ff),
    (byte)PixelType.GRASS => new Color(0x66aa5dff),
    (byte)PixelType.TALL_GRASS => new Color(0x2a8379ff),
    (byte)PixelType.BLOOMING_GRASS => new Color(0xe687c5ff),
    (byte)PixelType.FIRE => new Color(0xdd867dff),
    (byte)PixelType.WATER => new Color(0x589ffcff),
    (byte)PixelType.SIMMERING_WATER => new Color(0x7be1f6ff),
    (byte)PixelType.ASH => new Color(0x635d96ff),
    (byte)PixelType.FERTILIZED_DIRT => new Color(0xe49057ff),
    (byte)PixelType.STONE => new Color(0xc4c7eeff),
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
          if (temp[3 * y2 + x2] != 0 && x + x2 != 0 && x + x2 < 53) newMap[53 * (y + y2 - 1) + (x + x2 - 1)] = temp[3 * y2 + x2];
        break;
    }
    return newMap;
  }
  
  public void RedrawPixels() {
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
      return PixelType.NONE;
    }
  }
  #endregion
  
}

