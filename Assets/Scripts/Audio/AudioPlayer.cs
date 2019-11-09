using System.Collections.Generic;
using UnityEngine;

namespace  Goisagi
{

/// <summary>
/// AudioResourceとAudioObjectを管理するクラス.
/// </summary>
public class AudioPlayer
{
    const int EMPTY_DATA = -1;

    AudioResource[] m_resourceTable = null;
    AudioObject[]   m_audioObjects = null;

    List<AudioObject>   m_freeList = null;
    List<AudioObject>   m_activeList = null;

    AudioConfig m_config;

    int m_numPlayRequestPerFrame = 0;


    /// <summary>
    /// .
    /// </summary>
    public AudioPlayer( AudioConfig config, Transform parentTrans, string categoryName )
    {
        if( config == null ){
            config = new AudioConfig();
            Debug.LogWarning( "AudioConfig is null, so created dummy config." );
        }
        m_config = config;

        m_resourceTable = new AudioResource[ config.resourceMax ];
        m_audioObjects = new AudioObject[ config.audioPoolSize ];

        int capacity = config.audioPoolSize;
        m_freeList = new List<AudioObject>( capacity );
        m_activeList = new List<AudioObject>( capacity );

        // AudioSourceのプール 作成.
        for( int idx=0 ; idx < config.audioPoolSize ; idx++ )
        {
            GameObject obj = new GameObject( string.Format( "{0}{1:D2}", categoryName, idx ));
            obj.transform.parent = parentTrans;
            
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            m_audioObjects[ idx ] = new AudioObject( idx, audioSource );

            // 空きリストに登録しておく.
            m_freeList.Add( m_audioObjects[ idx ] );
        }
    }

    /// <summary>
    /// 更新処理.
    /// </summary>
    public void OnUpdate( float deltaTime )
    {
        for( int idx=(m_activeList.Count - 1) ; idx >= 0  ; idx-- )
        {
            if( !m_activeList[ idx ].OnUpdate( deltaTime )){
                RemoveActiveAudioObject( idx );
            }
        }

        m_numPlayRequestPerFrame = 0;
    }

    /// <summary>
    /// リソース接続.
    /// </summary>
    public AudioController Attach( AudioResource targetResource )
    {
        if( targetResource == null ){
            return null;
        }

        // AudioClipのチェック.
        AudioClip[] clips = targetResource.clips;
        if( clips == null || clips.Length <= 0 ){
            Debug.LogError( "failed to attach AudioResource!! Not found AudioClip's." );
            return null;
        }

        int dataHandle = EMPTY_DATA;
        for( int idx=0 ; idx < m_resourceTable.Length ; idx++ )
        {
            if( m_resourceTable[ idx ] == null ){
                m_resourceTable[ idx ] = targetResource;
                dataHandle = idx;
                break;
            }
        }

        if( dataHandle == EMPTY_DATA ){
            Debug.LogError( "Failed to attach AudioResource!!" );
            return null;
        }

        return new AudioController( dataHandle, this );
    }

    /// <summary>
    /// 接続解除.
    /// </summary>
    public void Detach( ref AudioController audioInteface )
    {
        if( audioInteface == null ){
            return;
        }

        int dataHandle = audioInteface.DataHandle;
        if( dataHandle == EMPTY_DATA ){
            return;
        }

        for( int idx=(m_activeList.Count - 1) ; idx >= 0  ; idx-- )
        {
            AudioObject audioObj = m_activeList[ idx ];
            int targetDataHandle = ((audioObj.Handle >> BITSHIFT_DATA_HANDLE) & 0xFF);
            if( dataHandle == targetDataHandle ){
                RemoveActiveAudioObject( idx );
            }
        }

        AudioResource targetData = m_resourceTable[ dataHandle ];
        if( targetData != null )
        {
            AudioClip[] clips = targetData.clips;
            for( int idx=0 ; idx < clips.Length ; idx++ ){
                clips[ idx ].UnloadAudioData();
            }
        }
        m_resourceTable[ dataHandle ] = null;

        audioInteface = null;
    }
    
    /// <summary>
    /// 再生.
    /// </summary>
    public int Play( int dataHandle, int clipIndex, bool isLoop = false, float fadeInTime = 0.0f, float volume = 1.0f, Transform trackingTrans = null, bool isTracking = false, System.Action onPlayEnded = null )
    {
        AudioResource resource = m_resourceTable[ dataHandle ]; 

        // 同時発音数制限.
        if( m_numPlayRequestPerFrame >= m_config.simultaneousPlayMax ){
            Debug.LogWarning( "[AudioManager] Over the number of simultaneous play per frame!! " + clipIndex );
            return AudioExtensions.EmptyAudioHandle;
        }

        // 空きがあるか
        int freeIdx = (m_freeList.Count - 1);
        if( freeIdx < 0 ){
            Debug.LogWarning( "[AudioManager] Not found free index!! " + clipIndex );            
            return AudioExtensions.EmptyAudioHandle;
        }
        
        AudioObject audioObj = m_freeList[ freeIdx ];
        m_freeList.RemoveAt( freeIdx );
        m_activeList.Add( audioObj );

        int audioHandle = MakeAudioHandle( audioObj.Index, clipIndex, dataHandle, (m_activeList.Count - 1));

        audioObj.Play(  resource.clips[ clipIndex ], 
                        resource.group, 
                        resource.priority, 
                        audioHandle, 
                        trackingTrans, 
                        isLoop, 
                        fadeInTime, 
                        volume,
                        isTracking,
                        onPlayEnded );

        m_numPlayRequestPerFrame++;

        return audioHandle;
    }

    /// <summary>
    /// 停止.
    /// </summary>
    public void Stop( int audioHandle, float fadeOutTime = 0.0f )
    {
        AudioObject audioObj = GetAudioObject( audioHandle );
        if( audioObj != null ){
            audioObj.Stop( fadeOutTime );
        }
    }

    /// <summary>
    /// 再生中の全AudioObjectを停止.
    /// </summary>
    public void StopAll( float fadeOutTime = 0.0f )
    {
        for( int idx=0 ; idx < m_activeList.Count ; idx++ ){
            m_activeList[ idx ].Stop( fadeOutTime );
        }
    }

    /// <summary>
    /// ポーズ.
    /// </summary>
    public void Pause( int audioHandle )
    {
        AudioObject audioObj = GetAudioObject( audioHandle );
        if( audioObj != null ){
            audioObj.Pause();
        }
    }

    /// <summary>
    /// 再生中の全AudioObjectをポーズ.
    /// </summary>
    public void PauseAll()
    {
        for( int idx=0 ; idx < m_activeList.Count ; idx++ ){
            m_activeList[ idx ].Pause();
        }
    }    

    /// <summary>
    /// ポーズ解除.
    /// </summary>
    public void Unpause( int audioHandle )
    {
        AudioObject audioObj = GetAudioObject( audioHandle );
        if( audioObj != null ){
            audioObj.Unpause();
        }
    }

    /// <summary>
    /// 再生中の全AudioObjectのポーズ解除.
    /// </summary>
    public void UnpauseAll()
    {
        for( int idx=0 ; idx < m_activeList.Count ; idx++ ){
            m_activeList[ idx ].Unpause();
        }
    } 
    
    /// <summary>
    /// 指定audioHandleに該当するAudioObjectを取得.
    /// </summary>
    public AudioObject GetAudioObject( int audioHandle )
    {
        int objIndex = ((audioHandle >> BITSHIFT_OBJ_INDEX) & 0xFF);
        if( objIndex >= 0 && objIndex < m_config.audioPoolSize )
        {
            AudioObject audioObj = m_audioObjects[ objIndex ];
            if( audioObj.Handle == audioHandle ){
                return audioObj;
            }
        }
        return null;
    }

    /// <summary>
    /// ステータスを取得.
    /// </summary>
    public AudioState GetAudioState( int audioHandle )
    {
        AudioObject obj = GetAudioObject( audioHandle );
        return (obj != null ? obj.State : AudioState.None);
    }

    /// <summary>
    /// 再生中？.
    /// </summary>
    public bool IsPlaying( int audioHandle )
    {
        AudioState state = GetAudioState( audioHandle );
        return (state == AudioState.FadeIn || state == AudioState.Playing || state == AudioState.FadeOut);
    }

    /// <summary>
    /// ポーズ中？.
    /// </summary>
    public bool IsPaused( int audioHandle )
    {
        return (GetAudioState( audioHandle ) == AudioState.Pause);
    }


    private const int BITSHIFT_OBJ_INDEX = 24;
    private const int BITSHIFT_CLIP_INDEX = 16;
    private const int BITSHIFT_DATA_HANDLE = 8;
    private const int BITSHIFT_ACTIVE_INDEX = 0;

    /// <summary>
    /// オーディオハンドル作成.
    /// </summary>
    private int MakeAudioHandle( int objIndex, int clipIndex, int dataHandle, int activeIndex )
    {
        return (objIndex << BITSHIFT_OBJ_INDEX | 
                clipIndex << BITSHIFT_CLIP_INDEX | 
                dataHandle << BITSHIFT_DATA_HANDLE | 
                activeIndex << BITSHIFT_ACTIVE_INDEX);
    }

    /// <summary>
    /// 指定targetIndexのアクティブAudioObjectを削除.
    /// </summary>
    void RemoveActiveAudioObject( int targetIndex )
    {
        AudioObject audioObj = m_activeList[ targetIndex ];
        audioObj.Reset();
        m_activeList.RemoveAt( targetIndex );
        m_freeList.Add( audioObj );
    }

    /// <summary>
    /// クリップ数 取得.
    /// </summary>
    public int GetNumClip( int dataHandle )
    {
        if( m_resourceTable[ dataHandle ] ){
            return m_resourceTable[ dataHandle ].clips.Length;
        }
        return 0;
    }

}   // End of class AudioPlayer.

} // namespace Goisagi.
