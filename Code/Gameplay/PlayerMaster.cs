using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PlayerMaster : Node2D {
  
  public enum Mode : byte {
    IDLE, PLACING, INFO
  }

  private bool gameOver;
  [Export] private ColorRect gameOverScreen;
  public static Vector2I IslandOffset => new(7, 9);
  private RandomNumberGenerator rand = new();
  
  [Export] private int maxMana, mana, baseManaRegen;
  [Export] private TextureProgressBar manaBar;
  [Export] public float animSpeed = .1f;
  [Export] public Island island;
  [Export] private PackedScene cardPrefab, manaParticle;
  [Export] private RichTextLabel manaText;
  [Export] private DeckVisual deckVisual;
  [Export] public TimePanel timePanel;
  [Export] public SoundManager soundManager;
  
  public Mode playMode;
  private readonly List<Card> hand = new();
  private readonly List<CardVisual> cardVisuals = new();
  private List<Card> deck;
  public readonly List<Creature> currentMobs = new();
  private byte selectedCard;
  private Vector2 mousePos;
  #region Singleton
  public static PlayerMaster Instance { get; private set; }
  public static Action<Vector2> MouseClick;
  public static event Action MouseRightClick;
  public override void _Ready() {
    if (Instance is not null) QueueFree();
    else Instance = this;
    
    rand.Randomize();
    
    InitialiseCards();

    deck = new List<Card> {
      new PlaceTree(), new PlaceTree(), new PlaceTree(), new PlaceTree(), new PlaceTree(),  new PlaceTree(),  new PlaceTree(),  new PlaceTree(),  new PlaceTree(),  new PlaceTree(), 
      new PlaceFire(), new PlaceFire(), 
      new PlaceWater(), new PlaceWater(), new PlaceWater(), 
      new PlaceStone(),
      new PlaceGrass(), new PlaceGrass(), new PlaceGrass(),
      new PlaceBunny(), new PlaceBunny(), 
      new PlaceHuman(), 
      new FireToStone(), 
    };
    timePanel.Speed = 0;
    mana = maxMana;
  }
  #endregion

  #region Ticker
  [Export] private float tickRate = 0.5f;
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
    if(gameOver) return;
    switch (@event) {
      case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouse:
        MouseClick?.Invoke(mouse.Position);
        if (selectedCard == 255 || playMode != Mode.IDLE) return;
        if (selectedCard == 6 && hand.Count < 5 && mana >= hand.Count) {
          mana -= hand.Count;
          AddCard(deck[rand.RandiRange(0, deck.Count - 1)]);
          selectedCard = 255;
          soundManager.Play(SoundManager.Sound.DRAW);
        }
        else if (hand[selectedCard].ManaCost <= mana) {
          int speed = timePanel.Speed;
          timePanel.Speed = 0;
          bool what = await hand[selectedCard].Play();
          if (what) {
            UseMana(hand[selectedCard].ManaCost);
            RemoveCard(selectedCard);
          }
          selectedCard = 255;
          timePanel.Speed = speed;
        }
        break;
      case InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right }:
        MouseRightClick?.Invoke();
        if (selectedCard >= 6 || playMode != Mode.IDLE || mana == 0) return;
        GainMana(hand.Count - 2, (Vector2I)cardVisuals[selectedCard].GlobalPosition);
        RemoveCard(selectedCard);
        if (mana < 0) mana = 0; 
        selectedCard = 255;
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
    
    if (gameOver) {
      gameOverScreen.Visible = true;
      if(soundManager.music.PitchScale > 0.02f) soundManager.music.PitchScale -= .001f;
      
    }
    else gameOverScreen.Visible = false;
    
    if (mana > 0) return;
    
    bool canRegen = false;
    if (currentMobs.Count > 0) foreach (Creature mob in currentMobs.Where(mob => mob.canRegenMana)) canRegen = true;
    else canRegen = false;
    if (!canRegen) 
      gameOver = true;
    
    
  }

  public void UseMana(int amount) {
    mana -= amount;
    if (mana < 0) mana = 0;
  }

  public void GainMana(int amount, Vector2I pos) {
    mana += amount;
    ManaParticle particle = manaParticle.Instantiate<ManaParticle>();
    AddChild(particle);
    particle.GlobalPosition = pos;
    soundManager.Play(SoundManager.Sound.GAIN_MANA);
    if (mana > maxMana) mana = maxMana;
  }
  
}
