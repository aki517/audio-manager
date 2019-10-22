using System;
using UnityEngine;

/// <summary>
/// .
/// </summary>
[Serializable]
public class AudioConfig
{
    [SerializeField, TooltipAttribute("登録可能なAudioResourceの最大数")]
    public int resourceMax = 1;

    [SerializeField, TooltipAttribute("AudioPoolの確保数")]
    public int audioPoolSize = 1;

    [SerializeField, TooltipAttribute("同時発音数の上限")]
    public int simultaneousPlayMax = 1;

    [SerializeField, TooltipAttribute("AudioMixerからExposeしたボリュームパラメータ名")]
    public string volumeParamName = string.Empty;

}   // End of class AudioConfig.
