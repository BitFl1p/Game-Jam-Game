using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SoundManager : Node2D {
  public enum Sound {
    DRAW, GAIN_MANA, PLACE_HARSH, PLACE_SOFT
  }
  
  [Export] private AudioStream drawSound, gainMana, placeHarsh, placeSoft;
  private List<AudioStreamPlayer> players = new();
  [Export] public AudioStreamPlayer music;
  [Export] public float sfxVolume, musicVolume;
  public void Play(Sound sound) {
    AudioStreamPlayer player = new();
    switch (sound) {
      case Sound.DRAW:
        player.Stream = drawSound;
        player.VolumeDb = -20 + sfxVolume;
        break;
      case Sound.GAIN_MANA:
        player.Stream = gainMana;
        player.VolumeDb = 10 + sfxVolume;
        break;
      case Sound.PLACE_HARSH:
        player.Stream = placeHarsh;
        player.VolumeDb = sfxVolume;
        break;
      case Sound.PLACE_SOFT:
        player.Stream = placeSoft;
        player.VolumeDb = sfxVolume;
        break;
      default:
        player.Stream = null;
        break;
    }

    if (player.Stream == null) return;
    players.Add(player);
    AddChild(player);
    player.Play();
  }
  public override void _Process(double delta) {
    foreach (AudioStreamPlayer player in players.Where(player => !player.Playing).ToList()) {
      players.Remove(player);
      player.QueueFree();
    }

    music.VolumeDb = -30 + musicVolume;
  }
}
