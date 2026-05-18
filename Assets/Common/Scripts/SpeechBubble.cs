using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
    /// <summary>
    /// Muestra texto en forma de globo de diálogo
    /// </summary>
    public class SpeechBubble : MonoBehaviour
    {
        // Sprite que sirve como fondo del globo de texto.
        [SerializeField] SpriteRenderer m_BackgroundSprite;

        // Texto que se muestra dentro del globo.
        [SerializeField] TMP_Text m_MessageLabel;

        // Espacio extra alrededor del texto para el tamaño del fondo.
        [SerializeField] Vector2 m_Padding;

        // Número máximo de caracteres por línea antes de hacer un salto de línea.
        [SerializeField] int m_MaxCharactersPerLine = 20;

        // Mensaje inicial que se muestra si se activa en Start.
        [SerializeField] string m_DefaultMessage;

        // Decide si el globo se configura automáticamente al iniciar la escena.
        [SerializeField] bool m_SetupOnStart;

        void Start()
        {
            if (m_SetupOnStart)
            {
                Setup(m_DefaultMessage);
            }
        }

        /// <summary>
        /// Muestra texto en el globo
        /// </summary>
        /// <param name="text">Texto a mostrar</param>
        public void Setup(string text)
        {
            // Muestra el globo de texto y actualiza su contenido.
            gameObject.SetActive(true);
            m_MessageLabel.SetText(WordWrap(text, m_MaxCharactersPerLine));
            m_MessageLabel.ForceMeshUpdate();

            // Ajusta el tamaño del fondo al texto renderizado más el padding.
            Vector2 textSize = m_MessageLabel.GetRenderedValues(false);
            m_BackgroundSprite.size = textSize + m_Padding;
        }

        /// <summary>
        /// Ajusta el texto para que quepa dentro del ancho especificado.
        /// </summary>
        /// <param name="text">Texto a ajustar</param>
        /// <param name="width">Ancho, en caracteres, al que ajustar el texto</param>
        /// <returns>El texto modificado</returns>
        /// <remarks>Basado en: https://www.codeproject.com/Articles/51488/Implementing-Word-Wrap-in-C </remarks>
        static string WordWrap(string text, int width)
        {
            if (width < 1)
            {
                return text;
            }

            int position;
            int next;
            var sb = new StringBuilder();
            // Parse each line of text
            for (position = 0; position < text.Length; position = next)
            {
                int lineEndingIndex = text.IndexOf(Environment.NewLine, position);
                if (lineEndingIndex == -1)
                {
                    next = lineEndingIndex = text.Length;
                }
                else
                {
                    next = lineEndingIndex + Environment.NewLine.Length;
                }

                // Copia cada línea del texto, rompiéndola si es demasiado larga.
                if (lineEndingIndex > position)
                {
                    do
                    {
                        int lineLentgh = lineEndingIndex - position;
                        if (lineLentgh > width)
                        {
                            lineLentgh = BreakLine(text, position, width);
                        }
                        sb.Append(text, position, lineLentgh);
                        sb.Append(Environment.NewLine);

                        // Elimina espacios en blanco al inicio de la siguiente línea.
                        position += lineLentgh;
                        while (position < lineEndingIndex && Char.IsWhiteSpace(text[position]))
                        {
                            position++;
                        }
                    } while (lineEndingIndex > position);
                }
                else
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Busca la posición para romper la línea evitando partir palabras.
        /// </summary>
        /// <param name="text">Cadena que contiene la línea de texto</param>
        /// <param name="pos">Índice donde comienza la línea</param>
        /// <param name="max">Longitud máxima de la línea</param>
        /// <returns>La longitud de la línea modificada</returns>
        static int BreakLine(string text, int pos, int max)
        {
            // Busca el último espacio en blanco dentro del rango permitido.
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            // Si no hay espacio en blanco, rompe la línea en el máximo permitido.
            if (i < 0)
            {
                return max;
            }

            // Retrocede hasta el primer carácter no blanco antes del espacio.
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            // Devuelve la longitud de texto antes del espacio detectado.
            return i + 1;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
