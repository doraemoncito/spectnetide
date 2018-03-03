﻿namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// Represents a device that implements the Spectrum Next feature set
    /// virtual machine
    /// </summary>
    public interface INextFeatureSetDevice: ISpectrumBoundDevice, ITbBlueControlDevice
    {
        /// <summary>
        /// Gets the value of the register specified by the latest
        /// SetRegisterIndex call
        /// </summary>
        /// <remarks>If the specified register is not supported, returns 0xFF</remarks>
        byte GetRegisterValue();
    }
}