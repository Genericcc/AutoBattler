using System.Collections.Generic;
using Data;
using DOTS.Battle;
using UnityEngine;

namespace UI
{
    public class SquadRecruitmentView : MonoBehaviour
    {
        [SerializeField] 
        private SquadRecruitButton squadRecruitButtonPrefab;
        
        [SerializeField] 
        private Transform buttonContainer;
        
        [SerializeField] 
        private Transform leftButtonContainer;
        
        public void Init(List<BaseSquadData> squadDatas)
        {
            foreach (var squadData in squadDatas)
            {
                var newButton = Instantiate(squadRecruitButtonPrefab, buttonContainer);
                newButton.Init(squadData, TeamType.Blue);
            }
            
            foreach (var squadData in squadDatas)
            {
                var newButton = Instantiate(squadRecruitButtonPrefab, leftButtonContainer);
                newButton.Init(squadData, TeamType.Red);
            }
        }
    }
}