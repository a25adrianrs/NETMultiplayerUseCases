using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.NetworkVariables
{
    /// <summary>
    /// Manages the color of a Networked object
    /// </summary>
    public class ColorManager : NetworkBehaviour
    {
        // NetworkVariable que sincroniza el color entre el servidor y los clientes.
        NetworkVariable<Color32> m_NetworkedColor = new NetworkVariable<Color32>();

        // Material local del objeto para cambiar su color en la escena.
        Material m_Material;

        [SerializeField, Tooltip("The seconds that will elapse between color changes")]
        float m_SecondsBetweenColorChanges;

        // Temporizador que acumula el tiempo desde el último cambio de color.
        float m_ElapsedSecondsSinceLastChange;

        void Awake()
        {
            // Obtiene el material del Renderer para poder actualizar su color.
            m_Material = GetComponent<Renderer>().material;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient)
            {
                /*
                 * Cuando un cliente se une, el valor ya puede estar sincronizado en el servidor.
                 * Llamamos manualmente para que el material local refleje el valor actual.
                 */
                OnClientColorChanged(m_Material.color, m_NetworkedColor.Value);
                m_NetworkedColor.OnValueChanged += OnClientColorChanged; // Se suscribe al evento de cambio de valor.
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsClient)
            {
                // Evita fugas quitando la suscripción cuando el objeto deja de existir.
                m_NetworkedColor.OnValueChanged -= OnClientColorChanged;
            }
        }

        void Update()
        {
            if (!IsSpawned)
            {
                // El objeto de red ya no existe en esta instancia.
                return;
            }
            if (!IsServer)
            {
                /*
                 * Solo el servidor cambia el color en este ejemplo.
                 * Los clientes se limitan a recibir la nueva información.
                 */
                return;
            }

            m_ElapsedSecondsSinceLastChange += Time.deltaTime;

            if (m_ElapsedSecondsSinceLastChange >= m_SecondsBetweenColorChanges)
            {
                m_ElapsedSecondsSinceLastChange = 0;
                OnServerChangeColor();
            }
        }

        void OnServerChangeColor()
        {
            // El servidor actualiza el NetworkVariable; la nueva información se replica a los clientes.
            m_NetworkedColor.Value = MultiplayerUseCasesUtilities.GetRandomColor();
        }

        void OnClientColorChanged(Color32 previousColor, Color32 newColor)
        {
            // El cliente actualiza el material local cuando el valor de la red cambia.
            m_Material.color = newColor;
        }
    }
}
