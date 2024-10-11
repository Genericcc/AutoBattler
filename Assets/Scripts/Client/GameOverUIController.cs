using System.Collections;
using Common;
using TMPro;

using Unity.Entities;
using Unity.NetCode;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client
{
    public class GameOverUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private Button _returnToMainMenuButton;
        [SerializeField] private Button _rageQuitButton;
        
        private EntityQuery _networkConnectionQuery;
        
        private void OnEnable()
        {
            _returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
            _rageQuitButton.onClick.AddListener(RageQuit);

            if (World.DefaultGameObjectInjectionWorld == null) return;
            _networkConnectionQuery = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateEntityQuery(typeof(NetworkStreamConnection));

            var startGameSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
            if (startGameSystem != null)
            {
                startGameSystem.OnGameOver += ShowGameOverUI;
            }
        }

        private void OnDisable()
        {
            _returnToMainMenuButton.onClick.RemoveAllListeners();
            _rageQuitButton.onClick.RemoveAllListeners();
            
            if (World.DefaultGameObjectInjectionWorld == null) return;
            var startGameSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
            if (startGameSystem != null)
            {
                startGameSystem.OnGameOver -= ShowGameOverUI;
            }
        }

        private void ShowGameOverUI(TeamType winningTeam)
        {
            _gameOverPanel.gameObject.SetActive(true);
            _gameOverText.text = $"{winningTeam} Team Wins!";
        }
        
        private void ReturnToMainMenu()
        {
            if (_networkConnectionQuery.TryGetSingletonEntity<NetworkStreamConnection>(out var networkConnectionEntity))
            {
                World.DefaultGameObjectInjectionWorld.EntityManager
                    .AddComponent<NetworkStreamRequestDisconnect>(networkConnectionEntity);
            }
            
            World.DisposeAllWorlds();
            SceneManager.LoadScene(0);
        }

        private void RageQuit()
        {
            Application.Quit();
        }
    }
}