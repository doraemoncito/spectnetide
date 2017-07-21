﻿using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// This class represents the cassette tape device in ZX Spectrum
    /// </summary>
    public class TapeDevice : ITapeDevice
    {
        private IBeeperDevice _beeperDevice;
        private IZ80Cpu _cpu;

        /// <summary>
        /// Number of tacts after save mod can be exited automatically
        /// </summary>
        public const int SAVE_STOP_SILENCE = 17_500_000;

        /// <summary>
        /// The address of the SAVE_BYTES routine in the Spectrum ROM
        /// </summary>
        public const ushort SAVE_BYTES_ROM_ADDRESS = 0x4C2;

        /// <summary>
        /// The address of the LOAD_START routine in the Spectrum ROM
        /// </summary>
        public const ushort LOAD_START_ROM_ADDRESS = 0x56C;

        /// <summary>
        /// The address of the ERROR routine in the Spectrum ROM
        /// </summary>
        public const ushort ERROR_ROM_ADDRESS = 0x0008;

        /// <summary>
        /// The maximum distance between two scans of the EAR bit
        /// </summary>
        public const int MAX_TACT_JUMP = 10000;

        /// <summary>
        /// Gets the TZX tape content provider
        /// </summary>
        public ITzxTapeContentProvider ContentProvider { get; }

        /// <summary>
        /// The current operation mode of the tape
        /// </summary>
        public TapeOperationMode CurrentMode { get; private set; }

        /// <summary>
        /// Stores the start CPU tact when either save or load mode commenced
        /// </summary>
        public long SaveStartTact { get; private set; }

        /// <summary>
        /// The CPU tact of the last MIC bit activity
        /// </summary>
        public long LastMicBitActivityTact { get; private set; }

        /// <summary>
        /// Gets the current state of the MIC bit
        /// </summary>
        public bool MicBitState { get; private set; }

        /// <summary>
        /// The pulses saved during the save mode
        /// </summary>
        public IList<MicBitPulse> SavedPulses { get; private set; }

        /// <summary>
        /// The TzxPlayer that can playback tape content
        /// </summary>
        public TzxPlayer Player { get; private set; }

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            _cpu = hostVm.Cpu;
            _beeperDevice = hostVm.BeeperDevice;
            Reset();
        }

        /// <summary>
        /// Initializes the tape device for the specified host VM
        /// </summary>
        /// <param name="contentProvider">Tape content provider</param>
        public TapeDevice(ITzxTapeContentProvider contentProvider)
        {
            ContentProvider = contentProvider;
        }

        /// <summary>
        /// Resets the tape device
        /// </summary>
        public void Reset()
        {
            ContentProvider?.Reset();
            Player = null;
            CurrentMode = TapeOperationMode.Passive;
            MicBitState = true;
            SavedPulses = new List<MicBitPulse>();
        }

        /// <summary>
        /// Gets the EAR bit read from the tape
        /// </summary>
        /// <param name="cpuTicks"></param>
        /// <returns></returns>
        public bool GetEarBit(long cpuTicks)
        {
            if (CurrentMode != TapeOperationMode.Load)
            {
                return true;
            }
            var earBit = Player?.GetEarBit(cpuTicks) ?? true;
            _beeperDevice.ProcessEarBitValue(earBit);
            return earBit;
        }

        /// <summary>
        /// Sets the current tape mode according to the current PC register
        /// and the MIC bit state
        /// </summary>
        public void SetTapeMode()
        {
            var ticks = _cpu.Tacts;
            var error = _cpu.Registers.PC == ERROR_ROM_ADDRESS;
            switch (CurrentMode)
            {
                case TapeOperationMode.Passive:
                    if (_cpu.Registers.PC == LOAD_START_ROM_ADDRESS)
                    {
                        EnterLoadMode();
                    }
                    else if (_cpu.Registers.PC == SAVE_BYTES_ROM_ADDRESS)
                    {
                        EnterSaveMode();
                    }
                    return;
                case TapeOperationMode.Save:
                    if (error || (int)(ticks - LastMicBitActivityTact) > SAVE_STOP_SILENCE)
                    {
                        LeaveSaveMode();
                    }
                    return;
                case TapeOperationMode.Load:
                    if (Player.Eof || error)
                    {
                        LeaveLoadMode();
                    }
                    return;
            }
        }

        /// <summary>
        /// Processes the the change of the MIC bit
        /// </summary>
        /// <param name="micBit"></param>
        public void ProcessMicBitValue(bool micBit)
        {
            if (CurrentMode != TapeOperationMode.Save
                || MicBitState == micBit)
            {
                return;
            }

            // --- Record the pulse
            var currentTact = _cpu.Tacts;
            SavedPulses.Add(new MicBitPulse
            {
                MicBit = micBit,
                Lenght = (int)(currentTact - LastMicBitActivityTact)
            });
            MicBitState = micBit;
            LastMicBitActivityTact = currentTact;
        }

        /// <summary>
        /// Puts the device in save mode. From now on, every MIC pulse is recorded
        /// </summary>
        private void EnterSaveMode()
        {
            CurrentMode = TapeOperationMode.Save;
            MicBitState = true;
            LastMicBitActivityTact = SaveStartTact = _cpu.Tacts;
            SavedPulses.Clear();
        }

        /// <summary>
        /// Leaves the save mode. Stops recording MIC pulses
        /// </summary>
        private void LeaveSaveMode()
        {
            CurrentMode = TapeOperationMode.Passive;
        }

        /// <summary>
        /// Puts the device in load mode. From now on, EAR pulses are played by a device
        /// </summary>
        private void EnterLoadMode()
        {
            CurrentMode = TapeOperationMode.Load;
            if (ContentProvider == null) return;

            var contentReader = ContentProvider.GetTzxContent();
            Player = new TzxPlayer(contentReader);
            Player.ReadContent();
            Player.InitPlay(_cpu.Tacts);
        }

        /// <summary>
        /// Leaves the load mode. Stops the device that playes EAR pulses
        /// </summary>
        private void LeaveLoadMode()
        {
            CurrentMode = TapeOperationMode.Passive;
            Player = null;
            ContentProvider?.Reset();
        }
    }
}