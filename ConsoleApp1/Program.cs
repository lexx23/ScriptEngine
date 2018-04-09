using ScriptEngine.EngineBase.Compiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Interpreter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //IList<ScriptModule> files = new List<ScriptModule>()
            //{
            //  new ScriptModule("goto", "goto", ModuleTypeEnum.STARTUP,false, "Exception\\try.scr")
            //};


            //ScriptProgramm programm = _helper.Compile(files);
            //ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            //interpreter.Debugger.AddBreakpoint("goto", 14);
            //interpreter.Debugger.AddBreakpoint("goto", 28);
            //interpreter.Debug();

            //int line = interpreter.CurrentLine;
            //int data = interpreter.Debugger.RegisterGetValue("ф").AsInt();
            //interpreter.Debugger.Continue();

            //line = interpreter.CurrentLine;
            //interpreter.Debugger.Continue();

        }
    }
}
