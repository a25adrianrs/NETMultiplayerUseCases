using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
{
    // Capas de objetos que se pueden empujar con el CharacterController.
    public LayerMask pushLayers;

    // Activa o desactiva la capacidad de empujar objetos.
    public bool canPush;

    // Fuerza con la que se empujan los cuerpos rígidos.
    [Range(0.5f, 5f)] public float strength = 1.1f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Este callback se ejecuta cuando el CharacterController colisiona con algo.
        if (canPush) PushRigidBodies(hit);
    }

    private void PushRigidBodies(ControllerColliderHit hit)
    {
        // Comprueba que el objeto tenga un Rigidbody válido y no sea kinemático.
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;

        // Limita el empuje solo a las capas deseadas.
        var bodyLayerMask = 1 << body.gameObject.layer;
        if ((bodyLayerMask & pushLayers.value) == 0) return;

        // No empuja objetos si el contacto viene desde arriba hacia abajo.
        if (hit.moveDirection.y < -0.3f) return;

        // Calcula una dirección horizontal para el empuje.
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

        // Aplica la fuerza al cuerpo rígido.
        body.AddForce(pushDir * strength, ForceMode.Impulse);
    }
}
