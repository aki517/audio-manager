using UnityEngine;
using UnityEngine.Audio;

namespace Goisagi
{

/// <summary>
/// 音源の状態制御を行うクラス.
/// </summary>
public class AudioObject
{
    AudioSource m_audioSource;
    Transform m_sourceTrans;
    Transform m_trackingTrans;
    System.Action m_onPlayEnded;
    float m_fadeVolume;
    float m_fadeEndTime;
    float m_fadeTimer;
    bool m_isTracking;

    public AudioState State{ get; private set; }
    public int Index{ get; private set; }
    public int Handle{ get; private set; }
    

    /// <summary>
    /// .
    /// </summary>
    public AudioObject( int _index, AudioSource _audioSource )
    {
        Index = _index;
        m_audioSource = _audioSource;
        m_sourceTrans = m_audioSource.transform;
        Reset();
    }

    /// <summary>
    /// 更新処理.
    /// <return>再生中ならtrueを返す.</return>
    /// </summary>
    public bool OnUpdate( float deltaTime )
    {
        if( State != AudioState.None && m_isTracking ){
            m_sourceTrans.localPosition = m_trackingTrans.localPosition;
        }

        switch( State )
        {
            case AudioState.FadeIn:
            {
                m_fadeTimer += deltaTime;
                float t = Mathf.Min( 1.0f, (m_fadeTimer / m_fadeEndTime));
                float volume = Mathf.Lerp( 0.0f, m_fadeVolume, t );
                m_audioSource.volume = volume;

                if( t >= 1.0f ){
                    ChangeState( AudioState.Playing );
                }
            }
            break;

            case AudioState.Playing:
            {
                if( !m_audioSource.isPlaying ){
                    ChangeState( AudioState.None );
                }
            }
            break;
            
            case AudioState.FadeOut:
            {
                m_fadeTimer += deltaTime;
                float t = Mathf.Min( 1.0f, (m_fadeTimer / m_fadeEndTime));
                float volume = Mathf.Lerp( m_fadeVolume, 0.0f, t );
                m_audioSource.volume = volume;

                if( t >= 1.0f ){
                    m_audioSource.Stop();
                    ChangeState( AudioState.None );
                }
            }
            break;
        }

        bool isPlaying = (State != AudioState.None);
        if( !isPlaying && m_onPlayEnded != null ){
            m_onPlayEnded();
            m_onPlayEnded = null;
        }

        return isPlaying;
    }

    /// <summary>
    /// 再生.
    /// </summary>
    public void Play(   AudioClip clip,
                        AudioMixerGroup group,
                        int priority,
                        int audioHandle,
                        Transform targetTrans, 
                        bool isLoop, 
                        float fadeInTime, 
                        float volume,
                        bool isTracking,
                        System.Action onPlayEnded )
    {
        float startVolume;
        if( fadeInTime > 0.0f )
        {
            ChangeState( AudioState.FadeIn );
            m_fadeEndTime = fadeInTime;
            m_fadeTimer = 0.0f;
            startVolume = 0.0f;
            m_fadeVolume = volume;
        }
        else{
            ChangeState( AudioState.Playing );
            startVolume = volume;
        }

        float spatialBlend = 0.0f;
        if( targetTrans != null ){
            spatialBlend = 1.0f;
            m_sourceTrans.localPosition = targetTrans.localPosition;
            m_trackingTrans = targetTrans;
        }else{
            m_sourceTrans.localPosition = Vector3.zero;
            m_trackingTrans = null;
        }

        AudioSource source = m_audioSource;
        source.outputAudioMixerGroup = group;
        source.priority = priority;
        source.volume = startVolume;
        source.spatialBlend = spatialBlend;
        source.clip = clip;
        source.loop = isLoop;
        source.Play();

        Handle = audioHandle;

        m_onPlayEnded = onPlayEnded;
        m_isTracking = isTracking;
    }

    /// <summary>
    /// 停止.
    /// </summary>
    public void Stop( float fadeOutTime )
    {
        if( m_audioSource.isPlaying )
        {
            if( fadeOutTime > 0.0f )
            {
                m_fadeEndTime = fadeOutTime;
                m_fadeVolume = m_audioSource.volume;
                m_fadeTimer = 0.0f;
                ChangeState( AudioState.FadeOut );
            }
            else{
                m_audioSource.Stop();
                ChangeState( AudioState.None );
            }
        }
        else{
            ChangeState( AudioState.None );
        }
    }

    /// <summary>
    /// ポーズ.
    /// </summary>
    public void Pause()
    {
        if( m_audioSource.isPlaying ){
            m_audioSource.Pause();
            ChangeState( AudioState.Pause );
        }
    }

    /// <summary>
    /// ポーズを解除.
    /// </summary>
    public void Unpause()
    {
        m_audioSource.Play();
        ChangeState( AudioState.Playing );
    }

    /// <summary>
    /// パラメータをリセット.
    /// </summary>
    public void Reset()
    {
        ChangeState( AudioState.None );
        Handle = AudioExtensions.EmptyAudioHandle;
        m_trackingTrans = null;
        m_isTracking = false;
        m_onPlayEnded = null;

        if( m_audioSource != null )
        {
            if( m_audioSource.isPlaying ){
                m_audioSource.Stop();
            }
            m_audioSource.clip = null;
        }
    }

    /// <summary>
    /// ステート変更.
    /// </summary>
    private void ChangeState( AudioState nextState )
    {
        //AudioState nowState = State;
        //if( nowState != AudioState.None ){}
        State = nextState;
    }


}   // End of class AudioObject.


// AudioObjectの状態制御用.
public enum AudioState
{
    None,
    FadeIn,
    Playing,
    FadeOut,
    Pause,
}


} // namespace Goisagi.

