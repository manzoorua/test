using System;
using System.Collections.Generic;
using System.Text;
using TomoTherapy.Utilities.Plugin;

namespace PluginTest
{
    public interface IModule : IPlugin
    {
        void Call();
    }
}
