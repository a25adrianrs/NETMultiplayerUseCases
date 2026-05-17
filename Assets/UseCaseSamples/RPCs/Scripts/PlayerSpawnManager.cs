using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.RPC
{
    /// <summary>
    /// Manages how a player will be spawned
    /// </summary>
    class PlayerSpawnManager : NetworkBehaviour
    {
        void Start()
        {
            // Se registra el callback que decide si cada conexión es aprobada y dónde nace su jugador.
            NetworkManager.ConnectionApprovalCallback = ConnectionApprovalCallback;
        }

        void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            /* Este método se ejecuta cuando un cliente intenta conectarse.
             * Aquí se puede aprobar o rechazar la conexión y personalizar la posición/objeto jugador. */
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Position = GetPlayerSpawnPosition();
        }

        Vector3 GetPlayerSpawnPosition()
        {
            /* Esta implementación solo devuelve una posición aleatoria sencilla.
             * En un juego real, podrías elegir puntos de spawn concretos según el equipo,
             * el orden de llegada u otras condiciones. */
            return new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
        }
    }
}
