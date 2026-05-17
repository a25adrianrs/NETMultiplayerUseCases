using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
    /// <summary>
    /// A class of utilities used across UseCases
    /// </summary>
    public static class MultiplayerUseCasesUtilities
    {
        // Nombres de usuario usados en ejemplos aleatorios.
        static readonly string[] s_Usernames = new string[] { "MaryDaBest", "BobTheBell", "FranklyVal", "Georgie96", "OP Morgan", "AdrianR", "EsterColero", "PixelKnight", "ShadowCoder", "NovaRunner", "IronPanda" };

        /// <summary>
        /// Generates a random color
        /// </summary>
        /// <returns>A random RGBA color</returns>
        public static Color32 GetRandomColor() => new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), 255);

        /// <summary>
        /// Returns a random username from a fixed list.
        /// </summary>
        public static string GetRandomUsername() => s_Usernames[UnityEngine.Random.Range(0, s_Usernames.Length)];

        /// <summary>
        /// Filters some 'bad words' from a string. In a production environment, consider using an external service for complex tasks like this. Players can be quite imaginative creating workarounds for those types of filters, using a library or service abstracting that complexity away is usually more strategic.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
