using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PlayerMaster : Node2D {
  
  public enum Mode : byte {
    IDLE, PLACING
  }

  public static Vector2I IslandOffset => new(7, 9);
  private RandomNumberGenerator rand = new();
  [Export] private int maxMana, mana, baseManaRegen;
  [Export] private TextureProgressBar manaBar;
  [Export] public float animSpeed = .1f;
  [Export] public Island island;
  [Export] private PackedScene cardPrefab;
  [Export] private RichTextLabel manaText;
  [Export] private DeckVisual deckVisual;
  [Export] private TimePanel timePanel;
  public Mode playMode;
  private readonly List<Card> hand = new();
  private readonly List<CardVisual> cardVisuals = new();
  private List<Card> deck;
  public readonly List<Mob> currentMobs = new();
  private byte selectedCard;
  private Vector2 mousePos;
  
  #region Singleton
  public static PlayerMaster Instance { get; private set; }
  public static Action<Vector2> MouseClick;
  public override void _Ready() {
    if (Instance is not null) QueueFree();
    else Instance = this;
    
    InitialiseCards();
    rand.Randomize();

    deck = new List<Card> {new TestCard()};
    timePanel.Speed = 0;
    Tick += OnTick;
    mana = maxMana;
  }
  #endregion

  #region Ticker
  [Export] private float tickRate = 0.25f;
  public float tickRateMultiplier;
  private float tick;
  
  public static event Action Tick;
  private void HandleTicks(float delta) {
    tick -= delta * tickRateMultiplier;
    
    if (tick > 0) return;
    Tick?.Invoke();
    tick = tickRate;
  }
  #endregion

  #region Cards
  public static int GetTilemapPosition(Vector2I globalPos) => 53 * (globalPos.Y - IslandOffset.Y) + (globalPos.X - IslandOffset.X);
  private void InitialiseCards() {
    foreach (CardVisual card in hand.Select(card => { CardVisual visual = cardPrefab.Instantiate<CardVisual>(); visual.card = card; return visual; })) {
      cardVisuals.Add(card);
      AddChild(card);
    }
  }
  private void AddCard(Card card) {
    if (hand.Count >= 5) return;
    hand.Add(card);
    
    CardVisual visual = cardPrefab.Instantiate<CardVisual>();
    visual.card = card;
    visual.Position = new Vector2(64, 64);
    
    cardVisuals.Add(visual);
    AddChild(visual);
  }
  private void RemoveCard(int index) {
    hand.RemoveAt(index);
    cardVisuals[index].QueueFree();
    cardVisuals.RemoveAt(index);
  }

  private void UpdateCards() {
    if(playMode == Mode.IDLE) selectedCard = 255;

    deckVisual.mana = hand.Count;
    if (mousePos is { X: > 56, Y: > 45 } && playMode == Mode.IDLE) {
      selectedCard = 6;
      deckVisual.Position = deckVisual.Position.Lerp(new Vector2(44, 39), animSpeed);
    }
    else {
      deckVisual.Position = deckVisual.Position.Lerp(new Vector2(60, 39), animSpeed);
    }

    
    for (byte i = 0; i < cardVisuals.Count; i++) {
      cardVisuals[i].Position = cardVisuals[i].Position.Lerp(new Vector2(16 + 8 * i, cardVisuals[i].Position.Y), animSpeed);
      if (cardVisuals[i].Position.X - mousePos.X is < 4 and > -4 && mousePos.Y > 50) {
        cardVisuals[i].Position = cardVisuals[i].Position.Lerp(new Vector2(cardVisuals[i].Position.X, playMode == Mode.IDLE ? 39.5f : 100), animSpeed);
        cardVisuals[i].Zoomed = true;
        cardVisuals[i].ZIndex = 100;
        selectedCard = i;
      }
      else {
        cardVisuals[i].Position = cardVisuals[i].Position.Lerp(new Vector2(cardVisuals[i].Position.X, playMode == Mode.IDLE ? 68 - mousePos.Y / 8 : 100), animSpeed);
        cardVisuals[i].Zoomed = false;
        cardVisuals[i].ZIndex = i + 11;
      }
    }
  }

  #endregion
  
  public override async void _Input(InputEvent @event) {
    switch (@event) {
      case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouse:
        MouseClick?.Invoke(mouse.Position);
        if (selectedCard == 255 || playMode != Mode.IDLE) return;
        if (selectedCard == 6 && hand.Count < 5 && mana >= hand.Count) {
          mana -= hand.Count;
          AddCard(deck[rand.RandiRange(0, deck.Count - 1)]);
          selectedCard = 255;
        }
        else if (hand[selectedCard].ManaCost <= mana) {
          timePanel.Speed = 0;
          if (await hand[selectedCard].Play()) {
            RemoveCard(selectedCard);
            selectedCard = 255;
          }
        }
        break;
      case InputEventMouseMotion motion:
        mousePos = motion.Position;
        break;
    }
  }
  
  public override void _Process(double delta) {
    HandleTicks((float)delta);
    UpdateCards();
    
    manaText.Text = "[center]Mana:" + mana + "/" + maxMana + "[/center]";
    
    manaBar.MinValue = 0;
    manaBar.MaxValue = maxMana;
    manaBar.Value = mana;
  }

  public void UseMana(int amount) {
    mana -= amount;
    if (mana < 0) mana = 0;
  }

  public void GainMana(int amount) {
    mana += amount;
    if (mana > maxMana) mana = maxMana;
  }
  
  private void OnTick() {
    //mana += mana >= maxMana ? 0 : baseManaRegen;
    //foreach (Mob mob in currentMobs) 
  }
  
}
