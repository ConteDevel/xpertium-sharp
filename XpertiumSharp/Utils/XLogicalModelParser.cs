using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XpertiumSharp.Core;
using XpertiumSharp.Logic;

namespace XpertiumSharp.Utils
{
    public class XLogicalModelParser
    {
        private readonly XTokenizer<XTokenType> tokenizer;

        public XLogicalModelParser()
        {
            var definitions = new List<XTokenDefinition<XTokenType>>()
            {
                new XTokenDefinition<XTokenType>(XTokenType.PredicateBlock, "^predicates:"),
                new XTokenDefinition<XTokenType>(XTokenType.ClauseBlock, "^clauses:"),
                new XTokenDefinition<XTokenType>(XTokenType.Value, "^\\\'(.*?)\\\'"),
                new XTokenDefinition<XTokenType>(XTokenType.LBrace, "^\\("),
                new XTokenDefinition<XTokenType>(XTokenType.RBrace, "^\\)"),
                new XTokenDefinition<XTokenType>(XTokenType.Name, "^[a-zA-Z]+"),
                new XTokenDefinition<XTokenType>(XTokenType.Arity, "^[0-9]+"),
                new XTokenDefinition<XTokenType>(XTokenType.Not, "^!"),
                new XTokenDefinition<XTokenType>(XTokenType.Separator, "^,"),
                new XTokenDefinition<XTokenType>(XTokenType.Dot, "^\\."),
                new XTokenDefinition<XTokenType>(XTokenType.Assignment, "^\\:-")
            };
            tokenizer = new XTokenizer<XTokenType>(definitions);
        }

        private int ParseSignature(List<XToken<XTokenType>> tokens, int start, int end, XDatabase db)
        {
            int position = start;

            if (tokens[position].Type == XTokenType.Name)
            {
                string name = tokens[position++].Value;

                if (position < end && tokens[position++].Type == XTokenType.LBrace)
                {
                    int arity = 0;

                    if (position < end && tokens[position].Type == XTokenType.Arity)
                    {
                        arity = int.Parse(tokens[position].Value);
                        ++position;
                    }
                    
                    if (position < end && tokens[position++].Type == XTokenType.RBrace)
                    {
                        foreach (var s in db.Signatures)
                        {
                            if (s.Name == name)
                            {
                                throw new ApplicationException("Redefinition of the signature");
                            }
                        }

                        db.Signatures.Add(new XSignature(name, arity));

                        return position;
                    }
                }
            }

            throw new ApplicationException("Invalid signature syntax");
        }

        private void ParseSignatures(List<XToken<XTokenType>> tokens, int start, int end, XDatabase db)
        {
            for (int i = start; i < end;)
            {
                i = ParseSignature(tokens, i, end, db);
            }
        }

        private int ParseArguments(List<XToken<XTokenType>> tokens, int start, int end, XVar[] args)
        {
            int position = start;

            for (int i = 0; i < args.Length; ++i)
            {
                if (position < end)
                {
                    if (tokens[position].Type == XTokenType.Name)
                    {
                        args[i] = new XVar(XType.Var, tokens[position++].Value);
                    }
                    else if (tokens[position].Type == XTokenType.Value)
                    {
                        args[i] = new XVar(XType.Const, tokens[position++].Value);
                    }
                    else
                    {
                        throw new ApplicationException("Invalid argument");
                    }

                    if (i != (args.Length - 1) && tokens[position++].Type != XTokenType.Separator)
                    {
                        throw new ApplicationException("Wrong arguments count");
                    }
                }
            }

            return position;
        }

        private int ParsePredicate(List<XToken<XTokenType>> tokens, int start, int end, XDatabase db, out XPredicate predicate)
        {
            int position = start;
            predicate = null;

            if (tokens[position].Type == XTokenType.Name)
            {
                string name = tokens[position++].Value;
                XSignature signature = null;

                foreach (var s in db.Signatures)
                {
                    if (s.Name == name)
                    {
                        signature = s;
                        break;
                    }
                }

                if (signature == null)
                {
                    throw new ApplicationException(string.Format("Undefined predicate {0}", name));
                }

                if (position < end && tokens[position++].Type == XTokenType.LBrace)
                {
                    var vars = new XVar[signature.Arity];
                    position = ParseArguments(tokens, position, end, vars);

                    if (position < end && tokens[position++].Type == XTokenType.RBrace)
                    {
                        predicate = new XPredicate(signature, vars);

                        return position;
                    }
                }
            }

            throw new ApplicationException("Invalid predicate syntax");
        }

        private int ParseExpression2(List<XToken<XTokenType>> tokens, int start, int end, XDatabase db, out IXExpression expression)
        {
            int position = start;

            if (tokens[position].Type == XTokenType.Not)
            {
                position = ParsePredicate(tokens, position + 1, end, db, out XPredicate predicate);
                expression = new XNot(new XExpression(predicate));
            }
            else
            {
                position = ParsePredicate(tokens, position, end, db, out XPredicate predicate);
                expression = new XExpression(predicate);
            }

            return position;
        }

        private int ParseExpression(List<XToken<XTokenType>> tokens, int start, int end, XDatabase db, out IXExpression expression)
        {
            int position = start;
            expression = null;
            var expressions = new List<IXExpression>();

            for (; position < end; ++position)
            {
                position = ParseExpression2(tokens, position, end, db, out IXExpression child);
                expressions.Add(child);

                if (tokens[position].Type != XTokenType.Separator)
                {
                    break;
                }
            }

            if (expressions.Count == 0)
            {
                throw new ApplicationException("Clause has an empty body");
            }
            else if (expressions.Count == 1)
            {
                expression = expressions[0];
            }
            else
            {
                expression = new XAnd(expressions.ToArray());
            }

            return position;
        }

        private int ParseClause(List<XToken<XTokenType>> tokens, int start, int end, XDatabase db)
        {
            int position = ParsePredicate(tokens, start, end, db, out XPredicate predicate);

            if (tokens[position].Type == XTokenType.Dot)
            {
                db.Clauses.Add(new XClause(null, predicate));
                return ++position;
            }

            if (tokens[position++].Type != XTokenType.Assignment)
            {
                throw new ApplicationException("Invalid clause syntax");
            }

            position = ParseExpression(tokens, position, end, db, out IXExpression expression);

            if (position == end || tokens[position++].Type != XTokenType.Dot)
            {
                throw new ApplicationException("Not terminated clause");
            }

            db.Clauses.Add(new XClause(expression, predicate));

            return position;
        }

        private void ParseClauses(List<XToken<XTokenType>> tokens, int start, int end, XDatabase db)
        {
            for (int i = start; i < end;)
            {
                i = ParseClause(tokens, i, end, db);
            }
        }

        private void ParseBody(List<XToken<XTokenType>> tokens, XDatabase db)
        {
            if (tokens.Count > 0)
            {
                if (tokens[0].Type == XTokenType.PredicateBlock)
                {
                    int end = tokens.Count;
                    bool hasClauses = false;

                    for (int i = 1; i < tokens.Count; ++i)
                    {
                        if (tokens[i].Type == XTokenType.ClauseBlock)
                        {
                            hasClauses = true;
                            end = i;
                            break;
                        }
                    }

                    if (tokens.Count == 1)
                    {
                        throw new ApplicationException("No available predicates");
                    }

                    ParseSignatures(tokens, 1, end, db);

                    ++end;

                    if (hasClauses)
                    {
                        ParseClauses(tokens, end, tokens.Count, db);
                    }
                }
                else
                {
                    throw new ApplicationException("Missing the predicate block");
                }
            }
        }

        public XPredicate ParseTarget(string input, XDatabase db)
        {
            input = Regex.Replace(input, @"\s+", string.Empty);

            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("No target");
            }

            var tokens = tokenizer.Tokenize(input);
            ParsePredicate(tokens, 0, tokens.Count, db, out XPredicate predicate);

            return predicate;
        }

        public XDatabase Parse(string input)
        {
            input = Regex.Replace(input, @"\s+", string.Empty);
            var tokens = tokenizer.Tokenize(input);
            var db = new XDatabase();
            ParseBody(tokens, db);

            return db;
        }

        private enum XTokenType
        {
            PredicateBlock,
            ClauseBlock,
            Value,
            LBrace,
            RBrace,
            Name,
            Arity,
            Separator,
            Not,
            Dot,
            Assignment
        }
    }
}
