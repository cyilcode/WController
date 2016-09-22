using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using WCS.MAIN.Globals;

namespace WCS.MAIN.Functions.Handlers
{
    public class PluginHandler
    {
        private const    string                     _pluginBasePath     = "../../src/WCS.MAIN/Plugins/";
        private readonly Dictionary<string, string> _providerOptions;
        public PluginHandler(string compilerVersion)
        {
            _providerOptions   = new Dictionary<string, string>();
            _providerOptions.Add("CompilerVersion", compilerVersion);
        }

        public object getPluginInstance(string className)
        {
            CSharpCodeProvider csCodeProvider     = new CSharpCodeProvider (_providerOptions);
            CompilerParameters providerParameters = new CompilerParameters { GenerateExecutable = false,
                                                                             GenerateInMemory   = true  };
            string fullPath = string.Format("{0}{1}.cs", _pluginBasePath, className);
            string codeSource = System.IO.File.ReadAllText(fullPath);
            CompilerResults compiledResults = csCodeProvider.
                                              CompileAssemblyFromSource(providerParameters, codeSource);
            if (compiledResults.Errors.Count != 0)
            {
                foreach (CompilerError item in compiledResults.Errors)
                {
                    string errorMessage = string.Format("An error occured on plugin file {0}. Line: {1}. Error message: {2}",
                                                        item.FileName,
                                                        item.Line,
                                                        item.ErrorText);
                    GlobalHelper.log(errorMessage);
                }
                throw new InvalidOperationException("An error occured while trying to execute a plugin function.");
            }
            object instanceOfObject = compiledResults.CompiledAssembly.CreateInstance(className);
            if (instanceOfObject == null)
                GlobalHelper.log("Couldn't get the instance of plugin class: " + className);
            return instanceOfObject;
        }
    }
}