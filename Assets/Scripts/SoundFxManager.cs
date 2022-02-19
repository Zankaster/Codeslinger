using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(List<AudioClip>))]
public class SoundFxManager : MonoBehaviour
{
    public List<SoundClip> sounds;
    public AudioSource audioSource;
    public Queue<AudioClip> soundsToPlay;

    public void Awake() {
        soundsToPlay = new Queue<AudioClip>();
    }

    public void PlayFx(SoundType soundType) {
        switch (soundType) {
            case SoundType.dataCollected:
                soundsToPlay.Enqueue( sounds.Where(x=>x.name =="DataPick").Select(y=>y.audioClip).FirstOrDefault());
                break;
            case SoundType.powerLost:
                soundsToPlay.Enqueue(sounds.Where(x => x.name == "Hit").Select(y => y.audioClip).FirstOrDefault());
                break;
            case SoundType.death:
                break;
            case SoundType.selection1:
                soundsToPlay.Enqueue(sounds.Where(x => x.name == "Selection1").Select(y => y.audioClip).FirstOrDefault());
                break;
            case SoundType.selection2:
                soundsToPlay.Enqueue(sounds.Where(x => x.name == "Selection2").Select(y => y.audioClip).FirstOrDefault());
                break;
            case SoundType.selection3:
                soundsToPlay.Enqueue(sounds.Where(x => x.name == "Selection3").Select(y => y.audioClip).FirstOrDefault());
                break;
            case SoundType.selectionFailed1:
                soundsToPlay.Enqueue(sounds.Where(x => x.name == "SelectionFailed1").Select(y => y.audioClip).FirstOrDefault());
                break;
            case SoundType.buy1:
                soundsToPlay.Enqueue(sounds.Where(x => x.name == "Buy1").Select(y => y.audioClip).FirstOrDefault());
                break;
        }
    }

    public IEnumerator WaitForSound(AudioClip sound) {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        audioSource.clip = sound;
        audioSource.Play();
    }

    private void Update() {
        if(!audioSource.isPlaying && soundsToPlay.Count > 0) {
            audioSource.clip = soundsToPlay.Dequeue();
            audioSource.Play();
        }
    }
}