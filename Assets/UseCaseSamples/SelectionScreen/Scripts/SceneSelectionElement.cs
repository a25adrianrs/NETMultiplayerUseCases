using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Netcode.Samples.MultiplayerUseCases.SelectionScreen
{
    /// <summary>
    /// Permite seleccionar una escena en la pantalla de selección
    /// </summary>
    internal class SceneSelectionElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Botón que representa la escena seleccionable.
        [SerializeField] Button m_SceneButton;

        // Texto que muestra el nombre de la escena.
        [SerializeField] TMP_Text m_TitleLabel;

        // Imagen oscura/contrastada que aparece al pasar el cursor.
        [SerializeField] Image m_ContrastImage;

        // Imagen que muestra el borde resaltado.
        [SerializeField] Image m_OutlineImage;

        // Color de borde normal.
        [SerializeField] Color m_OutlineColor;

        // Color de borde cuando el elemento está seleccionado o enfocado.
        [SerializeField] Color m_OutlineHighlightColor;

        internal void Setup(SelectableScene selectableScene)
        {
            // Asegura que el botón no tenga múltiples listeners añadidos.
            m_SceneButton.onClick.RemoveAllListeners();
            m_SceneButton.onClick.AddListener(() => OnClick(selectableScene.SceneName));

            // Asigna el texto y la imagen que se muestran en este elemento de escena.
            m_TitleLabel.text = selectableScene.DisplayName;
            if (selectableScene.Image)
            {
                m_SceneButton.image.sprite = Sprite.Create(selectableScene.Image, new Rect(0, 0, selectableScene.Image.width, selectableScene.Image.height), new Vector2(0.5f, 0.5f));
            }

            // Inicialmente desactiva el overlay y el resaltado.
            EnableOverlayElements(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Cuando el cursor pasa por encima del elemento, activa el overlay visual.
            EnableOverlayElements(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Cuando el cursor sale del área, desactiva el overlay.
            EnableOverlayElements(false);
        }

        void EnableOverlayElements(bool enable)
        {
            // Activa o desactiva los elementos gráficos de interacción.
            m_ContrastImage.gameObject.SetActive(enable);
            m_TitleLabel.gameObject.SetActive(enable);
            m_OutlineImage.color = enable ? m_OutlineHighlightColor : m_OutlineColor;
        }

        void OnClick(string sceneName)
        {
            // El usuario hizo clic en el botón, procede a cargar la escena.
            LoadScene(sceneName);
        }

        void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
