using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
    /// <summary>
    /// Clase de utilidades usada en los diferentes UseCases
    /// </summary>
    public static class MultiplayerUseCasesUtilities
    {
        // Nombres de usuario usados en ejemplos aleatorios.
        static readonly string[] s_Usernames = new string[] { "MaryDaBest", "BobTheBell", "FranklyVal", "Georgie96", "OP Morgan", "AdrianR", "EsterColero", "PixelKnight", "ShadowCoder", "NovaRunner", "IronPanda" };

        /// <summary>
        /// Genera un color aleatorio
        /// </summary>
        /// <returns>Un color RGBA aleatorio</returns>
        public static Color32 GetRandomColor() => new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), 255);

        /// <summary>
        /// Devuelve un nombre de usuario aleatorio de una lista fija.
        /// </summary>
        public static string GetRandomUsername() => s_Usernames[UnityEngine.Random.Range(0, s_Usernames.Length)];

        /// <summary>
        /// Filtra algunas "palabras malsonantes" de una cadena. En un entorno de producción, considere usar un servicio externo para tareas complejas como esta. Los jugadores pueden ingeniar formas de evitar filtros sencillos; usar una librería o servicio especializado suele ser más recomendable.
        /// </summary>
        /// <param name="input">Texto de entrada</param>
        /// <returns>Texto con las palabras filtradas</returns>
        public static string FilterBadWords(string input)
        {
            Regex regex = new Regex(@"\b(\w+)\b", RegexOptions.Compiled);
            var replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"potato", "*****"},
                {"duck", "@%!$"},
                {"pineapple", "$*%*!"}
            };

            // Reemplaza las palabras prohibidas por caracteres enmascarados.
            return regex.Replace(input, match => replacements.ContainsKey(match.Groups[1].Value) ? replacements[match.Groups[1].Value]
                                                                                                 : match.Groups[1].Value);
        }
    }
}
