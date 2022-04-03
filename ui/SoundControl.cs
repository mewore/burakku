using Godot;

public class SoundControl : VBoxContainer
{
    private const int MIN_VOLUME_DB = -60;
    private const int MAX_VOLUME_DB = 5;

    private const int MASTER_CHANNEL_INDEX = 0;
    private const int SFX_CHANNEL_INDEX = 1;
    private const int MUSIC_CHANNEL_INDEX = 2;

    private CanvasItem sfxNode;
    private HSlider masterSlider;
    private HSlider sfxSlider;
    private CanvasItem musicNode;
    private HSlider musicSlider;

    private static float MasterVolume
    {
        get => VolumeToRatio(AudioServer.GetBusVolumeDb(MASTER_CHANNEL_INDEX));
        set => AudioServer.SetBusVolumeDb(MASTER_CHANNEL_INDEX, RatioToVolume(value));
    }

    private static float SfxVolume
    {
        get => VolumeToRatio(AudioServer.GetBusVolumeDb(SFX_CHANNEL_INDEX));
        set => AudioServer.SetBusVolumeDb(SFX_CHANNEL_INDEX, RatioToVolume(value));
    }

    private static float MusicVolume
    {
        get => VolumeToRatio(AudioServer.GetBusVolumeDb(MUSIC_CHANNEL_INDEX));
        set => AudioServer.SetBusVolumeDb(MUSIC_CHANNEL_INDEX, RatioToVolume(value));
    }

    private bool editable = true;
    public bool Editable
    {
        get => editable;
        set
        {
            editable = value;
            if (masterSlider != null)
            {
                masterSlider.Editable = value;
            }
            SetNonMasterInputsEditable(masterSlider.Value);
        }
    }

    public override void _Ready()
    {
        Global.LoadSettings();
        masterSlider = GetNode<HSlider>("Master/MasterSlider");
        masterSlider.Value = Global.Settings.MasterVolume;
        masterSlider.Editable = editable;

        sfxNode = GetNode<CanvasItem>("Sfx");
        sfxSlider = sfxNode.GetNode<HSlider>("SfxSlider");
        sfxSlider.Value = Global.Settings.SfxVolume;

        musicNode = GetNode<CanvasItem>("Music");
        musicSlider = musicNode.GetNode<HSlider>("MusicSlider");
        musicSlider.Value = Global.Settings.MusicVolume;

        MasterVolume = Global.Settings.NormalizedMasterVolume;
        SfxVolume = Global.Settings.NormalizedSfxVolume;
        MusicVolume = Global.Settings.NormalizedMusicVolume;

        SetNonMasterInputsEditable(masterSlider.Value);
    }

    public void _on_MasterSlider_value_changed(float newValue)
    {
        Global.Settings.MasterVolume = Mathf.RoundToInt(newValue);
        Global.SaveSettings();
        MasterVolume = Global.Settings.NormalizedMasterVolume;
        SetNonMasterInputsEditable(newValue);
    }

    public void _on_SfxSlider_value_changed(float newValue)
    {
        Global.Settings.SfxVolume = Mathf.RoundToInt(newValue);
        Global.SaveSettings();
        SfxVolume = Global.Settings.NormalizedSfxVolume;
    }

    public void _on_MusicSlider_value_changed(float newValue)
    {
        Global.Settings.MusicVolume = Mathf.RoundToInt(newValue);
        Global.SaveSettings();
        MusicVolume = Global.Settings.NormalizedMusicVolume;
    }

    private void SetNonMasterInputsEditable(double masterValue)
    {
        if (sfxSlider == null || musicSlider == null)
        {
            return;
        }
        sfxSlider.Editable = musicSlider.Editable = editable && masterValue > 0;
        float visibility = masterValue > 0 ? 1f : .666f;
        sfxNode.Modulate = musicNode.Modulate = new Color(visibility, visibility, visibility);
    }

    private static float VolumeToRatio(float volume) => Mathf.InverseLerp(MIN_VOLUME_DB, MAX_VOLUME_DB, volume);
    private static float RatioToVolume(float ratio) => Mathf.Lerp(MIN_VOLUME_DB, MAX_VOLUME_DB, ratio);
}
