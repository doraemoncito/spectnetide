﻿namespace Spect.Net.Z80Tests.Audio
{
    /// <summary>
    /// Wave Callback Strategy
    /// </summary>
    public enum WaveCallbackStrategy
    {
        /// <summary>
        /// Use a function
        /// </summary>
        FunctionCallback,
        /// <summary>
        /// Create a new window (should only be done if on GUI thread)
        /// </summary>
        NewWindow
    }
}