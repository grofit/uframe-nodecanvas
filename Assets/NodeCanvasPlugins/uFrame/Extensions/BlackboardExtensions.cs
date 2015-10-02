using System;
using NodeCanvas.Framework;

namespace NodeCanvasAddons.uFrame.Extensions
{
    public static class BlackboardExtensions
    {
        public static bool HasData(this Blackboard blackboard, string dataName, Type dataType)
        {
            return blackboard.variables.ContainsKey(dataName);
        }

        public static bool HasData(this Blackboard blackboard, string dataName)
        {
            return blackboard.variables.ContainsKey(dataName);
        }
    }
}
