using System;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Proximity
{
    /// <summary>
    /// Informs about the proximity status of the local player
    /// </summary>
    public class ProximityChecker : MonoBehaviour
    {
        [SerializeField, Tooltip("At which distance will the player be considered 'close'?")]
        float m_ActivationRadius = 1;

        [SerializeField, Tooltip("A visual representation of the radius?")]
        Transform m_RadiusRepresentation;

        // Cache del transform para no llamar varias veces a transform.
        Transform m_Transform;

        // Evento que notifica cambios en la proximidad del jugador local.
        event Action<bool> OnLocalPlayerProximityStatusChanged;

        // Indica si el jugador local está dentro del radio de activación.
        internal bool LocalPlayerIsClose { get; private set; }

        void Awake()
        {
            m_Transform = transform;
            if (m_RadiusRepresentation)
            {
                const float k_OffsetFromGround = 0.01f;
                m_RadiusRepresentation.transform.localPosition = new Vector3(0, (m_Transform.lossyScale.y / -2) + k_OffsetFromGround, 0);
            }
        }

        internal void AddListener(Action<bool> callback)
        {
            // Permite que otros scripts se suscriban para recibir cambios de proximidad.
            OnLocalPlayerProximityStatusChanged += callback;
        }

        internal void RemoveListener(Action<bool> callback)
        {
            // Elimina la suscripción cuando ya no sea necesaria.
            OnLocalPlayerProximityStatusChanged -= callback;
        }

        void Update()
        {
            if (m_RadiusRepresentation)
            {
                m_RadiusRepresentation.localScale = new Vector3(m_ActivationRadius * 2, m_RadiusRepresentation.localScale.y, m_ActivationRadius * 2);
            }

            bool oldValue = LocalPlayerIsClose;
            LocalPlayerIsClose = LocalPlayerIsCloseEnough(m_Transform.position, m_ActivationRadius);
            if (oldValue != LocalPlayerIsClose)
            {
                // Notifica a los oyentes solo cuando el estado cambia.
                OnLocalPlayerProximityStatusChanged?.Invoke(LocalPlayerIsClose);
            }
        }

        bool LocalPlayerIsCloseEnough(Vector3 point, float range)
        {
            // Note: este ejemplo usa NetworkManager.Singleton.LocalClient.PlayerObject
            // para encontrar al jugador local en lugar de una referencia estática.
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.LocalClient == null)
            {
                return false;
            }

            NetworkObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (!localPlayer)
            {
                return false;
            }
            return Vector3.Distance(point, localPlayer.transform.position) < range;
        }
    }
}
