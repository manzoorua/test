using System;
using System.Collections.Generic;
using System.Text;

namespace PluginTest
{
    public class Plugin1 : IModule
    {
        public Guid Id 
        {
            get
            {
                return new Guid("304C5160-A65E-4ffb-BC00-FE7E165A795E");
            }
        }

        public string Name 
        {
            get
            {
                return "Test Module Plugin 1";
            }
        }

        public string Description 
        {
            get
            {
                return "Test Module 1 plugin Description";
            }
        }

        public Version Version 
        {
            get
            {
                return new Version();
            }
        }

        public void Call()
        {
            System.Console.WriteLine(DateTime.Now.ToString());
        }
    }
}
