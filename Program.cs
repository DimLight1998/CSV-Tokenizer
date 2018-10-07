using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSVTokenizer
{
    class Program
    {
        private enum TokenizerState
        {
            ReadingNumber,
            ReadingString,
            FindingNextToken,
            FindingNextDelimeter,
            Error,
            Finish
        }

        public static (bool Success, List<string> Tokens) Tokenize(string csvContent)
        {
            var currentToken = new StringBuilder();
            var currentState = new TokenizerState();
            var tokens = new List<string>();
            var currentLocation = 0;
            currentState = TokenizerState.FindingNextToken;

            while (currentState != TokenizerState.Error && currentState != TokenizerState.Finish)
            {
                if (currentLocation == csvContent.Length)
                {
                    currentState = TokenizerState.Finish;
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                    break;
                }

                var c = csvContent[currentLocation];
                currentLocation++;

                switch (currentState)
                {
                    case TokenizerState.ReadingNumber:
                        if ("0123456789".Contains(c))
                            currentToken.Append(c);
                        else if ("\r\n,".Contains(c))
                        {
                            tokens.Add(currentToken.ToString());
                            currentToken.Clear();
                            currentState = TokenizerState.FindingNextToken;
                        }
                        else if (" \t".Contains(c))
                            currentState = TokenizerState.FindingNextDelimeter;
                        else
                            currentState = TokenizerState.Error;
                        break;
                    case TokenizerState.ReadingString:
                        if ("\"".Contains(c))
                            currentState = TokenizerState.FindingNextDelimeter;
                        else if ("\r\n".Contains(c))
                            currentState = TokenizerState.Error;
                        else
                            currentToken.Append(c);
                        break;
                    case TokenizerState.FindingNextToken:
                        if ("\r\n \t".Contains(c))
                            break;
                        else if (",".Contains(c))
                            tokens.Add("");
                        else if ("0123456789".Contains(c))
                        {
                            currentToken.Append(c);
                            currentState = TokenizerState.ReadingNumber;
                        }
                        else if ("\"".Contains(c))
                            currentState = TokenizerState.ReadingString;
                        else
                            currentState = TokenizerState.Error;
                        break;
                    case TokenizerState.FindingNextDelimeter:
                        if (" \t".Contains(c))
                            break;
                        else if ("\n\r,".Contains(c))
                        {
                            tokens.Add(currentToken.ToString());
                            currentToken.Clear();
                            currentState = TokenizerState.FindingNextToken;
                        }
                        else
                            currentState = TokenizerState.Error;
                        break;
                    case TokenizerState.Error:
                        break;
                    case TokenizerState.Finish:
                        break;
                }
            }

            return (currentState == TokenizerState.Finish, tokens);
        }

        static void Main(string[] args)
        {
            var content = File.ReadAllText("Content.csv");
            Console.WriteLine(content);
            Console.WriteLine("============");
            var result = Tokenize(content);
            if (result.Success) Console.WriteLine("Success\n");
            else Console.WriteLine("Failed\n");
            result.Tokens.ForEach(Console.WriteLine);
        }
    }
}
