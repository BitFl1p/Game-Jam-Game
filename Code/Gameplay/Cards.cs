
using System.Threading.Tasks;

public class TestCard : Card {
  public TestCard() {
    ManaCost = 1;
    MiniSpriteId = 0;
    SpriteId = 1;
  }
 
  public override async Task<bool> Play() {
    if (!await PlaceMode(new Tree(), 5)) return false;
    player.UseMana(ManaCost);
    return true;
  }
}