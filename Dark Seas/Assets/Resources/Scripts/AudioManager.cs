using UnityEngine;

namespace DarkSeas {

    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour {

        public static AudioManager i;

        private AudioSource audioSource;

        [SerializeField] AudioClip menuTheme;

        [SerializeField] AudioClip[] bgm;

        #region Singleton

        private void Awake() {

            if (i != null) {
                Debug.LogError("More than one instance of Audio Manager found.");
                return;
            }

            i = this;

            // load AudioSource.
            audioSource = GetComponent<AudioSource>();

            // load menu theme music
            menuTheme = Resources.Load<AudioClip>("Audio/MenuTheme/menu_theme-01");

            // load welcome back audio files
            bgm = Resources.LoadAll<AudioClip>("Audio/BGM");

        }

        #endregion

        public void PlayMenuTheme() {

            audioSource.Stop();

            audioSource.loop = true;
            audioSource.clip = menuTheme;
            audioSource.Play();
            //audioSource.PlayOneShot(bgm[Random.Range(0, bgm.Length)]);

        }

        public void PlayBGM() {

            audioSource.Stop();

            audioSource.loop = true;
            audioSource.clip = bgm[Random.Range(0, bgm.Length)];
            audioSource.Play();

        }

    }

}