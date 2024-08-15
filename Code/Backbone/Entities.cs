using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPlaceable {
  public bool Place(Vector2 position);
}


public abstract partial class Creature : AnimatedSprite2D, IPlaceable {
  
  [Export] protected Vector2I islandPos = new(1, 2);
  public abstract bool canRegenMana { get; protected init; }
  public abstract bool Place(Vector2 position);
  public abstract void Destroy();
  protected abstract void Ai();

  protected int GetTilemapPosition() => PlayerMaster.GetTilemapPosition(islandPos);

}

public class Tile : IPlaceable {
  
  private readonly Island.PixelType type;

  public Tile(Island.PixelType type) {
    this.type = type;
  }
  public bool Place(Vector2 position) {
    int pos = PlayerMaster.GetTilemapPosition((Vector2I)position);
    for(int x = -1; x <= 1; x++) for (int y = -1; y <= 1; y++) 
      PlayerMaster.Instance.island.SetTile(pos + y * 53 + x, type);
    PlayerMaster.Instance.island.RedrawPixels();
    return true;
  }
}

public interface ICard {
  public Task<bool> Play();

}
public abstract class Card : ICard {
  public int ManaCost { get; internal init; }

  public int MiniSpriteId { get; internal init; }
  public int SpriteId { get; internal  init; }
  protected readonly PlayerMaster player;
  public abstract Task<bool> Play();

  protected Card() {
    PlayerMaster.MouseClick += Clicked;
    PlayerMaster.MouseRightClick += Cancel;
    player = PlayerMaster.Instance;
  }

  

  private TaskCompletionSource<Vector2> tcs = new();
  private void Clicked(Vector2 pos) => tcs.TrySetResult(pos);
  private void Cancel() => tcs.TrySetCanceled();
  

  
  internal async Task<bool> PlaceMode(IPlaceable toPlace, int amount) {
    player.playMode = PlayerMaster.Mode.PLACING;
    while (amount > 0) {
      tcs = new TaskCompletionSource<Vector2>();
      Vector2 pos = Vector2.Zero;
      try { pos = await tcs.Task; }
      catch {
        player.playMode = PlayerMaster.Mode.IDLE;
        return false;
      }
      
      if(pos is not {X: > 7 and < 57, Y: > 10 and < 44}) continue;
      if (!toPlace.Place(pos)) continue;
      amount--;
    }
    player.playMode = PlayerMaster.Mode.IDLE;
    return true;
  }
}

