using TMPro;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
    /// <summary>
    /// Manages the UI of the "NetworkVariable vs RPCs" scene
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        // Etiqueta que se muestra antes de que el jugador se conecte.
        [SerializeField] TMP_Text startupLabel;

        // Etiqueta con instrucciones de controles que se muestra cuando ya está conectado.
        [SerializeField] TMP_Text controlsLabel;

        void Start()
        {
            // Muestra u oculta etiquetas según el estado de conexión inicial.
            RefreshLabels(NetworkManager.Singleton && (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer));
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }

        void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            }
        }

        void OnServerStarted()
        {
            // Se llama cuando el servidor ha comenzado correctamente.
            RefreshLabels(true);
        }

        void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
            {
                if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
                {
                    return; // you don't want to do actions twice when jugando como host
                }
                RefreshLabels(true);
            }
            else if (connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
            {
                RefreshLabels(false);
            }
        }

        void RefreshLabels(bool isConnected)
        {
            // Muestra la etiqueta de inicio si no hay conexión y los controles cuando sí hay conexión.
            startupLabel.gameObject.SetActive(!isConnected);
            controlsLabel.gameObject.SetActive(isConnected);
        }
    }
}
