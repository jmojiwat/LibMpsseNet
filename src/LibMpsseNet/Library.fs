namespace Ftdi.LibMpsse

module internal Infrastructure =
    
    open System.Runtime.CompilerServices
    open Microsoft.FSharp.Core
    open Ftdi.LibMpsse
    open Ftdi.LibMpsse.Interop

    
    [<assembly: InternalsVisibleTo("LibMpsseNet.Tests")>]
    do ()
    
    let toDeviceError = function
        | FT_STATUS.FT_INVALID_HANDLE -> InvalidHandle 
        | FT_STATUS.FT_DEVICE_NOT_FOUND -> DeviceNotFound 
        | FT_STATUS.FT_DEVICE_NOT_OPENED -> DeviceNotOpened 
        | FT_STATUS.FT_IO_ERROR -> IoError 
        | FT_STATUS.FT_INSUFFICIENT_RESOURCES -> InsufficientResources 
        | FT_STATUS.FT_INVALID_PARAMETER -> InvalidParameter 
        | FT_STATUS.FT_INVALID_BAUD_RATE -> InvalidBaudrate 
        | FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_ERASE -> DeviceNotOpenedForErase 
        | FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_WRITE -> DeviceNotOpenedForWrite 
        | FT_STATUS.FT_FAILED_TO_WRITE_DEVICE -> FailedToWriteDevice  
        | FT_STATUS.FT_EEPROM_READ_FAILED -> EepromReadFailed  
        | FT_STATUS.FT_EEPROM_WRITE_FAILED -> EepromWriteFailed  
        | FT_STATUS.FT_EEPROM_ERASE_FAILED -> EepromEraseFailed  
        | FT_STATUS.FT_EEPROM_NOT_PRESENT -> EepromNotPresent  
        | FT_STATUS.FT_EEPROM_NOT_PROGRAMMED -> EepromNotProgrammed  
        | FT_STATUS.FT_INVALID_ARGS -> InvalidArgs  
        | FT_STATUS.FT_NOT_SUPPORTED -> NotSupported
        | FT_STATUS.FT_OTHER_ERROR -> OtherError
        | FT_STATUS.FT_DEVICE_LIST_NOT_READY -> DeviceListNotReady
        | _ -> OtherError

    [<Literal>]
    let FT_FLAGS_OPENED = 1u
    
    [<Literal>]
    let FT_FLAGS_CLOSED = 0u

    [<Literal>]
    let FT_FLAGS_HIGHSPEED = 2u
    
    [<Literal>]
    let FT_FLAGS_FULLSPEED = 0u
    
    let devicePortState flags =
        match flags ||| FT_FLAGS_OPENED with
        | 1u -> PortOpened
        | _ -> PortClosed
        
    let internal deviceSpeed flags =
        match flags ||| FT_FLAGS_HIGHSPEED with
        | 2u -> HighSpeed
        | _ -> FullSpeed
    
    let internal toDeviceType = function
        | FT_DEVICE.FT_DEVICE_BM -> DeviceBM
        | FT_DEVICE.FT_DEVICE_AM -> DeviceAM
        | FT_DEVICE.FT_DEVICE_100AX -> Device100AX
        | FT_DEVICE.FT_DEVICE_UNKNOWN -> DeviceUnknown
        | FT_DEVICE.FT_DEVICE_2232C -> Device2232
        | FT_DEVICE.FT_DEVICE_232R -> Device232R
        | FT_DEVICE.FT_DEVICE_2232H -> Device2232H
        | FT_DEVICE.FT_DEVICE_4232H -> Device4232H
        | FT_DEVICE.FT_DEVICE_232H -> Device232H
        | FT_DEVICE.FT_DEVICE_X_SERIES -> DeviceXSeries
        | FT_DEVICE.FT_DEVICE_4222H_0 -> Device4222HMode0
        | FT_DEVICE.FT_DEVICE_4222H_1_2 -> Device4222HMode1Or2
        | FT_DEVICE.FT_DEVICE_4222H_3 -> Device4222HMode3
        | FT_DEVICE.FT_DEVICE_4222_PROG -> DeviceFT4222Prog
        | FT_DEVICE.FT_DEVICE_900 -> DeviceFT900
        | FT_DEVICE.FT_DEVICE_930 -> DeviceFT930
        | FT_DEVICE.FT_DEVICE_UMFTPD3A -> DeviceUMFTPD3A
        | FT_DEVICE.FT_DEVICE_2233HP -> Device2233HP
        | FT_DEVICE.FT_DEVICE_4233HP -> Device4233HP
        | FT_DEVICE.FT_DEVICE_2232HP -> Device2232HP
        | FT_DEVICE.FT_DEVICE_4232HP -> Device4232HP
        | FT_DEVICE.FT_DEVICE_233HP -> Device233HP
        | FT_DEVICE.FT_DEVICE_232HP -> Device232HP
        | FT_DEVICE.FT_DEVICE_2232HA -> Device2232HA
        | FT_DEVICE.FT_DEVICE_4232HA -> Device4232HA
        | _ -> DeviceUnknown
    
    let toChannelInfo (dlin: FT_DEVICE_LIST_INFO_NODE) =
        let portState = devicePortState dlin.Flags
        let speed = deviceSpeed dlin.Flags
        let device = toDeviceType dlin.Type
        { PortState = portState
          Speed = speed
          Device = device
          Id = dlin.Id
          LocId = dlin.LocId
          SerialNumber = dlin.SerialNumber
          Description = dlin.Description
          Handle = dlin.Handle }
            
    
    let inline logicalOr accumulator tuple =
        let offset, value = tuple
        accumulator ||| (value <<< offset)

module I2cClockRate =
    
    open UnitsNet
    
    let StandardMode = Frequency.FromHertz(100_000)
    
    let FastMode = Frequency.FromHertz(400_000)
    
    let FastModePlus = Frequency.FromHertz(1_000_000)
    
    let HighSpeedMode = Frequency.FromHertz(3_400_000)
    

module I2c =
    
    open System
    open System.Runtime.InteropServices
    open Microsoft.FSharp.Core
    open Ftdi.LibMpsse.Interop
    open Ftdi.LibMpsse.Interop.I2c
    open Infrastructure
    open UnitsNet
    
    type I2cChannelConfig =
        { ClockRate: Frequency
          LatencyTimer: uint8
          DisableI2c3PhaseClocking: bool
          EnableI2cDriveOnlyZero: bool }
    
    type I2cTransferOptions =
        { StartBit: bool
          StopBit: bool
          BreakOnNak: bool
          FastTransferBytes: bool
          FastTransferBits: bool
          NoAddress: bool }
    
    let i2cGetNumChannels () =
        let mutable numChannels = 0u
        
//        let status = I2C_GetNumChannels(&numChannels)
        
        match I2C_GetNumChannels(&numChannels) with
        | FT_STATUS.FT_OK ->
            Ok numChannels
        | status ->
            Error (toDeviceError status)

    let i2cGetChannelInfo index =
        let mutable deviceListInfoNode = FT_DEVICE_LIST_INFO_NODE()
        let size = Marshal.SizeOf(deviceListInfoNode)
        let pointer = Marshal.AllocHGlobal(size)
        Marshal.StructureToPtr(deviceListInfoNode, pointer, false)
        
        let status = I2C_GetChannelInfo(index, &deviceListInfoNode)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok (toChannelInfo deviceListInfoNode)
        | _ ->
            Error (toDeviceError status)

    let i2cOpenChannel index =
        let mutable handle = IntPtr.Zero
        let status = I2C_OpenChannel(index, &handle)

        match status with
        | FT_STATUS.FT_OK ->
            Ok { Handle = handle }
        | _ ->
            Error (toDeviceError status)
        
    [<Literal>]
    let internal I2cDisable3PhaseClockingOffset = 0
    
    [<Literal>]
    let internal I2cEnableDriveOnlyZeroOffset = 1
    
    let internal mapToI2cChannelConfigStruct (config: I2cChannelConfig) =
        let options =
            [ config.DisableI2c3PhaseClocking; config.EnableI2cDriveOnlyZero ]
            |> List.map (fun v -> if v then 1u else 0u)
            |> List.zip [ I2cDisable3PhaseClockingOffset; I2cEnableDriveOnlyZeroOffset; ]
            |> List.fold logicalOr 0u

        let mutable channelConfig = CHANNEL_CONFIG()
        channelConfig.ClockRate <- config.ClockRate.Hertz |> uint32
        channelConfig.LatencyTimer <- config.LatencyTimer
        channelConfig.Options <- options
        channelConfig
        
    let i2cInitChannel config channel =
        let mutable channelConfig = mapToI2cChannelConfigStruct config
        let status = I2C_InitChannel(channel.Handle, &channelConfig)

        match status with
        | FT_STATUS.FT_OK ->
            Ok channel
        | _ ->
            Error (toDeviceError status)
        
    let i2cCloseChannel channel =
        let status = I2C_CloseChannel(channel.Handle)

        match status with
        | FT_STATUS.FT_OK ->
            Ok ()
        | _ ->
            Error (toDeviceError status)

    [<Literal>]
    let internal I2cStartBitOffset = 0
        
    [<Literal>]
    let internal I2cStopBitOffset = 1
        
    [<Literal>]
    let internal I2cBreakOnNakOffset = 2
        
    [<Literal>]
    let internal I2cFastTransferBytesOffset = 4
        
    [<Literal>]
    let internal I2cFastTransferBitsOffset = 5
        
    [<Literal>]
    let internal I2cNoAddressOffset = 6
        
    let internal toI2cTransferOptions transferOptions =
        [ transferOptions.StartBit
          transferOptions.StopBit
          transferOptions.BreakOnNak
          transferOptions.FastTransferBytes
          transferOptions.FastTransferBits
          transferOptions.NoAddress ]
        |> List.map (fun v -> if v then 1u else 0u)
        |> List.zip [ I2cStartBitOffset
                      I2cStopBitOffset
                      I2cBreakOnNakOffset
                      I2cFastTransferBytesOffset
                      I2cFastTransferBitsOffset
                      I2cNoAddressOffset ]
        |> List.fold logicalOr 0u

    let i2cDeviceRead channel options deviceAddress (buffer: byte[]) =
        let transferOptions = toI2cTransferOptions options
        let sizeToTransfer = buffer.Length |> uint32
        let mutable sizeTransferred = 0u
        
        let status = I2C_DeviceRead(channel.Handle, deviceAddress, sizeToTransfer, buffer, &sizeTransferred, transferOptions)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok sizeTransferred
        | _ ->
            Error (toDeviceError status)
        
    let i2cDeviceWrite channel options deviceAddress (buffer: byte[]) =
        let transferOptions = toI2cTransferOptions options
        let sizeToTransfer = buffer.Length |> uint32
        let mutable sizeTransferred = 0u

        let status = I2C_DeviceWrite(channel.Handle, deviceAddress, sizeToTransfer, buffer, &sizeTransferred, transferOptions)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok sizeTransferred
        | _ ->
            Error (toDeviceError status)

    let defaultConfig =
        { ClockRate = I2cClockRate.FastMode
          LatencyTimer = 255uy
          DisableI2c3PhaseClocking = false
          EnableI2cDriveOnlyZero = false }
        
    let getMpsseDevice config index =
        i2cOpenChannel index
        |> Result.bind (i2cInitChannel config)
        |> function
            | Ok device -> device
            | Error _ -> invalidArg "index" "No device found here"
        
    let getMpsseDeviceWithDefaultConfig = getMpsseDevice defaultConfig
    
    let defaultTransferOptions =
        { StartBit = true
          StopBit = false
          BreakOnNak = false
          FastTransferBytes = false
          FastTransferBits = false
          NoAddress = false }
        
    let readByte channel transferOptions deviceAddress =
        let buffer: byte[] = Array.zeroCreate 1
                    
        i2cDeviceRead channel transferOptions deviceAddress buffer
        |> Result.map (fun _ -> buffer[0])
        
    let read channel transferOptions deviceAddress buffer =
        i2cDeviceRead channel transferOptions deviceAddress buffer
        |> Result.map (fun _ -> ())
        
    let writeByte channel transferOptions deviceAddress data =
        let buffer = [| data |]

        i2cDeviceWrite channel transferOptions deviceAddress buffer 
        |> function
            | Ok _ -> Ok ()
            | Error e -> Error e

    let write channel transferOptions deviceAddress data =
        i2cDeviceWrite channel transferOptions deviceAddress data 
        |> function
            | Ok _ -> Ok ()
            | Error e -> Error e
            
    let writeRead channel transferOptions deviceAddress writeBuffer readBuffer =
        i2cDeviceWrite channel transferOptions deviceAddress writeBuffer
        |> Result.bind (fun _ -> i2cDeviceRead channel transferOptions deviceAddress readBuffer)
        |> Result.map ignore

module Spi =
    
    open System
    open System.Runtime.InteropServices
    open Microsoft.FSharp.Core
    open Ftdi.LibMpsse.Interop
    open Ftdi.LibMpsse.Interop.Spi
    open Infrastructure
    open UnitsNet
    
    type SpiMode =
        | SpiMode0
        | SpiMode1
        | SpiMode2
        | SpiMode3
        
    type ChipSelectLine =
        | Dbus3Line
        | Dbus4Line
        | Dbus5Line
        | Dbus6Line
        | Dbus7Line
        
    type ChipSelectActive =
        | ActiveLow
        | ActiveHigh
        
    type SpiChannelConfig =
        { ClockRate: Frequency
          LatencyTimer: uint8
          SpiMode: SpiMode
          ChipSelectLine: ChipSelectLine
          ChipSelectActive: ChipSelectActive }
    
    type SpiTransferOptions =
        { SizeInBits: bool
          ChipSelectEnable: bool
          ChipSelectDisable: bool }
    
    
    let spiGetNumChannels () =
        let mutable numChannels = 0u
        
        let status = SPI_GetNumChannels(&numChannels)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok numChannels
        | _ ->
            Error (toDeviceError status)

    let spiGetChannelInfo index =
        let mutable deviceListInfoNode = FT_DEVICE_LIST_INFO_NODE()
        let size = Marshal.SizeOf(deviceListInfoNode)
        let pointer = Marshal.AllocHGlobal(size)
        Marshal.StructureToPtr(deviceListInfoNode, pointer, false)
        
        let status = SPI_GetChannelInfo(index, &deviceListInfoNode)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok (toChannelInfo deviceListInfoNode)
        | _ ->
            Error (toDeviceError status)

    let spiOpenChannel index =
        let mutable handle = IntPtr.Zero
        let status = SPI_OpenChannel(index, &handle)

        match status with
        | FT_STATUS.FT_OK ->
            Ok { Handle = handle }
        | _ ->
            Error (toDeviceError status)
        
    let internal toSpiModeValue = function
        | SpiMode0 -> 0u
        | SpiMode1 -> 1u
        | SpiMode2 -> 2u
        | SpiMode3 -> 3u
        
    let internal toChipSelectLineValue = function
        | Dbus3Line -> 0u
        | Dbus4Line -> 1u
        | Dbus5Line -> 2u
        | Dbus6Line -> 3u
        | Dbus7Line -> 4u
          
    let internal toChipSelectActiveValue = function
        | ActiveHigh -> 0u
        | ActiveLow -> 1u

    [<Literal>]
    let internal SpiConfigModeOffset = 0

    [<Literal>]
    let internal SpiConfigChipSelectLineOffset = 2
    
    [<Literal>]
    let internal SpiConfigChipSelectActiveOffset = 5
    
    let internal mapToSpiChannelConfigStruct (config: SpiChannelConfig) =
        let options =
            [ toSpiModeValue config.SpiMode
              toChipSelectLineValue config.ChipSelectLine
              toChipSelectActiveValue config.ChipSelectActive ]
            |> List.zip [ SpiConfigModeOffset; SpiConfigChipSelectLineOffset; SpiConfigChipSelectActiveOffset ]
            |> List.fold logicalOr 0u
        
        let mutable channelConfig = CHANNEL_CONFIG()
        channelConfig.ClockRate <- config.ClockRate.Hertz |> uint32
        channelConfig.LatencyTimer <- config.LatencyTimer
        channelConfig.Options <- options
        channelConfig
        
    let spiInitChannel channel config =
        let mutable channelConfig = mapToSpiChannelConfigStruct config
        let status = SPI_InitChannel(channel.Handle, &channelConfig)

        match status with
        | FT_STATUS.FT_OK ->
            Ok channel
        | _ ->
            Error (toDeviceError status)
        
    let spiCloseChannel channel =
        let status = SPI_CloseChannel(channel.Handle)

        match status with
        | FT_STATUS.FT_OK ->
            Ok ()
        | _ ->
            Error (toDeviceError status)

    [<Literal>]
    let internal SpiSizeInBitsOffset = 0
    
    [<Literal>]
    let internal SpiChipSelectEnableOffset = 1
    
    [<Literal>]
    let internal SpiChipSelectDisableOffset = 2
    
    let internal toSpiTransferOptions transferOptions =
        [ transferOptions.SizeInBits
          transferOptions.ChipSelectEnable
          transferOptions.ChipSelectDisable ]
        |> List.map (fun v -> if v then 1u else 0u)
        |> List.zip [ SpiSizeInBitsOffset; SpiChipSelectEnableOffset; SpiChipSelectDisableOffset ]
        |> List.fold logicalOr 0u
        
    let spiRead channel options (buffer: byte[]) =
        let transferOptions = toSpiTransferOptions options
        let sizeToTransfer = buffer.Length |> uint32
        let mutable sizeTransferred = 0u

        let status = SPI_Read(channel.Handle, buffer, sizeToTransfer, &sizeTransferred, transferOptions)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok sizeTransferred
        | _ ->
            Error (toDeviceError status)
        
    let spiWrite channel options (buffer: byte[]) =
        let transferOptions = toSpiTransferOptions options
        let sizeToTransfer = buffer.Length |> uint32
        let mutable sizeTransferred = 0u

        let status = SPI_Write(channel.Handle, buffer, sizeToTransfer, &sizeTransferred, transferOptions)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok sizeTransferred
        | _ ->
            Error (toDeviceError status)
                
    let spiReadWrite channel options (readBuffer: byte[], writeBuffer: byte[]) =
        let transferOptions = toSpiTransferOptions options
        
        let writeSizeToTransfer = writeBuffer.Length |> uint32
        let mutable writeSizeTransferred = 0u

        let status = SPI_ReadWrite(channel.Handle, readBuffer, writeBuffer, writeSizeToTransfer, &writeSizeTransferred, transferOptions)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok writeSizeTransferred
        | _ ->
            Error (toDeviceError status)
                
    let spiIsBusy channel =
        let mutable state = false
        let status = SPI_IsBusy(channel.Handle, &state)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok state
        | _ ->
            Error (toDeviceError status)
        
module Gpio =
    
    open System.Collections
    open Ftdi.LibMpsse.Interop
    open Ftdi.LibMpsse.Interop.Gpio
    open Infrastructure
    open Ftdi.LibMpsse
    
    type GpioState =
        | Input
        | OutputHigh
        | OutputLow
        
    type GpioPins =
        { Cbus0: GpioState
          Cbus1: GpioState
          Cbus2: GpioState
          Cbus3: GpioState
          Cbus4: GpioState
          Cbus5: GpioState
          Cbus6: GpioState
          Cbus7: GpioState }
    
    type GpioInput =
        | High
        | Low
        | None
        
    type GpioPinInputReading =
        { Cbus0Input: GpioInput
          Cbus1Input: GpioInput
          Cbus2Input: GpioInput
          Cbus3Input: GpioInput
          Cbus4Input: GpioInput
          Cbus5Input: GpioInput
          Cbus6Input: GpioInput
          Cbus7Input: GpioInput }
        
    let internal gpioStateToDirection = function
        | Input -> 0uy
        | _ -> 1uy
            
    let internal gpioStateToValue = function
        | OutputLow -> 0uy
        | OutputHigh -> 1uy
        | Input -> 0uy
        
    let internal toGpioDirectionValue gpioPins =
        let pins =
          [ gpioPins.Cbus0
            gpioPins.Cbus1
            gpioPins.Cbus2
            gpioPins.Cbus3
            gpioPins.Cbus4
            gpioPins.Cbus5
            gpioPins.Cbus6
            gpioPins.Cbus7 ]
          
        let directions =
            List.map gpioStateToDirection pins
            |> List.zip [ 0; 1; 2; 3; 4; 5; 6; 7 ]
            |> List.fold logicalOr 0uy
            
        let values =
            List.map gpioStateToValue pins
            |> List.zip [ 0; 1; 2; 3; 4; 5; 6; 7 ]
            |> List.fold logicalOr 0uy
            
        directions, values

    let ftWriteGpio channel gpioPins =
        let dir, value = toGpioDirectionValue gpioPins
        let status = FT_WriteGPIO(channel.Handle, dir, value)

        match status with
        | FT_STATUS.FT_OK ->
            Ok ()
        | _ ->
            Error (toDeviceError status)

    let internal toGpioInputReading (value: byte) =
        let bitArray = BitArray([| value |])
        let bits = Array.create bitArray.Count false
        bitArray.CopyTo(bits, 0)
        let result =
            Seq.map (fun bit -> if bit then High else Low) bits
            |> Seq.rev
            |> Array.ofSeq

        { Cbus0Input = result[0]
          Cbus1Input = result[1]
          Cbus2Input = result[2]
          Cbus3Input = result[3]
          Cbus4Input = result[4]
          Cbus5Input = result[5]
          Cbus6Input = result[6]
          Cbus7Input = result[7] }        
            
    let ftReadGpio channel =
        let mutable value = 0uy
        let status = FT_ReadGPIO(channel.Handle, &value)
        
        match status with
        | FT_STATUS.FT_OK ->
            Ok (toGpioInputReading value)
        | _ ->
            Error (toDeviceError status)
    
module UsbSerialBridge =

    open System    
    open Ftdi.LibMpsse.Interop.Infrastructure
    

    type internal MpsseState =
        | Uninitialised
        | Initialised
        
    let mutable internal mpsseState = Uninitialised
    
    let initLibMpsse () =
        let monitor = Object()
        lock monitor (fun () ->
            match mpsseState with
            | Uninitialised ->
                Init_libMPSSE()
                mpsseState <- Initialised
            | _ ->
                ())
        
    let cleanupLibMpsse () =
        let monitor = Object()
        lock monitor (fun () ->
            match mpsseState with
            | Initialised ->
                Cleanup_libMPSSE()
                mpsseState <- Uninitialised
            | _ ->
                ())

    let useLibMpsse () =
        initLibMpsse ()
        { new IDisposable with
            member this.Dispose() = cleanupLibMpsse () }


    let toErrorString =
        function
        | InvalidHandle -> "Invalid Handle"
        | DeviceNotFound -> "Device Not found"
        | DeviceNotOpened -> "Device Not Opened"
        | IoError -> "IO Error"
        | InsufficientResources -> "Insufficient Resources"
        | InvalidParameter -> "Invalid Parameter"
        | InvalidBaudrate -> "Invalid Baudrate"
        | DeviceNotOpenedForErase -> "Device Not Opened For Erase"
        | DeviceNotOpenedForWrite -> "Device Not Opened For Write"
        | FailedToWriteDevice -> "Failed To Write Device"
        | EepromReadFailed -> "EEPROM Read Failed"
        | EepromWriteFailed -> "EEPROM Write Failed"
        | EepromEraseFailed -> "EEPROM Erase Failed"
        | EepromNotPresent -> "EEPROM Not Present"
        | EepromNotProgrammed -> "EEPROM Not Programmed"
        | InvalidArgs -> "Invalid Args"
        | NotSupported -> "Not Supported"
        | OtherError -> "Other Error"
        | DeviceListNotReady -> "Device List Not Ready"

