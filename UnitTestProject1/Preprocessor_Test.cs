using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser;
using ScriptEngine.EngineBase.Parser.Precompiler;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class Preprocessor_Test
    {
        [TestMethod]
        public void Preprocessor_Region()
        {
            IList<IToken> tokens;
            tokens = LoadFile("regions.scr",null);
            Assert.AreEqual(1, tokens.Count);
        }

        [TestMethod]
        [Description("Проверка логики работы Если Тест1")]
        public void Preprocessor_If1()
        {
            IList<IToken> tokens;
            Dictionary<string, bool> defines = new Dictionary<string, bool>();
            defines.Add("НаКлиенте",true);

            tokens = LoadFile("if_simple.scr", defines);
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("test1", tokens[0].Content);
            Assert.AreEqual("test1_4", tokens[1].Content);
        }

        [TestMethod]
        [Description("Проверка логики работы Если Тест2")]
        public void Preprocessor_If2()
        {
            IList<IToken> tokens;
            Dictionary<string, bool> defines = new Dictionary<string, bool>();
            defines.Add("НаСервере", true);

            tokens = LoadFile("if_simple.scr", defines);
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual("test2", tokens[0].Content);
        }

        [TestMethod]
        [Description("Проверка логики работы Если Тест3")]
        public void Preprocessor_If3()
        {
            IList<IToken> tokens;
            tokens = LoadFile("if_simple.scr", null);
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual("test3",tokens[0].Content);
        }

        [TestMethod]
        [Description("Проверка логики работы Если Тест4")]
        public void Preprocessor_If4()
        {
            IList<IToken> tokens;
            Dictionary<string, bool> defines = new Dictionary<string, bool>();
            defines.Add("НаСервере", true);
            defines.Add("НаКлиенте", true);

            tokens = LoadFile("if_simple.scr", defines);
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual("test4", tokens[0].Content);
        }

        [TestMethod]
        [Description("Проверка логики работы Если Тест1_1")]
        public void Preprocessor_If1_1()
        {
            IList<IToken> tokens;
            Dictionary<string, bool> defines = new Dictionary<string, bool>();
            defines.Add("НаКлиенте", true);
            defines.Add("НаКлиенте1", true);

            tokens = LoadFile("if_simple.scr",defines);
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("test1", tokens[0].Content);
            Assert.AreEqual("test1_1", tokens[1].Content);
        }


        [TestMethod]
        [Description("Проверка логики работы Если Тест1_1")]
        public void Preprocessor_If1_2()
        {
            IList<IToken> tokens;
            Dictionary<string, bool> defines = new Dictionary<string, bool>();
            defines.Add("НаКлиенте", true);
            defines.Add("НаКлиенте1", true);


            tokens = LoadFile("if_simple.scr", defines);
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("test1", tokens[0].Content);
            Assert.AreEqual("test1_1", tokens[1].Content);
        }

        [TestMethod]
        [Description("Проверка логики работы Если Тест1_3")]
        public void Preprocessor_If1_3()
        {
            IList<IToken> tokens;
            Dictionary<string, bool> defines = new Dictionary<string, bool>();
            defines.Add("НаКлиенте", true);
            defines.Add("НаКлиенте2", true);


            tokens = LoadFile("if_simple.scr", defines);
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("test1", tokens[0].Content);
            Assert.AreEqual("test1_2", tokens[1].Content);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Preprocessor_Not_Closed_Region()
        {
            LoadFile("not_closed_region.scr",null);
        }


        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Preprocessor_Not_Closed_If()
        {
            LoadFile("not_closed_if.scr",null);
        }


        private IList<IToken> LoadFile(string name,IDictionary<string,bool> defines)
        {
            string source = string.Empty;

            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Preprocessor\\" + name;
            if (File.Exists(path))
            {
                source = File.ReadAllText(path);

                ParserClass parser = new ParserClass(path, source);
                PrecompilerClass precompiler = new PrecompilerClass(parser.GetEnumerator(), defines);

                return precompiler.GetAllTokens();
                

            }
            else
                throw new Exception($"Файл {path} не найден.");

        }

    }
}
