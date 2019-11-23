using UnityEngine;

using Goisagi;

/// <summary>
/// サンプルアプリクラス.
/// </summary>
public class SampleApp : MonoBehaviour
{
    // 各カテゴリごとのオーディオ制御クラス.
    AudioController m_bgmCtrl = null;
    AudioController m_seCtrl = null;
    AudioController m_voiceCtrl = null;

    // 各カテゴリごとのオーディオ制御用ハンドル.
    int m_handleBgm = AudioExtensions.EmptyAudioHandle;
    int m_handleSE = AudioExtensions.EmptyAudioHandle;
    int m_handleVoice = AudioExtensions.EmptyAudioHandle;


    /// <summary>
    /// .
    /// </summary>
    void Start()
    {
        // 各カテゴリのAudioResourceを読込.
        AudioResource dataBgm = Resources.Load<AudioResource>( "bgm" );
        AudioResource dataSE = Resources.Load<AudioResource>( "se" );
        AudioResource dataVoice = Resources.Load<AudioResource>( "voice" );

        // 読込んだAudioResourceをAudioManagerへ接続して各AudioControllerを取得.
        m_bgmCtrl = AudioManager.I.AttachBgmData( dataBgm );
        m_seCtrl = AudioManager.I.AttachSEData( dataSE );
        m_voiceCtrl = AudioManager.I.AttachVoiceData( dataVoice );
    }

    /// <summary>
    /// .
    /// </summary>
    void OnDestroy()
    {
        // AudioResourceの解放.
        AudioManager.I.DetachBgmData( ref m_bgmCtrl );
        AudioManager.I.DetachSEData( ref m_seCtrl );
        AudioManager.I.DetachVoiceData( ref m_voiceCtrl );
    }

    /// <summary>
    /// .
    /// </summary>
    void OnGUI()
    {
        using( var scope = new GUILayout.HorizontalScope())
        {
            // BGM
            using( var vs = new GUILayout.VerticalScope())
            {
                if( m_bgmCtrl.IsPlaying( m_handleBgm ))
                {
                    if( GUILayout.Button( "Pause BGM" )){
                        m_bgmCtrl.Pause( m_handleBgm );
                    }
                    if( GUILayout.Button( "Stop BGM" )){
                        m_bgmCtrl.Stop( m_handleBgm );
                    }
                }
                else if( m_bgmCtrl.IsPaused( m_handleBgm ))
                {
                    if( GUILayout.Button( "Unpause BGM" )){
                        m_bgmCtrl.Unpause( m_handleBgm );
                    }
                }
                else
                {
                    if( GUILayout.Button( "Play BGM" )){
                        m_handleBgm = m_bgmCtrl.Play( clipIndex:0, false, 10f );
                    }
                }
            }

            // SE
            using( var vs = new GUILayout.VerticalScope())
            {
                if( GUILayout.Button( "Play SE" )){
                    // 0番目のSEを再生する.
                    m_handleSE = m_seCtrl.Play( clipIndex:0, false, 0.0f, 1.0f, ()=>{ Debug.Log( "Ended." );} );
                }
                else if( m_seCtrl.IsPlaying( m_handleSE ))
                {
                    if( GUILayout.Button( "Stop SE" )){
                        m_seCtrl.Stop( m_handleSE );
                    }
                }
            }

            // ボイス.
            using( var vs = new GUILayout.VerticalScope())
            {
                if( GUILayout.Button( "Play Voice" )){
                    m_handleVoice = m_voiceCtrl.Play( clipIndex:0, false, 0.0f, 1.0f ); // 0番目のボイスを再生する.
                }
                else if( m_voiceCtrl.IsPlaying( m_handleVoice ))
                {
                    if( GUILayout.Button( "Stop Voice" )){
                        m_voiceCtrl.Stop( m_handleVoice );
                    }
                }
            }

            // ボリューム調整.
            using( var vs = new GUILayout.VerticalScope())
            {
                float volume;

                using( var hs = new GUILayout.HorizontalScope()){
                    volume = AudioManager.I.GetMasterVolume();
                    GUILayout.Label( string.Format( "Master Volume {0:0.00}", volume ), GUILayout.MaxWidth( Screen.width * 0.15f ));
                    if( GUILayout.RepeatButton( "<" )){ AudioManager.I.SetMasterVolume( volume - 0.005f ); }
                    if( GUILayout.RepeatButton( ">" )){ AudioManager.I.SetMasterVolume( volume + 0.005f ); }
                }

                using( var hs = new GUILayout.HorizontalScope()){
                    volume = AudioManager.I.GetBgmVolume();
                    GUILayout.Label( string.Format( "Bgm Volume {0:0.00}", volume ), GUILayout.MaxWidth( Screen.width * 0.15f ));
                    if( GUILayout.RepeatButton( "<" )){ AudioManager.I.SetBgmVolume( volume - 0.005f ); }
                    if( GUILayout.RepeatButton( ">" )){ AudioManager.I.SetBgmVolume( volume + 0.005f ); }
                }

                using( var hs = new GUILayout.HorizontalScope()){
                    volume = AudioManager.I.GetSEVolume();
                    GUILayout.Label( string.Format( "SE Volume {0:0.00}", volume ), GUILayout.MaxWidth( Screen.width * 0.15f ));
                    if( GUILayout.RepeatButton( "<" )){ AudioManager.I.SetSEVolume( volume - 0.005f ); }
                    if( GUILayout.RepeatButton( ">" )){ AudioManager.I.SetSEVolume( volume + 0.005f ); }
                }

                using( var hs = new GUILayout.HorizontalScope()){
                    volume = AudioManager.I.GetVoiceVolume();
                    GUILayout.Label( string.Format( "Voice Volume {0:0.00}", volume ), GUILayout.MaxWidth( Screen.width * 0.15f ));
                    if( GUILayout.RepeatButton( "<" )){ AudioManager.I.SetVoiceVolume( volume - 0.005f ); }
                    if( GUILayout.RepeatButton( ">" )){ AudioManager.I.SetVoiceVolume( volume + 0.005f ); }
                }
            }
            
            // スナップショット変更.
            using( var vs = new GUILayout.VerticalScope())
            {
                GUILayout.Label( "Snapshots" );
                if( GUILayout.Button( "to OPEN_MENU" )){
                    AudioManager.I.TransitionSnapshot( "OPEN_MENU", 2.0f );
                }
                if( GUILayout.Button( "to MASTER" )){
                    AudioManager.I.TransitionSnapshot( "MASTER", 2.0f );
                }                
            }
        }
    }

}   // End of class SampleApp.

