using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public partial class AudioManager : Node
{
    [Export] private AudioStreamPlayer _musicStreamPlayer;
    [Export] private AudioStreamPlayer _sfxStreamPlayer;
    
    private AudioStreamPlaybackPolyphonic _polyPlayback;
    public static AudioManager Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
        
        var poly = new AudioStreamPolyphonic();
        _sfxStreamPlayer.Stream = poly;
        _sfxStreamPlayer.Play(); // запускаем пустой поток
        _polyPlayback = _sfxStreamPlayer.GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
    }
    
    public void PlayMusic(AudioStream stream, float volumeDb = 0f)
    {
        if (_musicStreamPlayer.Stream == stream && _musicStreamPlayer.Playing)
            return;
        
        _musicStreamPlayer.Stream = stream;
        _musicStreamPlayer.VolumeDb = volumeDb;
        _musicStreamPlayer.Play();
    }

    public void StopMusic()
    {
        _musicStreamPlayer.Stop();
    }
    
    public void PlaySFX(AudioStream stream, float volumeDb = 0f)
    {
        if (_polyPlayback != null)
        {
            _polyPlayback.PlayStream(stream, volumeDb);
        }
    }
}