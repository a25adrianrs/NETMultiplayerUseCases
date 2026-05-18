using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UIElements;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
    /// <summary>
    /// Clase utilitaria para métodos comunes de configuración de UIElements
    /// </summary>
    public static class UIElementsUtils
    {
#if UNITY_EDITOR
        // Ruta dentro del proyecto donde se almacenan los archivos UXML para el editor.
        static readonly string k_UIFilesPathInTemplate = Path.Combine("Assets", Path.Combine("Editor", "UI"));

        /// <summary>
        /// Carga un archivo UXML desde una carpeta del editor
        /// </summary>
        /// <param name="fileName">Nombre del archivo a cargar</param>
        /// <returns></returns>
        public static VisualTreeAsset LoadUXML(string fileName)
        {
            string path = $"{Path.Combine(k_UIFilesPathInTemplate, fileName)}.uxml";
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        }
#endif

        /// <summary>
        /// Inicializa un Button
        /// </summary>
        /// <param name="buttonName">Nombre del botón en el documento</param>
        /// <param name="onClickAction">Método a ejecutar al hacer click</param>
        /// <param name="isEnabled">¿El botón está habilitado?</param>
        /// <param name="parent">Elemento padre del botón</param>
        /// <param name="text">Texto a mostrar en el botón</param>
        /// <param name="tooltip">Tooltip del botón</param>
        /// <param name="showIfEnabled">Muestra el elemento si debe estar habilitado</param>
        /// <returns>El botón inicializado</returns>
        public static Button SetupButton(string buttonName, Action onClickAction, bool isEnabled, VisualElement parent, string text = "", string tooltip = "", bool showIfEnabled = true)
        {
            // Busca el botón en el documento UI por su nombre y lo configura.
            Button button = parent.Query<Button>(buttonName);
            button.SetEnabled(isEnabled);
            button.clickable = new Clickable(() => onClickAction?.Invoke());
            button.text = text;
            button.tooltip = string.IsNullOrEmpty(tooltip) ? button.text : tooltip;

            // Si el botón debe mostrarse y está habilitado, lo hace visible.
            if (showIfEnabled && isEnabled)
            {
                Show(button);
            }

            return button;
        }

        /// <summary>
        /// Inicializa un EnumField
        /// </summary>
        /// <typeparam name="T">Tipo de los valores en el EnumField</typeparam>
        /// <param name="enumName">Nombre del EnumField en el documento</param>
        /// <param name="text">Texto para la etiqueta del EnumField</param>
        /// <param name="onValueChanged">Método a ejecutar cuando cambia el valor</param>
        /// <param name="parent">Elemento padre del EnumField</param>
        /// <param name="defaultValue">Valor por defecto del EnumField</param>
        /// <returns>El EnumField inicializado</returns>
        public static EnumField SetupEnumField<T>(string enumName, string text, EventCallback<ChangeEvent<Enum>> onValueChanged, VisualElement parent, T defaultValue) where T : Enum
        {
            // Busca y configura un campo de Enum en la UI.
            EnumField uxmlField = parent.Q<EnumField>(enumName);
            uxmlField.label = text;
            uxmlField.Init(defaultValue);
            uxmlField.value = defaultValue;
            uxmlField.RegisterCallback(onValueChanged);
            return uxmlField;
        }

        /// <summary>
        /// Inicializa un Toggle
        /// </summary>
        /// <param name="name">Nombre del toggle</param>
        /// <param name="label">Texto de la etiqueta del toggle</param>
        /// <param name="text">Texto del toggle</param>
        /// <param name="defaultValue">Valor por defecto del toggle</param>
        /// <param name="onValueChanged">Método a llamar cuando cambia el valor</param>
        /// <param name="parent">Elemento padre del toggle</param>
        /// <returns>El Toggle inicializado</returns>
        public static Toggle SetupToggle(string name, string label, string text, bool defaultValue, EventCallback<ChangeEvent<bool>> onValueChanged, VisualElement parent)
        {
            Toggle uxmlField = parent.Q<Toggle>(name);
            uxmlField.label = label;
            uxmlField.text = text;
            uxmlField.value = defaultValue;
            uxmlField.SetEnabled(true);
            uxmlField.RegisterCallback(onValueChanged);
            return uxmlField;
        }

        /// <summary>
        /// Inicializa un IntegerField
        /// </summary>
        /// <param name="name">Nombre del IntegerField</param>
        /// <param name="value">Valor inicial del IntegerField</param>
        /// <param name="onValueChanged">Método a llamar cuando cambia el valor</param>
        /// <param name="parent">Elemento padre del IntegerField</param>
        /// <returns>El IntegerField inicializado</returns>
        public static IntegerField SetupIntegerField(string name, int value, EventCallback<ChangeEvent<int>> onValueChanged, VisualElement parent)
        {
            IntegerField uxmlField = parent.Q<IntegerField>(name);
            uxmlField.value = value;
            uxmlField.SetEnabled(true);
            uxmlField.RegisterCallback(onValueChanged);
            return uxmlField;
        }

        /// <summary>
        /// Inicializa un StringField
        /// </summary>
        /// <param name="name">Nombre del StringField</param>
        /// <param name="label">Texto de la etiqueta del StringField</param>
        /// <param name="value">Valor inicial del StringField</param>
        /// <param name="onValueChanged">Método a llamar cuando cambia el valor</param>
        /// <param name="parent">Elemento padre del StringField</param>
        /// <returns>El StringField inicializado</returns>
        public static TextField SetupStringField(string name, string label, string value, EventCallback<ChangeEvent<string>> onValueChanged, VisualElement parent)
        {
            TextField uxmlField = parent.Q<TextField>(name);
            uxmlField.label = label;
            uxmlField.value = value;
            uxmlField.SetEnabled(true);
            uxmlField.RegisterCallback(onValueChanged);
            return uxmlField;
        }

        /// <summary>
        /// Hace invisible un elemento visual
        /// </summary>
        /// <param name="element">El elemento</param>
        public static void Hide(VisualElement element)
        {
            element.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Hace visible un elemento visual
        /// </summary>
        /// <param name="element">El elemento</param>
        public static void Show(VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }
}
