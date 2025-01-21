using UnityEngine;

namespace BrierleyGames
{
    public class DynamicMenuAudioManager : MonoBehaviour
    {
        //this object accessible from anywhere
        public static DynamicMenuAudioManager _instance;

        //audio source to play sound
        [Header("References")]
        [SerializeField] private AudioSource audioSource;

        //Audio clip lists
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] moveToAudio;
        [SerializeField] private AudioClip[] moveBackAudio;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //ensure there isnt any doubling up of this static object instance
            if (DynamicMenuAudioManager._instance != null && DynamicMenuAudioManager._instance != this)
            {
                Destroy(gameObject);
            }

            _instance = this;

            //Check to make sure audio source is valid
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// Plays a random audio clip from a set list on this object
        /// </summary>
        public void PlayMoveToAudio()
        {
            //Stop audio playing
            if (audioSource.isPlaying) audioSource.Stop();

            //Get Random Clip
            AudioClip clipToPlay = GetRandomClipFromArray(moveToAudio);

            //return if audio is null
            if (clipToPlay == null)
            {
                Debug.LogWarning("No Audio file was found for selected sound type, please ensure you have added sounds to the respective audio clip slots, or set the DynamicMenuCameraTarget's sound type to Silent");
                return;
            }
            //Set Audio
            audioSource.clip = clipToPlay;

            //Play Audio
            audioSource.Play();
        }

        /// <summary>
        /// PLays a random audio clip from a set list on this object
        /// </summary>
        public void PlayMoveBackAudio()
        {
            //Stop audio playing
            if (audioSource.isPlaying) audioSource.Stop();

            //Get Random Clip
            AudioClip clipToPlay = GetRandomClipFromArray(moveBackAudio);

            //return if audio is null
            if (clipToPlay == null) return;

            //Set Audio
            audioSource.clip = clipToPlay;

            //Play Audio
            audioSource.Play();
        }

        /// <summary>
        /// Pass in a list of clips and play and random clip from the list
        /// </summary>
        /// <param name="listOfClips">List of clips to choose random audio from</param>
        public void PlayAudio(AudioClip[] listOfClips)
        {
            //Stop audio playing
            if (audioSource.isPlaying) audioSource.Stop();

            //Get Random Clip
            AudioClip clipToPlay = GetRandomClipFromArray(listOfClips);

            //return if audio is null
            if (clipToPlay == null) return;

            //Set Audio
            audioSource.clip = clipToPlay;

            //Play Audio
            audioSource.Play();
        }

        /// <summary>
        /// Gets a list of audioclips and returns a random clip from the list
        /// </summary>
        /// <param name="listOfClips"></param>
        /// <returns></returns>
        public AudioClip GetRandomClipFromArray(AudioClip[] listOfClips)
        {
            //return null if list is empty
            if (listOfClips.Length < 1)
            {
                return null;
            }

            //return random audio clip from list
            return listOfClips[Random.Range(0, listOfClips.Length)];
        }
    }
}
