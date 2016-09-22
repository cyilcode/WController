/*
  Extremely serious warning,
  This class is only and only an example for the plugin development functionality !!
 */
using System.Runtime.InteropServices;
public class MyPInvokeNativePlugin
{
    [DllImport("src/WCS.MAIN/Plugins/CompiledLibraries/MyPInvokeNativePlugin_Compiled.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int test(int number);
    public int MyAmazingCSharpNativeFunction()
    {
        return test(1337);
    }
}