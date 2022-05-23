using System.Collections;
using System.Management.Instrumentation;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace OMGeeky
{
    public class PauseMenuCaller : MonoBehaviour
    {
        private Scene mainMenuScene;
        private Canvas canvas;
        private MainMenu menu;
        private bool paused;
        public bool Paused
        {
            get => paused;
            set
            {
                paused = value;
                if ( canvas != null )
                    canvas.enabled = paused;

                Time.timeScale = value ? 0 : 1;
            }
        }

        private IEnumerator Start()
        {
            mainMenuScene = SceneManager.GetSceneByName( "MainMenu" );
            yield return new WaitUntil( () =>
            {
                if ( mainMenuScene == null || mainMenuScene.buildIndex == -1)
                {
                    mainMenuScene = SceneManager.GetSceneByName( "MainMenu" );
                    if ( mainMenuScene == null || mainMenuScene.buildIndex == -1)
                        SceneManager.LoadScene( "MainMenu" , LoadSceneMode.Additive );

                    return false;
                }

                return mainMenuScene.IsValid() && mainMenuScene.isLoaded;
            } );

            //
            // if ( mainMenuScene.isLoaded || !mainMenuScene.IsValid() )
            // {
            //     yield return new WaitUntil( () =>
            //     {
            //         return !mainMenuScene.IsValid()&& mainMenuScene.isLoaded;
            //     } );
            //
            // }

            mainMenuScene = SceneManager.GetSceneByName( "MainMenu" );

            var rootGameObjects = mainMenuScene.GetRootGameObjects();

            // canvas = FindObjectOfType<Canvas>();
            foreach ( GameObject rootGameObject in rootGameObjects )
            {
                if ( !rootGameObject.TryGetComponent( out canvas ) )
                    canvas = rootGameObject.GetComponentInChildren<Canvas>();

                if ( canvas != null )
                    break;
            }

            if ( canvas == null )
                Debug.LogError( $"Could not find an instance of {nameof(Canvas)}" );

            menu = canvas.GetComponentInChildren<MainMenu>();
            menu.IsPauseScreen = true;
            menu.StartGame -= menuOnStartGame;
            menu.StartGame += menuOnStartGame;
            Paused = true;
        }

        private void menuOnStartGame() { Paused = false; }

        void Update()
        {
            if ( Input.GetButtonDown( "Pause" ) )
            {
                Paused = !canvas.enabled;
            }
        }
    }
}
