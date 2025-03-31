using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Utilities;

[Serializable]
public class SoundEffect {
    public AudioClip Clip;
    public SoundEffectType Type;
}

[DefaultExecutionOrder(-99)]
public class SoundManager : Singleton<SoundManager> {

    public AudioMixer MainMixer;
    public AudioMixerGroup BGM;
    public AudioMixerGroup SFX;

    protected override void Awake() {
        base.Awake();
        SoundManager.StartSingleton();
    }
}