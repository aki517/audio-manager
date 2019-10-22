using UnityEngine;

namespace Goisagi
{

/// <summary>
/// オーディオ制御クラス.
/// </summary>
public class AudioController
{
    private AudioPlayer m_owner;
    public int DataHandle{ get; private set; }


    public AudioController( int dataHandle, AudioPlayer owner )
    {
        DataHandle = dataHandle;
        m_owner = owner;
    }

    public int Play( int clipIndex, bool isLoop = false, float fadeInTime = 0.0f, float volume = 1.0f )
    {
        return m_owner.Play( DataHandle, clipIndex, isLoop, fadeInTime, volume, null );
    }

    public int Play3D( int clipIndex, Transform targetTrans, bool isTracking, bool isLoop = false, float fadeInTime = 0.0f, float volume = 1.0f )
    {
        return m_owner.Play( DataHandle, clipIndex, isLoop, fadeInTime, volume, targetTrans, isTracking );
    }

    public void Stop( int audioHandle, float fadeOutTime = 0.0f ){ m_owner.Stop( audioHandle, fadeOutTime ); }
    public void StopAll( float fadeOutTime ){ m_owner.StopAll( fadeOutTime ); }

    public void Pause( int audioHandle ){ m_owner.Pause( audioHandle ); }
    public void PauseAll(){ m_owner.PauseAll(); }

    public void Unpause( int audioHandle ){ m_owner.Unpause( audioHandle ); }
    public void UnpauseAll(){ m_owner.UnpauseAll(); }

    public AudioState GetAudioState( int audioHandle ){ return m_owner.GetAudioState( audioHandle ); }
    public bool IsPlaying( int audioHandle ){ return m_owner.IsPlaying( audioHandle ); }
    public bool IsPaused( int audioHandle ){ return m_owner.IsPaused( audioHandle ); }

    public int NumClip{ get{ return m_owner.GetNumClip( DataHandle ); } }

}   // End of class AudioController.

} // namespace Goisagi.