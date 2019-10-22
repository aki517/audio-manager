using UnityEngine;

namespace Goisagi
{

/// <summary>
/// .
/// </summary>
public class AudioUtil
{
    private const float MAX_DB = 0f;
    private const float MIN_DB = -80f;

    /// <summary>
    /// デシベル -> ボリューム 変換.
    /// </summary>
    public static float DbToVolume(float dB)
    {
        if( dB > MIN_DB ){
            return Mathf.Clamp01( Mathf.Pow( 10f, dB * 0.05f ));
        }
        return 0.0f;
    }

    /// <summary>
    /// ボリューム -> デシベル 変換.
    /// </summary>
    public static float VolumeToDb(float linear)
    {
        if( linear > 0f ){
            return Mathf.Clamp( 20f * Mathf.Log10( linear ), MIN_DB, MAX_DB);
        }
        return MIN_DB;
    }

}   // End of class AudioUtil.


/// <summary>
/// .
/// </summary>
static class AudioExtensions
{
    public const int EmptyAudioHandle = -1;

    public static bool IsEmptyAudioHandle( this int val ){ return (val == EmptyAudioHandle); }

}   // End of class AudioExtensions.


}   // namespace Goisagi.

