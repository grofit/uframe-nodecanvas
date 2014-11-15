using System;
using NodeCanvas;

namespace NodeCanvasAddons.uFrame.Extensions
{
    public static class BlackboardExtensions
    {
        public static bool HasData(this Blackboard blackboard, string dataName, Type dataType)
        {
            return blackboard.GetData(dataName, dataType) != null;
        }

        public static bool HasData(this Blackboard blackboard, string dataName)
        {
            return blackboard.GetData(dataName, typeof(object)) != null;
        }
    }
}
