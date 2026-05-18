using UnityEngine;
using UnityEngine.InputSystem;
namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
    /// <summary>
    /// Gestor genérico que administra el ciclo de vida de un jugador
    /// </summary>
    public class PlayerManager : NetworkBehaviour
    {
        /// <summary>
        /// Reference estática al jugador local en ejecución.
        /// </summary>
        /// <remarks> Se usa para saber rápidamente cuál es el jugador local desde otros scripts.</remarks>
        public static PlayerManager s_LocalPlayer;

        // Componente PlayerInput que controla el jugador local.
        [SerializeField]
        PlayerInput inputManager;

        public override void OnNetworkSpawn()
        {
            /* Cuando un objeto de red se crea en el juego, aquí se decide si
             * este objeto pertenece al jugador local o a otro cliente.
             * IsOwner es true solo para el jugador local. */
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                OnLocalPlayerSpawned();
                return;
            }
            OnNonLocalPlayerSpawned();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsOwner)
            {
                OnLocalPlayerDeSpawned();
                return;
            }
            OnNonLocalPlayerDeSpawned();
        }

        void OnNonLocalPlayerSpawned()
        {
            // Los jugadores remotos no deben poder controlar este objeto localmente.
            if (inputManager)
            {
                inputManager.enabled = false;
            }
        }

        void OnLocalPlayerSpawned()
        {
            /* El jugador local guarda una referencia estática y habilita
             * solo sus componentes de entrada para que pueda moverse. */
            s_LocalPlayer = this;
            if (inputManager)
            {
                inputManager.enabled = IsOwner;
            }
        }

        void OnLocalPlayerDeSpawned()
        {
            s_LocalPlayer = null;
        }

        void OnNonLocalPlayerDeSpawned() { }
    }
}
