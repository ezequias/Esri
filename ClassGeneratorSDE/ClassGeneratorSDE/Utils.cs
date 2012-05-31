using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassGeneratorSDE
{
    class Utils
    {
        internal static string ChangeCharacters(string key)
        {
            key = key
                .Replace("¹", "1")
                .Replace("²", "2")
                .Replace("³", "3")
                .Replace("ª", "a")
                .Replace("º", "o")

                .Replace("ã", "a")
                .Replace("â", "a")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ä", "a")

                .Replace("Ã", "A")
                .Replace("Â", "A")
                .Replace("Á", "A")
                .Replace("À", "A")
                .Replace("Ä", "A")

                .Replace("ê", "e")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("ë", "e")

                .Replace("ê", "e")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("ë", "e")

                .Replace("õ", "o");
            return new Regex("[^a-zA-Z0-9_]+").Replace(key, "_");
        }
    }
}
