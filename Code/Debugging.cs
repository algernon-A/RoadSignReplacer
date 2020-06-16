using UnityEngine;


namespace RoadSignReplacer
{
    /// <summary>
    /// Debugging utility class.
    /// </summary>
    internal static class Debugging
    {
        /// <summary>
        /// Prints a single-line debugging message to the Unity output log.
        /// </summary>
        /// <param name="message"></param>
        internal static void Message(string message)
        {
            Debug.Log("Road Sign Replacer: " + message + ".");
        }
    }
}
