﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class TokenParser_Tests
    {
        private IList<TokenClass> LoadFile(string name)
        {
            string source = string.Empty;

            string path = Directory.GetCurrentDirectory() + "\\Scripts\\TokenParser\\" + name;
            if (File.Exists(path))
            {
                source = File.ReadAllText(path);

                ParserClass parser = new ParserClass(path, source);

                return parser.GetAllTokens();


            }
            else
                throw new Exception($"Файл {path} не найден.");

        }



        [TestMethod]
        public void String_Simple()
        {
            IList<TokenClass> tokens;

            tokens = LoadFile("string_simple.scr");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual("test", tokens[0].Content);
        }


        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void String_Not_Full()
        {
            LoadFile("not_full.scr");
        }


        [TestMethod]
        public void String_Double_Quote()
        {
            IList<TokenClass> tokens;

            tokens = LoadFile("double_quote.scr");
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("", tokens[0].Content);
            Assert.AreEqual("\"\"Test\"\"", tokens[1].Content); 
        }
    }
}