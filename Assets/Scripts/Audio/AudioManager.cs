using UnityEngine;
using UnityEngine.Audio;

namespace Goisagi
{

/// <summary>
/// オーディオ管理クラス.
/// </summary>
public partial class AudioManager : TSingleton<AudioManager>
{
    // 優先度.
    public const int PRIORITY_HIGHEST = 0;
    public const int PRIORITY_MIDDLE = 128;
    public const int PRIORITY_LOWEST = 255;

    // Inspectorで設定する項目群.
    [SerializeField] AudioMixer m_masterAudioMixer = null;
    [SerializeField] string m_masterVolumeName = string.Empty;
    [SerializeField] AudioConfig m_bgmConfig = null;
    [SerializeField] AudioConfig m_seConfig = null;
    [SerializeField] AudioConfig m_voiceConfig = null;

    AudioPlayer m_bgmPlayer = null;
    AudioPlayer m_sePlayer = null;
    AudioPlayer m_voicePlayer = null;


    /// <summary>
    /// .
    /// </summary>
    protected override void OnAwake()
    {
        m_bgmPlayer = new AudioPlayer( m_bgmConfig, this.transform, "Bgm" );
        m_sePlayer = new AudioPlayer( m_seConfig, this.transform, "SE" );
        m_voicePlayer = new AudioPlayer( m_voiceConfig, this.transform, "Voice" );

        DontDestroyOnLoad( this.gameObject );
    }
    
    /// <summary>
    /// .
    /// </summary>
    void LateUpdate()
    {
        float deltaTime = Time.unscaledDeltaTime;

        m_bgmPlayer.OnUpdate( deltaTime );
        m_sePlayer.OnUpdate( deltaTime );
        m_voicePlayer.OnUpdate( deltaTime );        
    }

    public void SetMasterVolume( float volumeRate ){ SetVolume( m_masterVolumeName, volumeRate ); }
    public float GetMasterVolume(){ return GetVolume( m_masterVolumeName ); }


    #region BGM
    public AudioController AttachBgmData( AudioResource data ){ return m_bgmPlayer.Attach( data ); }
    public void DetachBgmData( ref AudioController audioInterface ){ m_bgmPlayer.Detach( ref audioInterface ); }
    public void SetBgmVolume( float volumeRate ){ SetVolume( m_bgmConfig.volumeParamName, volumeRate ); }
    public float GetBgmVolume(){ return GetVolume( m_bgmConfig.volumeParamName ); }
    #endregion // BGM


    #region SE
    public AudioController AttachSEData( AudioResource targetData ){ return m_sePlayer.Attach( targetData ); }
    public void DetachSEData( ref AudioController audioInterface ){ m_sePlayer.Detach( ref audioInterface ); }
    public void SetSEVolume( float volumeRate ){ SetVolume( m_seConfig.volumeParamName, volumeRate ); }
    public float GetSEVolume(){ return GetVolume( m_seConfig.volumeParamName ); }
    #endregion // SE


    #region Voice
    public AudioController AttachVoiceData( AudioResource targetData ){ return m_voicePlayer.Attach( targetData ); }
    public void DetachVoiceData( ref AudioController audioInterface ){ m_voicePlayer.Detach( ref audioInterface ); }
    public void SetVoiceVolume( float volumeRate ){ SetVolume( m_voiceConfig.volumeParamName, volumeRate ); }
    public float GetVoiceVolume(){ return GetVolume( m_voiceConfig.volumeParamName ); }
    #endregion // Voice


    #region Snapshot
    /// <summary>
    /// スナップショット変更.
    /// </summary>
    public void TransitionSnapshot( string from, string to, float transitionTime, float fromWeight = 0.0f, float toWeight = 1.0f )
    {
        var fromSnap = m_masterAudioMixer.FindSnapshot( from );
        var toSnap = m_masterAudioMixer.FindSnapshot( to );
        TransitionSnapshot( fromSnap, toSnap, transitionTime, fromWeight, toWeight );
    }
    // 
    public void TransitionSnapshot( AudioMixerSnapshot from, AudioMixerSnapshot to, float transitionTime, float fromWeight = 0.0f, float toWeight = 1.0f )
    {
        var snapshots = new AudioMixerSnapshot[]{ from, to };
        var weights = new float[]{ fromWeight, toWeight };
        m_masterAudioMixer.TransitionToSnapshots( snapshots, weights, transitionTime );
    }
    // 
    public void TransitionSnapshot( string to, float transitionTime )
    {
        var snapshotOpenMenu = m_masterAudioMixer.FindSnapshot( to );
        snapshotOpenMenu.TransitionTo( transitionTime ); 
    }
    #endregion // Snapshot



    /// <summary>
    /// ボリューム(Linear値)を設定.
    /// </summary>
    void SetVolume( string volumeParamName, float volumeRate )
    {
        if( m_masterAudioMixer != null ){
            m_masterAudioMixer.SetFloat( volumeParamName, AudioUtil.VolumeToDb( volumeRate ));
        }
    }

    /// <summary>
    /// ボリューム(Linear値)を取得.
    /// </summary>
    float GetVolume( string volumeParamName )
    {
        if( m_masterAudioMixer != null ){
            float volume;
            m_masterAudioMixer.GetFloat( volumeParamName, out volume );
            return AudioUtil.DbToVolume( volume );
        }
        return 0.0f;
    }

}   // End of class AudioManager.


} // namespace Goisagi.