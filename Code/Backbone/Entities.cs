using Godot;
using System;
using System.Threading.Tasks;

public interface IPlaceable {
  public bool Place(Vector2 position);
}


public abstract partial class Mob : Node2D, IPlaceable {
  
  public Vector2I islandOffset = new(3, 2);
  public Vector2I islandPos = new(1, 2);
  
  public abstract bool Place(Vector2 position);
  public abstract void Ai();
}

public abstract class Tile : IPlaceable {
  public abstract Island.PixelType Type { get; internal set; }
  public bool Place(Vector2 position) {
    throw new NotImplementedException();
  }
}

public interface ICard {
  public Task<bool> Play();

}
public abstract class Card : ICard {
  public int ManaCost { get; internal set; }
  public int MiniSpriteID { get; internal set; }
  public int SpriteID { get; internal  set; }
  protected readonly PlayerMaster player;
  public abstract Task<bool> Play();

  protected Card() {
    PlayerMaster.MouseClick += Clicked;
    player = PlayerMaster.Instance;
  }

  

  private TaskCompletionSource<Vector2> tcs = new();
  private void Clicked(Vector2 pos) => tcs.TrySetResult(pos);
  

  
  internal async Task<bool> PlaceMode(IPlaceable toPlace, int amount) {
    player.playMode = PlayerMaster.Mode.PLACING;
    while (amount > 0) {
      tcs = new TaskCompletionSource<Vector2>();
      Vector2 pos = await tcs.Task;
      if(pos is not {X: > 7 and < 57, Y: > 10 and < 44}) continue;
      if (!toPlace.Place(pos)) return false;
      amount--;
    }
    player.playMode = PlayerMaster.Mode.IDLE;
    return true;
  }
}

