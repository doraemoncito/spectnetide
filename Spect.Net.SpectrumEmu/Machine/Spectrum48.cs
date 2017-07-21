﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Devices.Border;
using Spect.Net.SpectrumEmu.Devices.Interrupt;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Keyboard;
using Spect.Net.SpectrumEmu.Providers;
// ReSharper disable ConvertToAutoProperty

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents a ZX Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48: ISpectrumVm, ISpectrumVmTestSupport
    {
        private readonly int _frameTacts;
        private readonly List<ISpectrumBoundDevice> _spectrumDevices = new List<ISpectrumBoundDevice>();
        private readonly List<IFrameBoundDevice> _frameBoundDevices;

        /// <summary>
        /// The CPU tick at which the last frame rendering started;
        /// </summary>
        public long LastFrameStartCpuTick;

        /// <summary>
        /// The last rendered ULA tact 
        /// </summary>
        public int LastRenderedUlaTact;

        /// <summary>
        /// The length of the physical frame in clock counts
        /// </summary>
        public double PhysicalFrameClockCount { get; }

        /// <summary>
        /// The Z80 CPU of the machine
        /// </summary>
        public IZ80Cpu Cpu { get; }

        /// <summary>
        /// The memory device used by the virtual machine
        /// </summary>
        public ISpectrumMemoryDevice MemoryDevice { get; }

        /// <summary>
        /// The port device used by the virtual machine
        /// </summary>
        public ISpectrumPortDevice PortDevice { get; }

        /// <summary>
        /// The clock used within the VM
        /// </summary>
        public IClockProvider Clock { get; }

        /// <summary>
        /// The ULA border device used within the VM
        /// </summary>
        public IBorderDevice BorderDevice { get; }

        /// <summary>
        /// The ULA device that renders the VM screen
        /// </summary>
        public IScreenDevice ScreenDevice { get; }

        /// <summary>
        /// The ULA device that can render the VM screen during
        /// a debugging session
        /// </summary>
        public IScreenDevice ShadowScreenDevice { get; }

        /// <summary>
        /// The ULA device that takes care of raising interrupts
        /// </summary>
        public IInterruptDevice InterruptDevice { get; }

        /// <summary>
        /// The current status of the keyboard
        /// </summary>
        public IKeyboardDevice KeyboardDevice { get; }

        /// <summary>
        /// The beeper device attached to the VM
        /// </summary>
        public IBeeperDevice BeeperDevice { get; }

        /// <summary>
        /// The tape device attached to the VM
        /// </summary>
        public ITapeDevice TapeDevice { get; }

        /// <summary>
        /// Debug info provider object
        /// </summary>
        public IDebugInfoProvider DebugInfoProvider { get; private set; }

        /// <summary>
        /// The number of frame tact at which the interrupt signal is generated
        /// </summary>
        public int InterruptTact => 32;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Spectrum48(
            IRomProvider romProvider,
            IClockProvider clockProvider,
            IScreenPixelRenderer pixelRenderer,
            IEarBitPulseProcessor earBitPulseProcessor = null,
            ITzxTapeContentProvider tapeContentProvider = null)
        {
            // --- Init the CPU 
            MemoryDevice = new Spectrum48MemoryDevice();
            PortDevice = new Spectrum48PortDevice();
            Cpu = new Z80Cpu(MemoryDevice, PortDevice);

            // --- Setup the clock
            Clock = clockProvider;

            // --- Set up Spectrum devices
            BorderDevice = new BorderDevice();
            ScreenDevice = new Spectrum48ScreenDevice(pixelRenderer);
            ShadowScreenDevice = new Spectrum48ScreenDevice(pixelRenderer);
            BeeperDevice = new BeeperDevice(earBitPulseProcessor);
            KeyboardDevice = new KeyboardDevice();
            InterruptDevice = new InterruptDevice(InterruptTact);
            TapeDevice = new TapeDevice(tapeContentProvider);

            // --- Carry out frame calculations

            ResetUlaTact();
            _frameTacts = ScreenDevice.ScreenConfiguration.UlaFrameTactCount;
            PhysicalFrameClockCount = Clock.GetFrequency() / (double)ClockFrequeny * _frameTacts;
            FrameCount = 0;

            // --- Collect Spectrum devices
            _spectrumDevices.Add(MemoryDevice);
            _spectrumDevices.Add(PortDevice);
            _spectrumDevices.Add(BorderDevice);
            _spectrumDevices.Add(ScreenDevice);
            _spectrumDevices.Add(ShadowScreenDevice);
            _spectrumDevices.Add(BeeperDevice);
            _spectrumDevices.Add(KeyboardDevice);
            _spectrumDevices.Add(InterruptDevice);
            _spectrumDevices.Add(TapeDevice);

            // --- Now, prepare devices to find each other
            foreach (var device in _spectrumDevices)
            {
                device.OnAttachedToVm(this);
            }

            // --- Prepare frame bound devices
            _frameBoundDevices = _spectrumDevices.Where(d => d is IFrameBoundDevice).OfType<IFrameBoundDevice>()
                .ToList();

            DebugInfoProvider = new NoopDebugInfoProvider();

            // --- Init the ROM
            InitRom(romProvider, "ZXSpectrum48.rom");
        }

        /// <summary>
        /// Resets the CPU and the ULA chip
        /// </summary>
        public void Reset()
        {
            Cpu.Reset();
            ResetUlaTact();
            foreach (var device in _spectrumDevices)
            {
                device.Reset();
            }
        }

        /// <summary>
        /// #of frames rendered
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// #of tacts within the frame
        /// </summary>
        public int FrameTacts => _frameTacts;

        /// <summary>
        /// Gets the current frame tact according to the CPU tick count
        /// </summary>
        public virtual int CurrentFrameTact => (int) (Cpu.Tacts - LastFrameStartCpuTick);

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Resets the ULA tact to start screen rendering from the beginning
        /// </summary>
        public void ResetUlaTact()
        {
            LastRenderedUlaTact = -1;
        }

        /// <summary>
        /// Sets the debug info provider to the specified object
        /// </summary>
        /// <param name="provider">Provider object</param>
        public void SetDebugInfoProvider(IDebugInfoProvider provider)
        {
            DebugInfoProvider = provider;
        }

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="mode">Execution emulation mode</param>
        /// <param name="stepMode">Debugging execution mode</param>
        /// <return>True, if the cycle completed; false, if it has been cancelled</return>
        public bool ExecuteCycle(CancellationToken token, EmulationMode mode = EmulationMode.Continuous,
            DebugStepMode stepMode = DebugStepMode.StopAtBreakpoint)
        {
            return ExecuteCycle(token, new ExecuteCycleOptions(mode, stepMode, true));
        }

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="options">Execution options</param>
        /// <return>True, if the cycle completed; false, if it has been cancelled</return>
        public bool ExecuteCycle(CancellationToken token, ExecuteCycleOptions options)
        {
            // --- Prepare the cycle
            LastFrameStartCpuTick = Cpu.Tacts;
            var cycleStartTime = Clock.GetCounter();
            var cycleFrameCount = 0;
            if (options.EmulationMode == EmulationMode.Continuous)
            {
                ResetUlaTact();
            }
            var executedInstructionCount = -1;

            // --- Loop #1: The main cycle that goes on until cancelled
            while (!token.IsCancellationRequested)
            {
                // --- Prepare for the next screen frame
                var frameCompleted = false;
                var currentFrameStartCounter = Clock.GetCounter();

                DebugInfoProvider.CpuTime = 0;
                DebugInfoProvider.ScreenRenderingTime = 0;

                // --- Loop #2: The physical frame cycle tha goes on while CPU and ULA 
                // --- processes everything whithin a physical frame (0.019968 second)
                while (!frameCompleted)
                {
                    // --- Check debug mode when a CPU instruction has been entirelly executed
                    if (!Cpu.IsInOpExecution)
                    {
                        // --- Check for cancellation
                        if (token.IsCancellationRequested)
                        {
                            return false;
                        }

                        // --- The next instruction is about to be executed
                        executedInstructionCount++;

                        // --- Check for a debugging stop point
                        if (options.EmulationMode == EmulationMode.Debugger)
                        {
                            if (IsDebugStop(options.DebugStepMode, executedInstructionCount))
                            {
                                // --- At this point, the cycle should be stopped because of debugging reasons
                                // --- The screen should be refreshed
                                ScreenDevice.OnFrameCompleted();
                                return true;
                            }
                        }
                    }

                    // --- Check for interrupt signal generation
                    InterruptDevice.CheckForInterrupt(CurrentFrameTact);

                    // --- Run a single Z80 instruction
                    var cpuStart = Clock.GetCounter();
                    Cpu.ExecuteCpuCycle();
                    DebugInfoProvider.CpuTime += (ulong) (Clock.GetCounter() - cpuStart);

                    // --- Run a rendering cycle according to the current CPU tact count
                    var lastTact = CurrentFrameTact;
                    var renderStart = Clock.GetCounter();
                    ScreenDevice.RenderScreen(LastRenderedUlaTact + 1, lastTact);
                    DebugInfoProvider.ScreenRenderingTime += (ulong) (Clock.GetCounter() - renderStart);
                    LastRenderedUlaTact = lastTact;

                    // --- Exit if the emulation mode specifies so
                    if (options.EmulationMode == EmulationMode.UntilHalt 
                        && (Cpu.StateFlags & Z80StateFlags.Halted) != 0)
                    {
                        return true;
                    }

                    // --- Manage the tape device, trigger appropriate modes
                    TapeDevice.SetTapeMode();

                    // --- Decide whether this frame has been completed
                    frameCompleted = !Cpu.IsInOpExecution && CurrentFrameTact >= _frameTacts;

                } // -- End Loop #2

                // --- A physical frame has just been completed. Take care about screen refresh
                cycleFrameCount++;
                FrameCount++;

                // --- Calculate debug information
                DebugInfoProvider.FrameTime = (ulong)(Clock.GetCounter() - currentFrameStartCounter);
                DebugInfoProvider.UtilityTime = DebugInfoProvider.FrameTime - DebugInfoProvider.CpuTime
                                                - DebugInfoProvider.ScreenRenderingTime;
                DebugInfoProvider.CpuTimeInMs = DebugInfoProvider.CpuTime / (double) Clock.GetFrequency() * 1000;
                DebugInfoProvider.ScreenRenderingTimeInMs =
                    DebugInfoProvider.ScreenRenderingTime / (double) Clock.GetFrequency() * 1000;
                DebugInfoProvider.UtilityTimeInMs =
                    DebugInfoProvider.UtilityTime / (double) Clock.GetFrequency() * 1000;
                DebugInfoProvider.FrameTimeInMs = DebugInfoProvider.FrameTime / (double) Clock.GetFrequency() * 1000;

                BeeperDevice.OnFrameCompleted();
                ScreenDevice.OnFrameCompleted();

                // --- Exit if the emulation mode specifies so
                if (options.EmulationMode == EmulationMode.UntilFrameEnds)
                {
                    return true;
                }

                var nextFrameCounter = cycleStartTime + cycleFrameCount * PhysicalFrameClockCount;
                Clock.WaitUntil((long) nextFrameCounter, token);

                // --- Start a new frame and carry on
                var remainingTacts = CurrentFrameTact % _frameTacts;
                LastFrameStartCpuTick = Cpu.Tacts - remainingTacts;
                ScreenDevice.OnNewFrame();
                ScreenDevice.RenderScreen(0, remainingTacts);
                LastRenderedUlaTact = remainingTacts;

                // --- We start a new beeper frame, too
                BeeperDevice.OnNewFrame();

                // --- Reset the interrupt device
                InterruptDevice.OnNewFrame();

            } // --- End Loop #1
            return false;
        }

        /// <summary>
        /// Use this method to refresh the shadow screen while a
        /// debugging session is paused
        /// </summary>
        public void RefreshShadowScreen()
        {
            ShadowScreenDevice.OnNewFrame();
            ShadowScreenDevice.RenderScreen(0, _frameTacts - 1);
        }

        /// <summary>
        /// Checks whether the execution cycle should be stopped for debugging
        /// </summary>
        /// <param name="stepMode">Debug setp mode</param>
        /// <param name="executedInstructionCount">
        /// The count of instructions already executed in this cycle
        /// </param>
        /// <returns>True, if the execution should be stopped</returns>
        private bool IsDebugStop(DebugStepMode stepMode, int executedInstructionCount)
        {
            // --- No debug provider, no stop
            if (DebugInfoProvider == null) return false;

            // --- In Step-Into mode we always stop when we're about to
            // --- execute the next instruction
            if (stepMode == DebugStepMode.StepInto)
            {
                return executedInstructionCount > 0;
            }

            // --- In Stop-At-Breakpoint mode we stop only if a predefined
            // --- breakpoint is reached
            if (stepMode == DebugStepMode.StopAtBreakpoint
                && DebugInfoProvider.Breakpoints.Contains(Cpu.Registers.PC))
            {
                // --- We do not stop unless we executed at least one instruction
                return executedInstructionCount > 0;
            }

            // --- We're in Step-Over mode
            if (stepMode == DebugStepMode.StepOver)
            {
                if (DebugInfoProvider.ImminentBreakpoint != null)
                {
                    // --- We also stop, if an imminent breakpoint is reached, and also remove
                    // --- this breakpoint
                    if (DebugInfoProvider.ImminentBreakpoint == Cpu.Registers.PC)
                    {
                        DebugInfoProvider.ImminentBreakpoint = null;
                        return true;
                    }
                }
                else
                {
                    var imminentJustCreated = false;

                    // --- We check for a CALL-like instruction
                    var length = Cpu.GetCallInstructionLength();
                    if (length > 0)
                    {
                        // --- Its a CALL-like instraction, create an imminent breakpoint
                        DebugInfoProvider.ImminentBreakpoint = (ushort)(Cpu.Registers.PC + length);
                        imminentJustCreated = true;
                    }

                    // --- We stop, we executed at least one instruction and if there's no imminent 
                    // --- breakpoint or we've just created one
                    if (executedInstructionCount > 0
                        && (DebugInfoProvider.ImminentBreakpoint == null || imminentJustCreated))
                    {
                        return true;
                    }
                }
            }

            // --- In any other case, we carry on
            return false;
        }

        /// <summary>
        /// Writes a byte to the memory
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Data byte</param>
        public void WriteSpectrumMemory(ushort addr, byte value) =>
            MemoryDevice.OnWriteMemory(addr, value);

        #region I/O Access functions

        /// <summary>
        /// Writes the given <paramref name="value" /> to the
        /// given port specified in <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="value">Value to write</param>
        protected virtual void WritePort(ushort addr, byte value)
        {
            if ((addr & 0x0001) == 0)
            {
                BorderDevice.BorderColor = value & 0x07;
                BeeperDevice.ProcessEarBitValue((value & 0x10) != 0);
                TapeDevice.ProcessMicBitValue((value & 0x08) != 0);
            }
        }

        /// <summary>
        /// Reads a byte from the port specified in <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Value read from the port</returns>
        protected virtual byte ReadPort(ushort addr)
        {
            if ((addr & 0x0001) != 0) return 0xFF;

            var portBits = KeyboardDevice.GetLineStatus((byte) (addr >> 8));
            var earBit = TapeDevice.GetEarBit(Cpu.Tacts);
            if (!earBit)
            {
                portBits = (byte) (portBits & 0b1011_1111);
            }
            return portBits;
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Loads the content of the ROM through the specified provider
        /// </summary>
        /// <param name="romProvider">ROM provider instance</param>
        /// <param name="romResourceName">ROM Resource name</param>
        /// <remarks>
        /// The content of the ROM is copied into the memory
        /// </remarks>
        public void InitRom(IRomProvider romProvider, string romResourceName)
        {
            var romBytes = romProvider.LoadRom(romResourceName);
            MemoryDevice.FillMemory(romBytes);
        }

        #endregion

        /// <summary>
        /// This is a no operation debug info provider
        /// </summary>
        private class NoopDebugInfoProvider : IDebugInfoProvider
        {
            /// <summary>
            /// The component provider should be able to reset itself
            /// </summary>
            public void Reset()
            {
            }

            public NoopDebugInfoProvider()
            {
                Breakpoints = new BreakpointCollection();
            }

            /// <summary>
            /// The currently defined breakpoints
            /// </summary>
            public BreakpointCollection Breakpoints { get; }

            /// <summary>
            /// Gets or sets an imminent breakpoint
            /// </summary>
            public ushort? ImminentBreakpoint { get; set; }

            /// <summary>
            /// Entire time spent within a single ULA frame
            /// </summary>
            public ulong FrameTime { get; set; }

            /// <summary>
            /// Time spent with executing CPU instructions
            /// </summary>
            public ulong CpuTime { get; set; }

            /// <summary>
            /// Time spent with screen rendering
            /// </summary>
            public ulong ScreenRenderingTime { get; set; }

            /// <summary>
            /// Time spent with other utility activities
            /// </summary>
            public ulong UtilityTime { get; set; }

            /// <summary>
            /// Entire time spent within a single ULA frame
            /// </summary>
            public double FrameTimeInMs { get; set; }

            /// <summary>
            /// Time spent with executing CPU instructions
            /// </summary>
            public double CpuTimeInMs { get; set; }

            /// <summary>
            /// Time spent with screen rendering
            /// </summary>
            public double ScreenRenderingTimeInMs { get; set; }

            /// <summary>
            /// Time spent with other utility activities
            /// </summary>
            public double UtilityTimeInMs { get; set; }
        }

        /// <summary>
        /// Gets the frequency of the virtual machine's clock in Hz
        /// </summary>
        public int ClockFrequeny => 3_500_000;
    }
}