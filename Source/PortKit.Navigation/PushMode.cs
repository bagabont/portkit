namespace PortKit.Navigation
{
    public enum PushMode
    {
        /// <summary>
        /// Push to the current navigation stack.
        /// </summary>
        Stack,

        /// <summary>
        /// Clear the current navigation stack.
        /// </summary>
        Clear,

        /// <summary>
        /// Remove all navigation stacks and clear the root navigation stack.
        /// </summary>
        Reset,

        /// <summary>
        /// Set the root navigation stack.
        /// </summary>
        Root
    }
}