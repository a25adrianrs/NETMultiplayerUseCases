using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Proximity
{
    /// <summary>
    /// Manages the color of a Networked object
    /// </summary>
    public class ColorManager : NetworkBehaviour
    {
        // NetworkVariable que almacena el color sincronizado.
        NetworkVariable<Color32> m_NetworkedColor = new NetworkVariable<Color32>();

        // Material local del objeto para cambiar su color.
        Material m_Material;

        // Componente que detecta la proximidad del jugador.
        ProximityChecker m_ProximityChecker;

        // Acción de interacción del jugador.
        InputAction interactAction;

        void Awake()
        {
            m_Material = GetComponent<Renderer>().material;
            m_ProximityChecker = GetComponent<ProximityChecker>();
        }

        void Start()
        {
            // Obtiene la acción de input que el jugador usará para cambiar el color.
            interactAction = InputSystem.actions.FindAction("Interact");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient)
            {
                /* Cuando un cliente se une, sincroniza el color inicial y suscribe el callback
                 * de proximidad para recibir cambios en el estado local del jugador. */
                OnClientColorChanged(m_Material.color, m_NetworkedColor.Value);
                m_NetworkedColor.OnValueChanged += OnClientColorChanged;
                m_ProximityChecker.AddListener(OnClientLocalPlayerProximityStatusChanged);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsClient)
            {
                // Limpia eventos cuando el objeto deja de existir.
                m_NetworkedColor.OnValueChanged -= OnClientColorChanged;
                m_ProximityChecker.RemoveListener(OnClientLocalPlayerProximityStatusChanged);
            }
        }

        void Update()
        {
            if (!IsClient || !m_ProximityChecker.LocalPlayerIsClose)
            {
                /* Se ejecuta solo en el cliente, y solo cuando el jugador local está lo suficientemente cerca.
                 * Si no cumple esas condiciones, no hace nada. */
                return;
            }

            if (interactAction.WasPressedThisFrame())
            {
                OnClientRequestColorChange();
            }
        }

        void OnClientRequestColorChange()
        {
            // Si el botón de interacción fue presionado, el cliente solicita al servidor el cambio de color.
            ServerChangeColorRpc();
        }

        [Rpc(SendTo.Server)]
        void ServerChangeColorRpc()
        {
            m_NetworkedColor.Value = MultiplayerUseCasesUtilities.GetRandomColor();
        }

        void OnClientColorChanged(Color32 previousColor, Color32 newColor)
        {
            // Aplica el nuevo color localmente cuando la NetworkVariable cambia.
            m_Material.color = newColor;
        }

        void OnClientLocalPlayerProximityStatusChanged(bool isClose)
        {
            Debug.Log($"Local player is now {(isClose ? "close" : "far")}");
        }
    }
}
