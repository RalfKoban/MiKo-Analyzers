using System;
using System.Collections.Generic;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class WordSeparator
    {
        internal static string Separate(string text, char separator, FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            if (text.IsNullOrEmpty())
            {
                return text;
            }

            var multipleUpperCases = false;

            const int CharacterToStartWith = 1;

            var characters = new List<char>(text);

            for (var index = CharacterToStartWith; index < characters.Count; index++)
            {
                var c = characters[index];

                if (c == separator)
                {
                    // keep the existing separator
                    continue;
                }

                if (c.IsUpperCase())
                {
                    if (index == CharacterToStartWith)
                    {
                        // multiple upper cases in a line at beginning of the name, so do not flip
                        multipleUpperCases = true;
                    }

                    if (multipleUpperCases)
                    {
                        // let's see if we start with an IXyz interface
                        if (characters[index - 1] == 'I')
                        {
                            // seems we are in an IXyz interface
                            multipleUpperCases = false;
                        }

                        continue;
                    }

                    // let's consider an upper-case 'A' as a special situation as that is a single word
                    var isSpecialCharA = c == 'A';

                    multipleUpperCases = isSpecialCharA is false;

                    var nextC = c.ToLowerCase();

                    var nextIndex = index + 1;

                    if ((nextIndex >= characters.Count || (nextIndex < characters.Count && characters[nextIndex].IsUpperCase())) && isSpecialCharA is false)
                    {
                        // multiple upper cases in a line, so do not flip
                        nextC = c;
                    }

                    if (characters[index - 1] == separator)
                    {
                        characters[index] = nextC;
                    }
                    else
                    {
                        // only add an underline if we not already have one
                        characters[index] = separator;
                        index++;
                        characters.Insert(index, nextC);
                    }
                }
                else
                {
                    if (multipleUpperCases && characters[index - 1].IsUpperCase())
                    {
                        // we are behind multiple upper cases in a line, so add an underline
                        characters[index++] = separator;

                        characters.Insert(index, c);
                    }

                    multipleUpperCases = false;
                }
            }

            switch (firstWordHandling)
            {
                case FirstWordHandling.MakeLowerCase:
                    characters[0] = characters[0].ToLowerCase();

                    break;

                case FirstWordHandling.MakeUpperCase:
                    characters[0] = characters[0].ToLowerCase();

                    break;
            }

            return new string(characters.ToArray());
        }
    }
}