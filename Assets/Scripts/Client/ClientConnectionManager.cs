using Common;

using TMPro;

using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client
{
    public class ClientConnectionManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _addressField;
        [SerializeField] private TMP_InputField _portField;
        [SerializeField] private TMP_Dropdown _connectionModeDropdown;
        [SerializeField] private TMP_Dropdown _teamDropdown;
        [SerializeField] private Button _connectButton;
        
        private ushort Port => ushort.Parse(_portField.text);
        private string Address => _addressField.text;

        private void OnEnable()
        {
            _connectionModeDropdown.onValueChanged.AddListener(OnConnectionModeChanged);
            _connectButton.onClick.AddListener(OnButtonConnect);
            OnConnectionModeChanged(_connectionModeDropdown.value);
        }

        private void OnDisable()
        {
            _connectionModeDropdown.onValueChanged.RemoveAllListeners();
            _connectButton.onClick.RemoveAllListeners();
        }

        private void OnConnectionModeChanged(int connectionMode)
        {
            string buttonLabel;
            _connectButton.enabled = true;
            
            switch (connectionMode)
            {
                case 0:
                    buttonLabel = "Start Host";
                    break;
                case 1 : 
                    buttonLabel = "Start Server";
                    break;
                case 2:
                    buttonLabel = "Start Client";
                    break;
                default:
                    buttonLabel = "<ERROR>";
                    _connectButton.enabled = false;
                    break;
            }

            var buttonText = _connectButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = buttonLabel;
        }

        private void OnButtonConnect()
        {
            DestroyLocalSimulationWorld();
            SceneManager.LoadScene(1);
            
            switch (_connectionModeDropdown.value)
            {
                case 0:
                    StartServer();
                    StartClient();
                    break;
                case 1:
                    StartServer();
                    break;
                case 2:
                    StartClient();
                    break;
                default:
                    Debug.LogError("Error: Unknown connection mode", gameObject);
                    break;
            }
        }

        private static void DestroyLocalSimulationWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }
        }

        private void StartServer()
        {
            var serverWorld = ClientServerBootstrap.CreateServerWorld("Turbo Server World");
            var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);

            //we want to get NetworkStreamDriver, so it can Listen for new network connections on given endpoint 
            //
            //using makes sure it gets disposed after getting out of scope (?)
            //Query is the recommended approach, even for signletons as GetSingleton throws an error if there are more matching entities
            {
                using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                var networkStreamDriver = networkDriverQuery.GetSingletonRW<NetworkStreamDriver>();
                networkStreamDriver.ValueRW.Listen(serverEndpoint);
            }
        }

        //On creating Client's world we want to hook it up to NetworkStreamDriver
        private void StartClient()
        {
            var clientWorld = ClientServerBootstrap.CreateClientWorld("Turbo Client World");
            //Parse is a method that takes adress and port and spits out NetworkEndpoint
            var connectionEndpoint = NetworkEndpoint.Parse(Address, Port);

            {
                using var networkEntityQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                var networkStreamDriver = networkEntityQuery.GetSingletonRW<NetworkStreamDriver>();
                networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);
            }
            
            World.DefaultGameObjectInjectionWorld = clientWorld;
            
            var team = _teamDropdown.value switch
            {
                0 => TeamType.AutoAssign,
                1 => TeamType.Blue,
                2 => TeamType.Red,
                _ => TeamType.None,
            };

            var teamRequestEntity = clientWorld.EntityManager.CreateEntity();
            clientWorld.EntityManager.AddComponentData(teamRequestEntity, new ClientTeamRequest
            {
                Value = team,
            });
        }
    }
}