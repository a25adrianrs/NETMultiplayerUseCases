using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Netcode.Samples.MultiplayerUseCases.NetVarVsRpc
{
    /// <summary>
    /// Manages the color of a Networked object
    /// </summary>
    public class ColorManager : NetworkBehaviour
    {
        // Selección de si este objeto debe usar NetworkVariable o RPCs para sincronizar el color.
        [SerializeField]
        bool m_UseNetworkVariableForColor;

        // NetworkVariable opcional que almacena el color sincronizado.
        NetworkVariable<Color32> m_NetworkedColor = new NetworkVariable<Color32>();

        // Material local para cambiar el color visualmente.
        Material m_Material;

        // Acción de input para detectar el botón de interacción.
        InputAction interactAction;

        void Awake()
        {
            m_Material = GetComponent<Renderer>().material;
        }

        void Start()
        {
            // Obtiene la acción de entrada "Interact" del Input System.
            interactAction = InputSystem.actions.FindAction("Interact");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient)
            {
                if (m_UseNetworkVariableForColor)
                {
                    /*
                     * Cuando un cliente se une, actualiza el material local con el valor actual
                     * de la NetworkVariable para no quedarse desincronizado.
                     */
                    OnClientColorChanged(m_Material.color, m_NetworkedColor.Value);
                    m_NetworkedColor.OnValueChanged += OnClientColorChanged;
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsClient)
            {
                if (m_UseNetworkVariableForColor)
                {
                    m_NetworkedColor.OnValueChanged -= OnClientColorChanged;
                }
            }
        }

        void Update()
        {
            if (!IsClient)
            {
                /* note: this script solo hace lógica en el cliente.
                 * Se detiene temprano para no ejecutar código innecesario en el servidor.
                 */
                return;
            }

            if (interactAction.WasPressedThisFrame())
            {
                OnClientRequestColorChange();
            }
        }

        void OnClientRequestColorChange()
        {
            // El cliente solicita al servidor que cambie el color.
            ServerChangeColorRpc();
        }

        [Rpc(SendTo.Server)]
        void ServerChangeColorRpc()
        {
            Color32 newColor = MultiplayerUseCasesUtilities.GetRandomColor();
            if (m_UseNetworkVariableForColor)
            {
                // Cuando se usa NetworkVariable, el servidor escribe en el valor sincronizado.
                m_NetworkedColor.Value = newColor;
                return;
            }
            // Si no se usa NetworkVariable, el servidor notifica a todos los clientes con un RPC.
            ClientNotifyColorChangedRpc(newColor);
        }

        [Rpc(SendTo.ClientsAndHost)]
        void ClientNotifyColorChangedRpc(Color32 newColor)
        {
            // RPC que actualiza el color en todos los clientes y en el host.
            m_Material.color = newColor;
        }

        void OnClientColorChanged(Color32 previousColor, Color32 newColor)
        {
            // Método que responde a la actualización de NetworkVariable.
            m_Material.color = newColor;
        }
    }
}
