using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Netcode.Samples.MultiplayerUseCases.SelectionScreen
{
    [Serializable]
    internal struct SelectableScene
    {
        [SerializeField] internal string SceneName;
        [SerializeField] internal string DisplayName;
        [SerializeField] internal Texture2D Image;
    }

    /// <summary>
    /// An UI that allows players to pick a scene to load
    /// </summary>
    internal class SceneSelectionUI : MonoBehaviour
    {
        // Lista de escenas que el usuario puede seleccionar desde la UI.
        [SerializeField] SelectableScene[] m_Scenes;

        // Contenedor de UI donde se instancian los elementos de selección de escena.
        [SerializeField] GridLayoutGroup m_Container;

        // Prefab visual que representa cada escena disponible para seleccionar.
        [SerializeField] SceneSelectionElement m_SceneUIPrefab;

        void OnEnable()
        {
            // Se ejecuta cuando el GameObject se activa.
            // Llama a Setup para (re)construir la lista de opciones de escena.
            Setup();
        }

        void Setup()
        {
            // Limpia cualquier elemento anterior antes de crear la lista actual.
            DestroyAllChildrenOf(m_Container.transform);

            // Crea un SceneSelectionElement para cada escena configurada.
            foreach (var scene in m_Scenes)
            {
                SceneSelectionElement sceneUI = Instantiate(m_SceneUIPrefab, m_Container.transform);
                sceneUI.Setup(scene);
            }
        }

        static void DestroyAllChildrenOf(Transform t)
        {
            int childrenToRemove = t.childCount;
            for (int i = childrenToRemove - 1; i >= 0; i--)
            {
                GameObject.Destroy(t.GetChild(i).gameObject);
            }
        }
    }
}
