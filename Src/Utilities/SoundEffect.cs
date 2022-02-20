using System;
using System.Collections.Generic;
using Stargate.Domain;
using UniLinq;
using UnityEngine;

namespace Stargate.Utilities
{
    public class SoundEffect
    {
        public float volume { get; set; } = GameSettings.SHIP_VOLUME;
        public bool isLoop { get; set; } = false;

        private const string path = "Blaarkies/Stargate/sound/";
        private const string logCategory = "audio-setup";

        private readonly List<AudioSource> _audioSources;
        private readonly Cycler _fileCycler;

        public SoundEffect(SoundPack soundPack, GameObject gameObject)
        {
            isLoop = soundPack.IsLoop;
            _audioSources = Enumerable.Range(1, soundPack.Count)
                .Select(index => GetSoundFileAudioSource($"{soundPack.BaseName}{index}", gameObject))
                .ToList();
            _fileCycler = new Cycler(soundPack.Count);
        }

        private AudioSource GetSoundFileAudioSource(string fileName, GameObject gameObject)
        {
            var filePath = path + fileName;

            if (!GameDatabase.Instance.ExistsAudioClip(filePath))
            {
                BlaarkiesLog.Debug($"Could not find file in DB [{fileName}]", logCategory);
                throw new Exception($"Could not find file in DB [{fileName}]");
            }

            var audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = GameDatabase.Instance.GetAudioClip(filePath);
            audioSource.dopplerLevel = 0f;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 0.5f;
            audioSource.maxDistance = 10f;
            return audioSource;
        }

        // TODO: add delay
        public void Play(float delay = 0f)
        {
            var previousSource = _audioSources[_fileCycler.Index];
            previousSource.Stop();
            _fileCycler.Next();

            var source = _audioSources[_fileCycler.Index];
            source.loop = isLoop;
            source.volume = volume;
            source.time = 0f;

            Debug.Log($"Cycler for {_audioSources[0].clip.name} at index {_fileCycler.Index} of {_audioSources.Count} to play");
            source.Play();
        }

        public void Stop()
        {
            _audioSources[_fileCycler.Index].Stop();
        }
    }
}
