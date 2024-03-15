namespace Ftdi.LibMpsse

open System
open UnitsNet

type DeviceError =
    /// The device handle is invalid
    | InvalidHandle
    /// Device not found
    | DeviceNotFound
    /// Device is not open
    | DeviceNotOpened
    /// IO error
    | IoError
    /// Insufficient resources
    | InsufficientResources
    /// A parameter was invalid
    | InvalidParameter
    /// The requested baud rate is invalid
    | InvalidBaudrate
    /// Device not opened for erase
    | DeviceNotOpenedForErase
    /// Device not poened for write
    | DeviceNotOpenedForWrite
    /// Failed to write to device
    | FailedToWriteDevice
    /// Failed to read the device EEPROM
    | EepromReadFailed
    /// Failed to write the device EEPROM
    | EepromWriteFailed
    /// Failed to erase the device EEPROM
    | EepromEraseFailed
    /// An EEPROM is not fitted to the device
    | EepromNotPresent
    /// Device EEPROM is blank
    | EepromNotProgrammed
    /// Invalid arguments
    | InvalidArgs
    | NotSupported
    /// An other error has occurred
    | OtherError
    | DeviceListNotReady

type DeviceType =
    /// FT232B or FT245B device
    | DeviceBM
    /// FT8U232AM or FT8U245AM device
    | DeviceAM
    /// FT8U100AX device
    | Device100AX
    /// Unknown device
    | DeviceUnknown
    /// FT2232 device
    | Device2232
    /// FT232R or FT245R device
    | Device232R
    /// FT2232H device
    | Device2232H
    /// FT4232H device
    | Device4232H
    /// FT232H device
    | Device232H
    /// FT X-Series device
    | DeviceXSeries
    /// FT4222 hi-speed device Mode 0 - 2 interfaces
    | Device4222HMode0
    /// FT4222 hi-speed device Mode 1 or 2 - 4 interfaces
    | Device4222HMode1Or2
    /// FT4222 hi-speed device Mode 3 - 1 interface
    | Device4222HMode3
    /// OTP programmer board for the FT4222.
    | DeviceFT4222Prog
    /// OTP programmer board for the FT900.
    | DeviceFT900
    /// OTP programmer board for the FT930.
    | DeviceFT930
    /// Flash programmer board for the UMFTPD3A.
    | DeviceUMFTPD3A
    /// FT2233HP hi-speed device.
    | Device2233HP
    /// FT4233HP hi-speed device.
    | Device4233HP
    /// FT2233HP hi-speed device.
    | Device2232HP
    /// FT4233HP hi-speed device.
    | Device4232HP
    /// FT233HP hi-speed device.
    | Device233HP
    /// FT232HP hi-speed device.
    | Device232HP
    /// FT2233HA hi-speed device.
    | Device2232HA
    /// FT4233HA hi-speed device.
    | Device4232HA

type DevicePortState =
    | PortOpened
    | PortClosed
    
type DeviceSpeed =
    | HighSpeed
    | FullSpeed

type ChannelInfo =
    { PortState: DevicePortState
      Speed: DeviceSpeed
      Device: DeviceType
      Id: uint32
      LocId: uint32
      SerialNumber: string
      Description: string
      Handle: IntPtr }

type Channel =
    { Handle: nativeint }

