﻿namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the DivIDE device
    /// </summary>
    public interface IDivIdeDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Indicates if DivIDE ROM is paged in
        /// </summary>
        bool IsDivIdePagedIn { get; }

        /// <summary>
        /// The CONMEM bit of the controller
        /// </summary>
        bool ConMem { get; }

        /// <summary>
        /// The MAPRAM bit of the controller
        /// </summary>
        bool MapRam { get; }

        /// <summary>
        /// The selected bank (0..3)
        /// </summary>
        int Bank { get; }

        /// <summary>
        /// Sets the divide control register
        /// </summary>
        /// <param name="value">Control register value</param>
        void SetControlRegister(byte value);

        /// <summary>
        /// Processed the specified operation address
        /// </summary>
        /// <param name="addr"></param>
        void ProcessOpAddress(ushort addr);
    }
}