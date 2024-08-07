using Godot;
using System;
using System.Threading.Tasks;

public interface IPlaceable {
  public void Place(Vector2 position);
}


public abstract class Mob : IPlaceable {
  public abstract void Place(Vector2 position);
  public abstract void Ai();
}

public abstract class Tile : IPlaceable {
  public abstract Island.PixelType Type { get; internal set; }
  public void Place(Vector2 position) {
    throw new NotImplementedException();
  }
}

public interface ICard {
  public void Play();

}
public abstract class Card : ICard {
  public int ManaCost { get; internal set; }
  public int MiniSpriteID { get; internal set; }
  public int SpriteID { get; internal  set; }
  protected readonly PlayerMaster player;
  public abstract void Play();

  protected Card() {
    PlayerMaster.MouseClick += Clicked;
    player = PlayerMaster.Instance;
  }

  

  private TaskCompletionSource<Vector2> tcs = new();
  private void Clicked(Vector2 pos) => tcs.TrySetResult(pos);
  

  
  internal async Task PlaceMode(IPlaceable toPlace, int amount) {
    player.playMode = PlayerMaster.Mode.PLACING;
    while (amount > 0) {
      tcs = new TaskCompletionSource<Vector2>();
      Vector2 pos = await tcs.Task;
      toPlace.Place(pos);
      amount--;
    }
    player.playMode = PlayerMaster.Mode.IDLE;
  }
}

