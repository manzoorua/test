using System;
using System.Collections.Generic;
using System.Text;
using TomoTherapy.Utilities.Plugin;

namespace PluginConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the plugin manager.
            //
            PluginManager<PluginTest.IModule> manager = new PluginManager<PluginTest.IModule>("./plugins");

            manager.LoadPlugins("*.dll");

            PluginTest.IModule module = manager.CreateInstanceByPluginId(new Guid("304C5160-A65E-4ffb-BC00-FE7E165A795E"));

            if (module != null)
            {
                module.Call();

                module = null;
            }
        }
    }
}
