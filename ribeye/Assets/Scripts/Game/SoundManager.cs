using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    #region Declares

    //Declare publics
    [SerializeField] private SoundClass[] soundObjects;

    #endregion

    #region Classes and structures

    [System.Serializable]
    private class SoundClass {
        //Declare
        public GameObject SoundPrefab = null;
        public AudioClip[] TheAudioClips = null;
        [Range(1, 200)]
        public int NumberOfObjects = 30;
        
        [System.NonSerialized] public int AudioSourceIndex = 0;
        [System.NonSerialized] public Transform[] SoundTransforms;
        [System.NonSerialized] public AudioSource[] AudioSources;
        [System.NonSerialized] public Transform[] FollowTransforms;
    }

    #endregion

    #region Start and initialization

    private void Awake() {
        //Loop
        for (int i = 0; i < soundObjects.Length; i++) {
            //Expand
            soundObjects[i].SoundTransforms = new Transform[soundObjects[i].NumberOfObjects];
            soundObjects[i].AudioSources = new AudioSource[soundObjects[i].NumberOfObjects];
            soundObjects[i].FollowTransforms = new Transform[soundObjects[i].NumberOfObjects];
            //Loop
            for (int j = 0; j < soundObjects[i].NumberOfObjects; j++) {
                //Create
                GameObject objSound = Instantiate(soundObjects[i].SoundPrefab, transform, true);
                //Set transform
                soundObjects[i].SoundTransforms[j] = objSound.transform;
                //Set audio source
                soundObjects[i].AudioSources[j] = objSound.GetComponent<AudioSource>();
            }
        }
    }

    #endregion

    #region Update

    private void Update() {
        //Loop
        for (int i = 0; i < soundObjects.Length; i++) {
            //Loop
            for (int j = 0; j < soundObjects[i].NumberOfObjects; j++) {
                //Check if need to follow
                if (soundObjects[i].FollowTransforms[j] != null) {
                    //Follow the transform
                    soundObjects[i].SoundTransforms[j].position = soundObjects[i].FollowTransforms[j].position;
                }
            }
        }
    }

    #endregion

    #region Other functions

    internal void PlaySound(int soundObjectIndex, Vector3 playAtLocation, Transform followTransform = null, float volume = 1f) {
        //Check if not playing sound
        if (!soundObjects[soundObjectIndex].AudioSources[soundObjects[soundObjectIndex].AudioSourceIndex].isPlaying) {
            //Change clip
            soundObjects[soundObjectIndex].AudioSources[soundObjects[soundObjectIndex].AudioSourceIndex].clip =
                soundObjects[soundObjectIndex].TheAudioClips[Random.Range(0, soundObjects[soundObjectIndex].TheAudioClips.Length)];
            //Move transform
            soundObjects[soundObjectIndex].SoundTransforms[soundObjects[soundObjectIndex].AudioSourceIndex].transform.position = playAtLocation;
            //Set
            soundObjects[soundObjectIndex].FollowTransforms[soundObjects[soundObjectIndex].AudioSourceIndex] = followTransform;
            //Set volume
            soundObjects[soundObjectIndex].AudioSources[soundObjects[soundObjectIndex].AudioSourceIndex].volume = volume;
            //Play sound
            soundObjects[soundObjectIndex].AudioSources[soundObjects[soundObjectIndex].AudioSourceIndex].Play();
            //Increase index
            soundObjects[soundObjectIndex].AudioSourceIndex = (soundObjects[soundObjectIndex].AudioSourceIndex + 1) % soundObjects[soundObjectIndex].NumberOfObjects;
        }
    }

    #endregion

}