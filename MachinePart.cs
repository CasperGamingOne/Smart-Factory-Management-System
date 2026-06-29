using System.Net.NetworkInformation;

namespace Smart_Factory_Management_System
{
    internal class MachinePart
    {
        private protected string? part_name;

        private protected bool? condition;

        public string? Name { get { return part_name; } set { part_name = value; } }
        public bool? Condition { get { return condition; } set { condition = true; } }

        public MachinePart(string name)
        {
            Name = name; 
        }
    }
}
