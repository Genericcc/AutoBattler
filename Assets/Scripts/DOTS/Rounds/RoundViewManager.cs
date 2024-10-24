using System.Globalization;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace DOTS.Rounds
{
    public class RoundViewManager : MonoBehaviour
    {
        // [SerializeField] 
        // private GameObject beginGamePanel;
        
        [SerializeField] 
        private GameObject countdownPanel;
        
        [SerializeField] 
        private TextMeshProUGUI countdownText;
        
        // [SerializeField] 
        // private Button startGameButton;
        
        private void OnEnable()
        {
            countdownPanel.SetActive(true);
            //startGameButton.onClick.AddListener(StartGame);

            if (World.DefaultGameObjectInjectionWorld == null)
            {
                return;
            }

            var countdownSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CountdownToGameStartSystem>();
            if (countdownSystem != null)
            {
                countdownSystem.OnUpdateCountdownText += UpdateCountdownText;
            }
        }
        
        private void OnDisable()
        {
            //startGameButton.onClick.RemoveAllListeners();
            
            if (World.DefaultGameObjectInjectionWorld == null) return;
            
            var countdownSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CountdownToGameStartSystem>();
            if (countdownSystem != null)
            {
                countdownSystem.OnUpdateCountdownText -= UpdateCountdownText;
            }
        }
        
        private void StartGame()
        {
            
        }
        
        private void UpdateCountdownText(float countdownTime)
        {
            countdownText.text = countdownTime.ToString(CultureInfo.CurrentCulture);
        }
    }
}