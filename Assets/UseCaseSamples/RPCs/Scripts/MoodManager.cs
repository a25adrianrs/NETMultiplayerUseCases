using System.Collections;
using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.RPC
{
    /// <summary>
    /// Gestiona el estado de humor de un jugador o NPC
    /// </summary>
    public class MoodManager : NetworkBehaviour
    {
        // Prefab para mostrar el diálogo del personaje.
        [SerializeField] SpeechBubble m_SpeechBubblePrefab;

        // Instancia local del globo de texto cuando se muestra el mensaje.
        SpeechBubble m_SpeechBubble;

        [SerializeField, Tooltip("The seconds that will elapse between data changes"), Range(2, 5)]
        float m_SecondsBetweenDataChanges;

        // Cronómetro para controlar cada cuánto se envía un mensaje.
        float m_ElapsedSecondsSinceLastChange;

        readonly string[] s_ChatMessages = new string[]
        {
            "Have a lovely day",
            "Are you pineapple? Duck you, potato!",
            "Today I feel like pineapple!",
            "Wow you're awesome!"
        };

        void Update()
        {
            if (!IsOwner)
            {
                // Solo el jugador local envía mensajes de humor.
                return;
            }

            m_ElapsedSecondsSinceLastChange += Time.deltaTime;
            if (m_ElapsedSecondsSinceLastChange >= m_SecondsBetweenDataChanges)
            {
                m_ElapsedSecondsSinceLastChange = 0;
                ServerMoodMessageReceivedRpc(s_ChatMessages[Random.Range(0, s_ChatMessages.Length)]);
            }
        }

        [Rpc(SendTo.Server)]
        void ServerMoodMessageReceivedRpc(string message)
        {
            /* El cliente envía el mensaje al servidor, que puede validar o filtrar contenido
             * antes de reenviarlo a todos los clientes. */
            string redactedMessage = OnServerFilterBadWords(message);
            ClientMoodMessageReceivedRpc(redactedMessage);
        }

        string OnServerFilterBadWords(string message)
        {
            return MultiplayerUseCasesUtilities.FilterBadWords(message);
        }

        [Rpc(SendTo.ClientsAndHost)]
        void ClientMoodMessageReceivedRpc(string message)
        {
            if (!m_SpeechBubble)
            {
                // Crea el globo de texto una sola vez y lo ancla al transform del objeto.
                m_SpeechBubble = Instantiate(m_SpeechBubblePrefab.gameObject, Vector3.zero, Quaternion.Euler(new Vector3(45, 0, 0))).GetComponent<SpeechBubble>();
                var positionOffsetKeeper = m_SpeechBubble.gameObject.AddComponent<PositionOffsetKeeper>();
                positionOffsetKeeper.Initialize(transform, new Vector3(0, 3, 0));
            }
            m_SpeechBubble.Setup(message);
            StartCoroutine(OnClientHideMessage());
        }

        IEnumerator OnClientHideMessage()
        {
            yield return new WaitForSeconds(1);
            m_SpeechBubble.Hide();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (m_SpeechBubble)
            {
                m_SpeechBubble.Hide();
                Destroy(m_SpeechBubble.gameObject);
            }
        }
    }
}
