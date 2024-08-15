using System.Threading.Tasks;

public class PlaceTree : Card {
  public PlaceTree() {
    ManaCost = 1;
    MiniSpriteId = 2;
    SpriteId = 3;
  }
 
  public override async Task<bool> Play() => await PlaceMode(new Tree(), 1);
}

public class PlaceFire : Card {
  public PlaceFire() {
    ManaCost = 4;
    MiniSpriteId = 4;
    SpriteId = 5;
  }
 
  public override async Task<bool> Play() => await PlaceMode(new Fire(), 1);
  
}

public class PlaceWater : Card {
  public PlaceWater() {
    ManaCost = 1;
    MiniSpriteId = 6;
    SpriteId = 7;
  }
 
  public override async Task<bool> Play() => await PlaceMode(new Tile(Island.PixelType.WATER), 1);
  
}

public class PlaceStone : Card {
  public PlaceStone() {
    ManaCost = 3;
    MiniSpriteId = 8;
    SpriteId = 9;
  }
 
  public override async Task<bool> Play() => await PlaceMode(new Tile(Island.PixelType.STONE), 1);
  
}

public class PlaceGrass : Card {
  public PlaceGrass() {
    ManaCost = 3;
    MiniSpriteId = 14;
    SpriteId = 15;
  }
 
  public override async Task<bool> Play() => await PlaceMode(new Tile(Island.PixelType.GRASS), 1);
  
}

public class PlaceBunny : Card {
  public PlaceBunny() {
    ManaCost = 2;
    MiniSpriteId = 10;
    SpriteId = 11;
  }

  public override async Task<bool> Play() => await PlaceMode(new Bunny(), 1);
  
  
}

public class PlaceHuman : Card {
  public PlaceHuman() {
    ManaCost = 5;
    MiniSpriteId = 12;
    SpriteId = 13;
  }

  public override async Task<bool> Play() => await PlaceMode(new Human(), 1);
  
  
}

public class FireToStone : Card {
  public FireToStone() {
    ManaCost = 9;
    MiniSpriteId = 16;
    SpriteId = 17;
  }

  public override Task<bool> Play() {
    foreach (Creature mob in player.currentMobs) {
      if (mob is not Fire fire) continue;
      new Stone().Place(fire.GlobalPosition);
      fire.Destroy();
    }
    return Task.FromResult(true);
  }
  
}