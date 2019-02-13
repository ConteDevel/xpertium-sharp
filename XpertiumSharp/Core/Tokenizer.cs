using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XpertiumSharp.Core.Exceptions;

namespace XpertiumSharp.Core
{
    public class Token<TTokenType> where TTokenType : Enum
    {
        public readonly TTokenType Type;
        public readonly string Value;

        public Token(TTokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public Token(TTokenType type) : this(type, null)
        {

        }
    }

    public class TokenMatch<TTokenType> where TTokenType : Enum
    {
        public bool IsMatch { get; set; }
        public TTokenType TokenType { get; set; }
        public string Value { get; set; }
        public string RemainingText { get; set; }
    }

    public class TokenDefinition<TTokenType> where TTokenType : Enum
    {
        private readonly TTokenType type;
        private readonly Regex regex;

        public TokenDefinition(TTokenType type, Regex regex)
        {
            this.type = type;
            this.regex = regex;
        }

        public TokenDefinition(TTokenType type, string regexPattern) : this(type, new Regex(regexPattern, RegexOptions.IgnoreCase))
        {

        }

        public TokenMatch<TTokenType> Match(string input)
        {
            var match = regex.Match(input);

            if (match.Success)
            {
                string remainingText = string.Empty;

                if (match.Length != input.Length)
                {
                    remainingText = input.Substring(match.Length);
                }

                return new TokenMatch<TTokenType>()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = type,
                    Value = match.Value
                };
            }
            else
            {
                return new TokenMatch<TTokenType>() { IsMatch = false };
            }
        }
    }

    public class Tokenizer<TTokenType> where TTokenType : Enum
    {
        private readonly List<TokenDefinition<TTokenType>> definitions;

        public Tokenizer(List<TokenDefinition<TTokenType>> definitions)
        {
            this.definitions = definitions;
        }

        public Tokenizer() : this(new List<TokenDefinition<TTokenType>>())
        {

        }

        public void AddDefinition(TokenDefinition<TTokenType> definition)
        {
            definitions.Add(definition);
        }

        public bool RemoveDefinition(TokenDefinition<TTokenType> definition)
        {
            return definitions.Remove(definition);
        }

        public void RemoveDefinitionAt(int index)
        {
            definitions.RemoveAt(index);
        }

        private TokenMatch<TTokenType> FindMatch(string input)
        {
            foreach (var definition in definitions)
            {
                var match = definition.Match(input);

                if (match.IsMatch)
                {
                    return match;
                }
            }

            return new TokenMatch<TTokenType>() { IsMatch = false };
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
        /// <exception cref="InvalidTokenException">Throws when the next token wasn't recognized</exception>
        public List<Token<TTokenType>> Tokenize(string input)
        {
            var tokens = new List<Token<TTokenType>>();
            string remainingText = input;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);

                if (match.IsMatch)
                {
                    tokens.Add(new Token<TTokenType>(match.TokenType, match.Value));
                    remainingText = match.RemainingText;
                }
                else
                {
                    int index = input.Length - match.RemainingText.Length;
                    throw new InvalidTokenException(string.Format("Invalid token was found at {0}", index));
                }
            }

            return tokens;
        }
    }
}
