using System;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace OMGeeky
{
    public class MainMenu : MonoBehaviour
    {
        private bool visible;
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                panel.SetActive( value );
            }
        }

        public event Action StartGame;
        private Canvas canvas;
        // [SerializeField] public bool IsPauseScreen;
        private GameObject panel;

        private void OnEnable()
        {
            panel = transform.GetChild( 0 ).gameObject;
            Scene scene = SceneManager.GetSceneByName( "MainLevel" );
            if ( !scene.isLoaded )
                SceneManager.LoadScene( "MainLevel" , LoadSceneMode.Additive );

            scene = SceneManager.GetSceneByName( "MainLevel" );


            if ( canvas == null )
                canvas = GetComponentInParent<Canvas>();
        }

        public void StartNewGame() { StartGame?.Invoke(); }

        public void Options() { throw new NotImplementedException(); }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
