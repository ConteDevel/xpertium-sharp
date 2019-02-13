using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XpertiumSharp.Core.Exceptions;

namespace XpertiumSharp.Core
{
    public class XToken<TTokenType> where TTokenType : Enum
    {
        public readonly TTokenType Type;
        public readonly string Value;

        public XToken(TTokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public XToken(TTokenType type) : this(type, null)
        {

        }
    }

    public class XTokenMatch<TTokenType> where TTokenType : Enum
    {
        public bool IsMatch { get; set; }
        public TTokenType TokenType { get; set; }
        public string Value { get; set; }
        public string RemainingText { get; set; }
    }

    public class XTokenDefinition<TTokenType> where TTokenType : Enum
    {
        private readonly TTokenType type;
        private readonly Regex regex;

        public XTokenDefinition(TTokenType type, Regex regex)
        {
            this.type = type;
            this.regex = regex;
        }

        public XTokenDefinition(TTokenType type, string regexPattern) : this(type, new Regex(regexPattern, RegexOptions.IgnoreCase))
        {

        }

        public XTokenMatch<TTokenType> Match(string input)
        {
            var match = regex.Match(input);

            if (match.Success)
            {
                string remainingText = string.Empty;

                if (match.Length != input.Length)
                {
                    remainingText = input.Substring(match.Length);
                }

                return new XTokenMatch<TTokenType>()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = type,
                    Value = match.Value
                };
            }
            else
            {
                return new XTokenMatch<TTokenType>() { IsMatch = false };
            }
        }
    }

    public class XTokenizer<TTokenType> where TTokenType : Enum
    {
        private readonly List<XTokenDefinition<TTokenType>> definitions;

        public XTokenizer(List<XTokenDefinition<TTokenType>> definitions)
        {
            this.definitions = definitions;
        }

        public XTokenizer() : this(new List<XTokenDefinition<TTokenType>>())
        {

        }

        public void AddDefinition(XTokenDefinition<TTokenType> definition)
        {
            definitions.Add(definition);
        }

        public bool RemoveDefinition(XTokenDefinition<TTokenType> definition)
        {
            return definitions.Remove(definition);
        }

        public void RemoveDefinitionAt(int index)
        {
            definitions.RemoveAt(index);
        }

        private XTokenMatch<TTokenType> FindMatch(string input)
        {
            foreach (var definition in definitions)
            {
                var match = definition.Match(input);

                if (match.IsMatch)
                {
                    return match;
                }
            }

            return new XTokenMatch<TTokenType>() { IsMatch = false };
        }

        private bool IsWhitespace(string lqlText)
        {
            return Regex.IsMatch(lqlText, "^\\s+");
        }
        
        /// <summary>
        /// Tokenizes input string
        /// </summary>
        /// <param name="input">Input string containing tokens</param>
        /// <returns>Tokens</returns>
        /// <exception cref="XInvalidTokenException">Throws when the next token wasn't recognized</exception>
        public List<XToken<TTokenType>> Tokenize(string input)
        {
            var tokens = new List<XToken<TTokenType>>();
            string remainingText = input;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);

                if (match.IsMatch)
                {
                    tokens.Add(new XToken<TTokenType>(match.TokenType, match.Value));
                    remainingText = match.RemainingText;
                }
                else
                {
                    int index = input.Length - match.RemainingText.Length;
                    throw new XInvalidTokenException(string.Format("Invalid token was found at {0}", index));
                }
            }

            return tokens;
        }
    }
}
