using TMPro;
using Unity.Collections;
using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Netcode.Samples.MultiplayerUseCases.NetworkVariables
{
    /// <summary>
    /// A complex data structure. Can only contain the types listed here: https://docs-multiplayer.unity3d.com/netcode/current/basics/networkvariable/index.html#supported-types
    /// </summary>
    struct SyncableCustomData : INetworkSerializable
    {
        // Valores que queremos mantener sincronizados en la red.
        public int Health;

        // Versión de string con asignación fija que es más eficiente para el netcode.
        public FixedString128Bytes Username;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Health);
            serializer.SerializeValue(ref Username);
        }
    }

    /// <summary>
    /// Manages the data of a character
    /// </summary>
    public class CharacterManager : NetworkBehaviour
    {
        // NetworkVariable que sincroniza una estructura compleja de datos.
        NetworkVariable<SyncableCustomData> m_SyncedCustomData = new NetworkVariable<SyncableCustomData>(writePerm: NetworkVariableWritePermission.Owner);

        // UI que muestra la barra de salud y el nombre.
        [SerializeField] Image m_HealthBarImage;
        [SerializeField] TMP_Text m_UsernameLabel;

        [SerializeField, Tooltip("The seconds that will elapse between data changes")]
        float m_SecondsBetweenDataChanges;
        float m_ElapsedSecondsSinceLastChange;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient)
            {
                /*
                 * Sincroniza de inmediato el estado de salud y nombre cuando el cliente se une.
                 * Esto permite que el UI tenga el valor correcto incluso si se une tarde.
                 */
                OnClientCustomDataChanged(m_SyncedCustomData.Value, m_SyncedCustomData.Value);
                m_SyncedCustomData.OnValueChanged += OnClientCustomDataChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsClient)
            {
                m_SyncedCustomData.OnValueChanged -= OnClientCustomDataChanged;
            }
        }

        void Update()
        {
            if (!IsSpawned)
            {
                // El objeto de red ya no está activo, no hay que actualizar.
                return;
            }

            if (!IsServer)
            {
                /*
                 * El servidor es el único que escribe datos en este ejemplo.
                 * Los clientes reciben los cambios via NetworkVariable.
                 */
                return;
            }

            m_ElapsedSecondsSinceLastChange += Time.deltaTime;

            if (m_ElapsedSecondsSinceLastChange >= m_SecondsBetweenDataChanges)
            {
                m_ElapsedSecondsSinceLastChange = 0;
                OnServerChangeData();
            }
        }


        void OnServerChangeData()
        {
            // El servidor actualiza la estructura completa de datos sincronizados.
            m_SyncedCustomData.Value = new SyncableCustomData
            {
                Health = Random.Range(10, 101),
                Username = MultiplayerUseCasesUtilities.GetRandomUsername()
            };
        }

        void OnClientHealthChanged(int previousHealth, int newHealth)
        {
            // Ajusta el ancho de la barra de salud según el porcentaje.
            m_HealthBarImage.rectTransform.localScale = new Vector3((float)newHealth / 100.0f, 1);
            OnClientUpdateHealthBarColor(newHealth);
            // El previousHealth podría servir para reproducir una animación de daño o curación.
        }

        void OnClientUpdateHealthBarColor(int newHealth)
        {
            const int k_MaxHealth = 100;
            float healthPercent = (float)newHealth / k_MaxHealth;
            Color healthBarColor = new Color(1 - healthPercent, healthPercent, 0);
            m_HealthBarImage.color = healthBarColor;
        }

        void OnClientUsernameChanged(string newUsername)
        {
            // Muestra el nombre de usuario actualizado en la UI.
            m_UsernameLabel.text = newUsername;
        }

        void OnClientCustomDataChanged(SyncableCustomData previousValue, SyncableCustomData newValue)
        {
            // Actualiza tanto la barra de salud como el nombre cuando cambia el valor sincronizado.
            OnClientHealthChanged(previousValue.Health, newValue.Health);
            OnClientUsernameChanged(newValue.Username.ToString());
        }
    }
}
