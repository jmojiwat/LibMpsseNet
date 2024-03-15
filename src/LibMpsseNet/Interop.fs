namespace Ftdi.LibMpsse

module Interop =

    open System
    open System.Runtime.InteropServices
    
    //[<assembly: InternalsVisibleTo("LibMpsseNet.Tests")>]
    //do ()
    

    [<Literal>]
    let Libmpsse = "libmpsse.dll"

    type FT_STATUS =
        | FT_OK = 0
        | FT_INVALID_HANDLE = 1
        | FT_DEVICE_NOT_FOUND = 2
        | FT_DEVICE_NOT_OPENED = 3
        | FT_IO_ERROR = 4
        | FT_INSUFFICIENT_RESOURCES = 5
        | FT_INVALID_PARAMETER = 6
        | FT_INVALID_BAUD_RATE = 7
        | FT_DEVICE_NOT_OPENED_FOR_ERASE = 8
        | FT_DEVICE_NOT_OPENED_FOR_WRITE = 9
        | FT_FAILED_TO_WRITE_DEVICE = 10
        | FT_EEPROM_READ_FAILED = 11
        | FT_EEPROM_WRITE_FAILED = 12
        | FT_EEPROM_ERASE_FAILED = 13
        | FT_EEPROM_NOT_PRESENT = 14
        | FT_EEPROM_NOT_PROGRAMMED = 15
        | FT_INVALID_ARGS = 16
        | FT_NOT_SUPPORTED = 17
        | FT_OTHER_ERROR = 18
        | FT_DEVICE_LIST_NOT_READY = 19
    
    type FT_DEVICE =
        | FT_DEVICE_BM = 0
        | FT_DEVICE_AM = 1
        | FT_DEVICE_100AX = 2
        | FT_DEVICE_UNKNOWN = 3
        | FT_DEVICE_2232C = 4
        | FT_DEVICE_232R = 5
        | FT_DEVICE_2232H = 6
        | FT_DEVICE_4232H = 7
        | FT_DEVICE_232H = 8
        | FT_DEVICE_X_SERIES = 9
        | FT_DEVICE_4222H_0 = 10
        | FT_DEVICE_4222H_1_2 = 11
        | FT_DEVICE_4222H_3 = 12
        | FT_DEVICE_4222_PROG = 13
        | FT_DEVICE_900 = 14
        | FT_DEVICE_930 = 15
        | FT_DEVICE_UMFTPD3A = 16
        | FT_DEVICE_2233HP = 17
        | FT_DEVICE_4233HP = 18
        | FT_DEVICE_2232HP = 19
        | FT_DEVICE_4232HP = 20
        | FT_DEVICE_233HP = 21
        | FT_DEVICE_232HP = 22
        | FT_DEVICE_2232HA = 23
        | FT_DEVICE_4232HA = 24
        
    [<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)>]
    type FT_DEVICE_LIST_INFO_NODE =
        struct
            /// The flag value is a 4-byte bit map containing miscellaneous data
            /// Bit 0 (least significant bit) of this number indicates if the port is open (1) or closed (0). Bit 1
            /// indicates if the device is enumerated as a high-speed USB device (2) or a full-speed USB device (0). The
            /// remaining bits (2 - 31) are reserved.
            val mutable Flags : uint32
            /// FTxxxx device type
            val mutable Type : FT_DEVICE
            /// The Vendor IDof the device
            val mutable Id : uint32
            /// The physical location identifier of the device
            val mutable LocId : uint32
    
            /// The device serial number
            [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)>]
            val mutable SerialNumber : string
    
            /// The device description
            [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)>]
            val mutable Description : string
    
            /// The device handle. This value is not used externally and is provided for information only.
            /// If the device is not open, this value is 0.
            val mutable Handle : IntPtr
        end
        
    type CHANNEL_CONFIG =
        struct
            val mutable ClockRate: uint32
            val mutable LatencyTimer: uint8
            val mutable Options: uint32
        end
    
    module I2c =

        /// Gets the number of I2C channels that are connected to the host system. The number
        /// of ports available in each of these chips is different.
        ///
        /// FT_STATUS I2C_GetNumChannels (uint32 *numChannels)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS I2C_GetNumChannels(uint32& numChannels)
    
        /// Takes a channel index (valid values are from 0 to the value returned by
        /// I2C_GetNumChannels – 1) and provides information about the channel in the form of a populated
        /// FT_DEVICE_LIST_INFO_NODE structure.
        ///
        /// FT_STATUS I2C_GetChannelInfo (uint32 index,FT_DEVICE_LIST_INFO_NODE *chanInfo)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS I2C_GetChannelInfo(uint32 index, FT_DEVICE_LIST_INFO_NODE& chanInfo)
        
        /// opens the indexed channel and provides a handle to it. Valid values for the index of
        /// channel can be from 0 to the value obtained using I2C_GetNumChannels – 1).
        ///
        /// FT_STATUS I2C_OpenChannel (uint32 index, FT_HANDLE *handle)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS I2C_OpenChannel(uint32 index, nativeint& handle)
        
        /// Initializes the channel and the communication parameters associated with it.
        ///
        /// FT_STATUS I2C_InitChannel (FT_HANDLE handle, ChannelConfig *config)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS I2C_InitChannel(nativeint handle, CHANNEL_CONFIG& config)
        
        /// Closes a channel and frees all resources that were used by it
        ///
        /// FT_STATUS I2C_CloseChannel (FT_HANDLE handle)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS I2C_CloseChannel(nativeint handle)
        
        /// Reads the specified number of bytes from an addressed I2C slave.
        ///
        /// FT_STATUS I2C_DeviceRead(FT_HANDLE handle, uint32 deviceAddress, uint32 sizeToTransfer, uint8 *buffer, uint32 *sizeTransferred, uint32 options).
        [<DllImport(Libmpsse)>]
        extern FT_STATUS I2C_DeviceRead(
            nativeint handle,
            uint32 deviceAddress,
            uint32 sizeToTransfer,
            byte[] buffer,
            uint32& sizeTransferred,
            uint32 options)
        
        /// This function writes the specified number of bytes to an addressed I2C slave.
        /// 
        /// FT_STATUS I2C_DeviceWrite (FT_HANDLE handle, uint32 deviceAddress, uint32 sizeToTransfer, uint8 *buffer, uint32 *sizeTransferred, uint32 options)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS I2C_DeviceWrite(
            nativeint handle,
            uint32 deviceAddress,
            uint32 sizeToTransfer,
            byte[] buffer,
            uint32& sizeTransferred,
            uint32 options)

    
    module Spi =

        /// Gets the number of SPI channels that are connected to the host
        /// system. The number of ports available in each of these chips is different.
        /// 
        /// FT_STATUS SPI_GetNumChannels (uint32 *numChannels)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_GetNumChannels(uint32& numChannels)

        /// Takes a channel index (valid values are from 0 to the value returned by
        /// SPI_GetNumChannels - 1) and provides information about the channel in the form of
        /// a populated FT_DEVICE_LIST_INFO_NODE structure.
        ///
        /// FT_STATUS SPI_GetChannelInfo (uint32 index, FT_DEVICE_LIST_INFO_NODE *chanInfo)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_GetChannelInfo(uint32 index, FT_DEVICE_LIST_INFO_NODE& chanInfo)

        /// Opens the indexed channel and provides a handle to it. Valid values for
        /// the index of channel can be from 0 to the value obtained using SPI_GetNumChannels
        /// - 1).
        /// 
        /// FT_STATUS SPI_OpenChannel (uint32 index, FT_HANDLE *handle)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_OpenChannel(uint32 index, nativeint& handle)

        /// Initializes the channel and the communication parameters associated
        /// with it.
        ///
        /// FT_STATUS SPI_InitChannel (FT_HANDLE handle, ChannelConfig *config)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_InitChannel(nativeint handle, CHANNEL_CONFIG& config)

        /// Closes a channel and frees all resources that were used by it
        /// 
        /// FT_STATUS SPI_CloseChannel (FT_HANDLE handle)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_CloseChannel(nativeint handle)

        /// Reads the specified number of bits or bytes (depending on transferOptions
        /// parameter) from an SPI slave.
        ///
        /// FT_STATUS SPI_Read(FT_HANDLE handle, uint8 *buffer, uint32 sizeToTransfer, uint32 *sizeTransferred, uint32 transferOptions)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_Read(
            nativeint handle,
            byte[] buffer,
            uint32 sizeToTransfer,
            uint32& sizeTransferred,
            uint32 transferOptions)

        /// Writes the specified number of bits or bytes (depending on transferOptions
        /// parameter) to a SPI slave.
        ///
        /// FT_STATUS SPI_ Write(FT_HANDLE handle, uint8 *buffer, uint32 sizeToTransfer, uint32 *sizeTransferred, uint32 transferOptions)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_Write(
            nativeint handle,
            byte[] buffer,
            uint32 sizeToTransfer,
            uint32& sizeTransferred,
            uint32 transferOptions)

        /// Reads from and writes to the SPI slave simultaneously. Meaning that,
        /// one bit is clocked in and one bit is clocked out during every clock cycle.
        ///
        /// FT_STATUS SPI_ReadWrite(FT_HANDLE handle, uint8 *inBuffer, uint8 *outBuffer, uint32 sizeToTransfer, uint32 *sizeTransferred, uint32 transferOptions)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_ReadWrite(
            nativeint handle,
            byte[] inBuffer,
            byte[] outBuffer,
            uint32 sizeToTransfer,
            uint32& sizeTransferred,
            uint32 transferOptions)

        /// Reads the state of the MISO line without clocking the SPI bus.
        /// Some applications need the SPI master to poll the MISO line without clocking the bus
        /// to check if the SPI slave has completed previous operation and is ready for the next
        /// operation. This function is useful for such applications.
        ///
        /// FT_STATUS SPI_ IsBusy(FT_HANDLE handle, bool *state)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_IsBusy(nativeint handle, bool& state)

        /// This function changes the chip select line that is to be used to communicate to the
        /// SPI slave.
        ///
        /// FT_STATUS SPI_ChangeCS(FT_HANDLE handle, uint32 configOptions)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS SPI_ChangeCS(nativeint handle, uint32 configOptions)
        
        
    module Gpio =
        
        /// Writes to the 8 GPIO lines associated with the high byte of the MPSSE channel
        /// 
        /// FT_STATUS FT_WriteGPIO(FT_HANDLE handle, uint8 dir, uint8 value)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS FT_WriteGPIO(nativeint handle, uint8 dir, uint8 value)
        
        /// Reads from the 8 GPIO lines associated with the high byte of the MPSSE channel
        ///
        /// FT_STATUS FT_ReadGPIO(FT_HANDLE handle,uint8 *value)
        [<DllImport(Libmpsse)>]
        extern FT_STATUS FT_ReadGPIO(nativeint handle, uint8& value)
        
    module Infrastructure =
        
        /// Initializes the library
        /// 
        /// void Init_libMPSSE(void)
        [<DllImport(Libmpsse)>]
        extern void Init_libMPSSE()
        
        /// Cleans up resources used by the library
        ///
        /// void Cleanup_libMPSSE(void)
        [<DllImport(Libmpsse)>]
        extern void Cleanup_libMPSSE()

        
    
