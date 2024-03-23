namespace Ftdi.LibMpsse

open System
open System.Runtime.InteropServices

module Interop =

    type FT_STATUS =
        | FT_OK = 0u
        | FT_INVALID_HANDLE = 1u
        | FT_DEVICE_NOT_FOUND = 2u
        | FT_DEVICE_NOT_OPENED = 3u
        | FT_IO_ERROR = 4u
        | FT_INSUFFICIENT_RESOURCES = 5u
        | FT_INVALID_PARAMETER = 6u
        | FT_INVALID_BAUD_RATE = 7u
        | FT_DEVICE_NOT_OPENED_FOR_ERASE = 8u
        | FT_DEVICE_NOT_OPENED_FOR_WRITE = 9u
        | FT_FAILED_TO_WRITE_DEVICE = 10u
        | FT_EEPROM_READ_FAILED = 11u
        | FT_EEPROM_WRITE_FAILED = 12u
        | FT_EEPROM_ERASE_FAILED = 13u
        | FT_EEPROM_NOT_PRESENT = 14u
        | FT_EEPROM_NOT_PROGRAMMED = 15u
        | FT_INVALID_ARGS = 16u
        | FT_NOT_SUPPORTED = 17u
        | FT_OTHER_ERROR = 18u
        | FT_DEVICE_LIST_NOT_READY = 19u
    
    /// <summary><para>Device Types.</para>
    /// <para>Known supported FTDI device types supported by this library.</para></summary>
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

    // from libmpsse_i2c.h
    
    // Options to I2C_DeviceWrite & I2C_DeviceRead
    // Generate start condition before transmitting.
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_START_BIT = 0x0000_0001
    
    // Generate stop condition before transmitting.
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_STOP_BIT = 0x0000_0002
    
    // Continue transmitting data in bulk without caring about Ack or nAck from device if this bit
    // is not set. If this bit is set then stop transferring the data in the buffer when the device
    // nACKs.
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_BREAK_ON_NACK = 0x0000_0004
    
    // libMPSSE-I2C generates an ACKs for every byte read. Some I2C slaves require the I2C
    // master to generate a nACK for the last data byte read. Setting this bit enables working with
    // such I2C slaves.
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_NACK_LAST_BYTE = 0x0000_0008
    
    // Fast transfers prepare a buffer containing commands to generate START/STOP/ADDRESS
    // conditions and commands to read/write data. This buffer is sent to the MPSSE in one shot,
    // hence delays between different phases of the I2C transfer are eliminated. Fast transfers
    // can have data length in terms of bits or bytes. The user application should call
    // I2C_DeviceWrite or I2C_DeviceRead with either
    // I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BYTES or
    // I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BITS bit set to perform a fast transfer.
    // I2C_TRANSFER_OPTIONS_START_BIT and I2C_TRANSFER_OPTIONS_STOP_BIT have
    // their usual meanings when used in fast transfers, however
    // I2C_TRANSFER_OPTIONS_BREAK_ON_NACK and
    // I2C_TRANSFER_OPTIONS_NACK_LAST_BYTE are not applicable in fast transfers. */
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_FAST_TRANSFER = 0x0000_0030     // not visible to user
    
    // When the user calls I2C_DeviceWrite or I2C_DeviceRead with this bit set then libMPSSE
    // packs commands to transfer sizeToTransfer number of bytes, and to read/write
    // sizeToTransfer number of ack bits. If data is written then the read ack bits are ignored, if
    // data is being read then an acknowledgement bit(SDA=LOW) is given to the I2C slave
    // after each byte read.
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BYTES = 0x0000_0010
    
    // When the user calls I2C_DeviceWrite or I2C_DeviceRead with this bit set then libMPSSE
    // packs commands to transfer sizeToTransfer number of bits. There is no ACK phase when
    // this bit is set.
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BITS = 0x0000_0020
    
    // The address parameter is ignored in transfers if this bit is set. This would mean that
    // the address is either a part of the data or this is a special I2C frame that doesn't require
    // an address. However if this bit is not set then 7bit address and 1bit direction will be
    // written to the I2C bus each time I2C_DeviceWrite or I2C_DeviceRead is called and a
    // 1bit acknowledgement will be read after that.
    [<Literal>]
    let I2C_TRANSFER_OPTIONS_NO_ADDRESS = 0x0000_0040
    
    [<Literal>]
    let I2C_CMD_GETDEVICEID_RD = 0xF9
    [<Literal>]
    let I2C_CMD_GETDEVICEID_WR = 0xF8
    
    [<Literal>]
    let I2C_GIVE_ACK = 1
    [<Literal>]
    let I2C_GIVE_NACK = 0
    
    // 3-phase clocking is enabled by default. Setting this bit in ConfigOptions will disable it
    [<Literal>]
    let I2C_DISABLE_3PHASE_CLOCKING = 0x0001uy
    
    // option to enable pinstate configuration
    [<Literal>]
    let I2C_ENABLE_PIN_STATE_CONFIG = 0x0010uy
    
    type I2C_CLOCKRATE =
        | I2C_CLOCK_STANDARD_MODE = 100_000     // 100kb/sec
        | I2C_CLOCK_FAST_MODE = 400_000         // 400kb/sec
        | I2C_CLOCK_FAST_MODE_PLUS = 1000_000   // 1000kb/sec
        | I2C_CLOCK_HIGH_SPEED_MODE = 3400_000  // 3.4Mb/sec
        
    type I2C_CHANNEL_CONFIG =
        struct
            /// <summary><para>There were 2 functions I2C_TurnOn/OffDivideByFive</para>
            /// <para>ClockinghiSpeedDevice (FTC_HANDLE fthandle) in the old DLL. This function turns on the
            /// divide by five for the MPSSE clock to allow the hi-speed devices FT2232H and FT4232H to
            /// clock at the same rate as the FT2232D device. This allows for backward compatibility.</para></summary>
            /// <remarks>NOTE: This feature is probably a per chip feature and not per device.</remarks>
//            val mutable ClockRate: I2C_CLOCKRATE
            val mutable ClockRate: uint32
            
            /// <summary><para>Required value, in milliseconds, of latency timer.</para>
            /// <para>Valid range is 2 - 255</para>
            /// <para>In the FT8U232AM and FT8U245AM devices, the receive buffer timeout that is used to flush
            /// remaining data from the receive buffer was fixed at 16 ms. In all other FTDI devices, this
            /// timeout is programmable and can be set at 1 ms intervals between 2ms and 255 ms.  This
            /// allows the device to be better optimized for protocols requiring faster response times from
            /// short data packets.</para></summary>
            /// <remarks>NOTE: This feature is probably a per chip feature and not per device</remarks>
            val mutable LatencyTimer: uint8

            /// <summary><para>This member provides a way to enable/disable features
            /// specific to the protocol that are implemented in the chip.</para>
            /// <para>BIT0: 3PhaseDataClocking - Setting this bit will turn on 3 phase data clocking for a
            /// FT2232H dual hi-speed device or FT4232H quad hi-speed device. Three phase
            /// data clocking, ensures the data is valid on both edges of a clock.</para>
            /// <para>BIT1: Loopback.</para>
            /// <para>BIT2: Clock stretching.</para>
            /// <para>BIT3: Enable PinState config.</para>
            /// <para>BIT4 - BIT31: Reserved.</para></summary>
            val mutable Options: uint32
            
            /// <summary><para>BIT7 - BIT0: Initial direction of the pins.</para>
            /// <para>BIT15 - BIT8: Initial values of the pins.</para>
            /// <para>BIT23 - BIT16: Final direction of the pins.</para>
            /// <para>BIT31 - BIT24: Final values of the pins.</para></summary>
            val mutable Pin: uint32
            
            /// <summary><para>BIT7 - BIT0: Current direction of the pins.</para>
            /// <para>BIT15 -BIT8: Current values of the pins.</para></summary>
            val mutable currentPinState: uint16
        end

    /// <summary>This structure associates the channel configuration information to a handle stores them in the
    /// form of a linked list.</summary>
    type I2cChannelContext =
        struct
            val mutable handle: nativeint
            val mutable config: I2C_CHANNEL_CONFIG
            val mutable next: nativeint
        end
    
    [<Literal>]
    let Libmpsse = "libmpsse.dll"

   
    /// <summary><para>Initializes libMPSSE.</para>
    /// <para>This function is called once when the library is loaded. It initializes all the modules in the
    /// library. This function initializes all the variables and data structures that are required to be
    /// initialized once only during loading.</para></summary>
    ///
    /// <remarks>May individually call Ftdi_I2C_Module_Init, Ftdi_SPI_Module_Init, Ftdi_Mid_Module_Init,
    /// Ftdi_Common_Module_Init, etc if required. This function should be called by the OS specific
    /// function(eg: DllMain for windows) that is called by the OS automatically during startup.</remarks>
    ///
    /// void Init_libMPSSE(void)
    [<DllImport(Libmpsse)>]
    extern void Init_libMPSSE()
    
    /// <summary><para>Cleans up resources used by the library.</para>
    /// <para>This function frees all the resources that were allocated during initialization. It should be called
    /// by the OS to ensure that the module exits gracefully.</para></summary>
    ///
    /// void Cleanup_libMPSSE(void)
    [<DllImport(Libmpsse)>]
    extern void Cleanup_libMPSSE()
        

    /// <summary><para>Gets the number of I2C channels that are connected to the host system. The number.</para>
    /// <para>This function gets the number of I2C channels that are connected to the host system
    /// The number of ports available in each of these chips are different.</para></summary>
    /// <remarks><para>This function doesn't return the number of FTDI chips connected to the host system</para>
    /// <para>note FT2232D has 1 MPSSE port</para>
    /// <para>note FT2232H has 2 MPSSE ports</para>
    /// <para>note FT4232H has 4 ports but only 2 of them have MPSSEs so call to this function will return 2 if a FT4232 is connected to it.</para></remarks>
    /// <param name="numChannels">Pointer to variable in which the no of channels will be returned.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    /// 
    /// FT_STATUS I2C_GetNumChannels (uint32 *numChannels)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_GetNumChannels(uint32& numChannels)

    /// <summary><para>Provides information about channel.</para>
    /// <para>This function takes a channel index (valid values are from 1 to the value returned by
    /// I2C_GetNumChannels) and provides information about the channel in the form of a populated
    /// ChannelInfo structure.</para></summary>
    /// <remarks>The channel ID can be determined by the user from the last digit of the location ID.</remarks>
    /// <param name="index">Index of the channel.</param>
    /// <param name="chanInfo">Pointer to FT_DEVICE_LIST_INFO_NODE structure(see D2XX Programmer's Guide).</param>
    /// <returns>status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS I2C_GetChannelInfo (uint32 index,FT_DEVICE_LIST_INFO_NODE *chanInfo)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_GetChannelInfo(uint32 index, FT_DEVICE_LIST_INFO_NODE& chanInfo)
    
    /// <summary><para>Opens a channel and returns a handle to it.</para>
    /// <para>This function opens the indexed channel and returns a handle to it.</para></summary>
    /// <param name="index">Index of the channel.</param>
    /// <param name="handle">Pointer to the handle of the opened channel.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS I2C_OpenChannel (uint32 index, FT_HANDLE *handle)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_OpenChannel(uint32 index, nativeint& handle)
    
    /// <summary><para>Initializes a channel.</para>
    /// <para>This function initializes the channel and the communication parameters associated with it.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="config">Pointer to the ChannelConfig structure (memory to be allocated by caller).</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS I2C_InitChannel (FT_HANDLE handle, ChannelConfig *config)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_InitChannel(nativeint handle, I2C_CHANNEL_CONFIG& config)
    
    /// <summary><para>Closes a channel.</para>
    /// <para>Closes a channel and frees all resources that were used by it.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS I2C_CloseChannel (FT_HANDLE handle)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_CloseChannel(nativeint handle)
    
    /// <summary><para>Reads data from I2C slave.</para>
    /// <para>This function reads the specified number of bytes from an addressed I2C slave.</para></summary>
    /// <remarks><para>Definitions of macros I2C_TRANSFER_OPTIONS_START_BIT,</para>
    /// <para>I2C_TRANSFER_OPTIONS_STOP_BIT, I2C_TRANSFER_OPTIONS_BREAK_ON_NACK,</para>
    /// <para>I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BYTES,</para>
    /// <para>I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BITS &</para>
    /// <para>I2C_TRANSFER_OPTIONS_NO_ADDRESS</para></remarks>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="deviceAddress">Address of the I2C slave.</param>
    /// <param name="sizeToTransfer">Number of bytes to be read.</param>
    /// <param name="buffer">Pointer to the buffer where data is to be read.</param>
    /// <param name="sizeTransferred">Pointer to variable containing the number of bytes read.</param>
    /// <param name="options">This parameter specifies data transfer options. Namely, if a start/stop bits
    /// are required, if the transfer should continue or stop if device nAcks, etc.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS I2C_DeviceRead(FT_HANDLE handle, uint32 deviceAddress, uint32 sizeToTransfer, uint8 *buffer, uint32 *sizeTransferred, uint32 options).
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_DeviceRead(
        nativeint handle,
        byte deviceAddress,
        uint32 sizeToTransfer,
        byte[] buffer,
        uint32& sizeTransferred,
        uint32 options)
    
    /// <summary><para>Writes data from I2C slave.</para>
    /// <para>This function writes the specified number of bytes from an addressed I2C slave.</para></summary>
    /// <remarks><para>Definitions of macros I2C_TRANSFER_OPTIONS_START_BIT,</para>
    /// <para>I2C_TRANSFER_OPTIONS_STOP_BIT, I2C_TRANSFER_OPTIONS_BREAK_ON_NACK,</para>
    /// <para>I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BYTES,</para>
    /// <para>I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BITS &</para>
    /// <para>I2C_TRANSFER_OPTIONS_NO_ADDRESS</para></remarks>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="deviceAddress">Address of the I2C slave.</param>
    /// <param name="sizeToTransfer">Number of bytes to be written.</param>
    /// <param name="buffer">Pointer to the buffer where data is to be written.</param>
    /// <param name="sizeTransferred">Pointer to variable containing the number of bytes written.</param>
    /// <param name="options">This parameter specifies data transfer options. Namely if a start/stop bits
    /// are required, if the transfer should continue or stop if device nAcks, etc</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    /// 
    /// FT_STATUS I2C_DeviceWrite (FT_HANDLE handle, uint32 deviceAddress, uint32 sizeToTransfer, uint8 *buffer, uint32 *sizeTransferred, uint32 options)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_DeviceWrite(
        nativeint handle,
        byte deviceAddress,
        uint32 sizeToTransfer,
        byte[] buffer,
        uint32& sizeTransferred,
        uint32 options)

    /// <summary><para>Get the I2C device ID.</para>
    /// <para>This function retrieves the I2C device ID. It may not be enabled in the library
    /// depending on build configuration. If it is not enabled then it will return
    /// FT_NOT_SUPPORTED.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="deviceAddress">Address of the I2C slave.</param>
    /// <param name="deviceID">Address of memory where the 3byte I2C device ID will be stored.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    [<DllImport(Libmpsse)>]
    extern FT_STATUS I2C_GetDeviceID(nativeint handle, byte deviceAddress, byte[] deviceID)
        
    /// <summary><para>Writes to the 8 GPIO lines.</para>
    /// <para>Writes to the 8 GPIO lines associated with the high byte of the MPSSE channel</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="dir">The direction of the 8 lines. 0 for in and 1 for out.</param>
    /// <param name="value">Output state of the 8 GPIO lines.</param>
    /// <returns>Status.</returns>
    [<DllImport(Libmpsse)>]
    extern FT_STATUS FT_WriteGPIO(
        nativeint handle, 
        byte dir,
        byte value)
    
    /// <summary><para>Reads from the 8 GPIO lines.</para>
    /// <para>This function reads the GPIO lines associated with the high byte of the MPSSE channel.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="value">Input statae of the 8 GPIO lines (1 = high).</param>
    [<DllImport(Libmpsse)>]
    extern FT_STATUS FT_ReadGPIO(nativeint handle, byte& value)

    /// <summary><para>Version Number Function.</para>
    /// <para>Returns libMPSSE and libFTD2XX version number.</para></summary>
    /// <param name="libmpsse">MPSSE version number is returned.</param>
    /// <param name="libftd2xx">D2XX version number is returned.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FTDIMPSSE_API FT_STATUS Ver_libMPSSE(LPDWORD libmpsse, LPDWORD libftd2xx); 
    [<DllImport(Libmpsse)>]
    extern FT_STATUS Ver_libMPSSE(uint32 libmpsse, uint32[] libftd2xx)
    
    // from libmpsse_spi.h
    
    // Bit definition of the transferOptions parameter in SPI_Read, SPI_Write & SPI_Transfer
    
    // transferOptions-Bit0: If this bit is 0 then it means that the transfer size provided is in bytes
    [<Literal>]
    let SPI_TRANSFER_OPTIONS_SIZE_IN_BYTES = 0x0000_0000
    
    // transferOptions-Bit0: If this bit is 1 then it means that the transfer size provided is in bytes
    [<Literal>]
    let SPI_TRANSFER_OPTIONS_SIZE_IN_BITS = 0x0000_0001
    // transferOptions-Bit1: if BIT1 is 1 then CHIP_SELECT line will be enabled at start of transfer
    [<Literal>]
    let SPI_TRANSFER_OPTIONS_CHIPSELECT_ENABLE = 0x0000_0002
    // transferOptions-Bit2: if BIT2 is 1 then CHIP_SELECT line will be disabled at end of transfer
    [<Literal>]
    let SPI_TRANSFER_OPTIONS_CHIPSELECT_DISABLE = 0x0000_0004
    // transferOptions-Bit3: if BIT3 is 1 then LSB will be processed first
    [<Literal>]
    let SPI_TRANSFER_OPTIONS_LSB_FIRST = 0x0000_0008
    
    
    // Bit definition of the Options member of configOptions structure
    [<Literal>]
    let SPI_CONFIG_OPTION_MODE_MASK = 0x0000_0003
    [<Literal>]
    let SPI_CONFIG_OPTION_MODE0 = 0x0000_0000
    [<Literal>]
    let SPI_CONFIG_OPTION_MODE1 = 0x0000_0001
    [<Literal>]
    let SPI_CONFIG_OPTION_MODE2 = 0x0000_0002
    [<Literal>]
    let SPI_CONFIG_OPTION_MODE3 = 0x0000_0003
    
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_MASK = 0x0000_001C     // 111 00
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_DBUS3 = 0x0000_0000    // 000 00
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_DBUS4 = 0x0000_0004    // 001 00
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_DBUS5 = 0x0000_0008    // 010 00
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_DBUS6 = 0x0000_000C    // 011 00
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_DBUS7 = 0x0000_0010    // 100 00
    
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_ACTIVEHIGH = 0x0000_0000
    [<Literal>]
    let SPI_CONFIG_OPTION_CS_ACTIVELOW = 0x0000_0020
        
    /// <summary>This Structure contains configuration information of the SPI channel. It is populated by the user
    /// application during initialization of the channel and then it is saved in a linked-list and used
    /// internally by other SPI functions during operation. The structure is removed from the list when
    /// the user application calls SPI_CloseChannel.</summary>
    type SPI_CHANNEL_CONFIG =
        struct
            /// SPI clock rate, value should be <= 30000000 
            val mutable ClockRate: uint32
            /// value in milliseconds, maximum value should be <= 255
            val mutable LatencyTimer: uint8
            /// <summary><para>This member provides a way to enable/disable features
            /// specific to the protocol that are implemented in the chip.</para>
            /// <para>BIT1-0=CPOL-CPHA:</para>
            /// <para>00 - MODE0 - data captured on rising edge, propagated on falling.</para>
            /// <para>01 - MODE1 - data captured on falling edge, propagated on rising.</para>
            /// <para>10 - MODE2 - data captured on falling edge, propagated on rising.</para>
            /// <para>11 - MODE3 - data captured on rising edge, propagated on falling.</para>
            /// <para>BIT4-BIT2:</para>
            /// <para>000 - A/B/C/D_DBUS3=ChipSelect.</para>
            /// <para>001 - A/B/C/D_DBUS4=ChipSelect.</para>
            /// <para>010 - A/B/C/D_DBUS5=ChipSelect.</para>
            /// <para>011 - A/B/C/D_DBUS6=ChipSelect.</para>
            /// <para>100 - A/B/C/D_DBUS7=ChipSelect.</para>
            /// <para>BIT5: ChipSelect is active high if this bit is 0.</para>
            /// <para>BIT6 - BIT31: Reserved.</para></summary>
            val mutable configOptions: uint32
            /// <summary><para>BIT7 - BIT0: Initial direction of the pins.</para>
            /// <para>BIT15 - BIT8: Initial values of the pins.</para>
            /// <para>BIT23 - BIT16: Final direction of the pins.</para>
            /// <para>BIT31 - BIT24: Final values of the pins.</para></summary>
            val mutable Pin: uint32
            /// <summary><para>BIT7 - BIT0: Current direction of the pins.</para>
            /// <para>BIT15 - BIT8: Current values of the pins.</para></summary>
            val mutable currentPinState: uint16
        end
        
    /// <summary>This structure associates the channel configuration information to a handle stores them in the
    /// form of a linked list.</summary>
    type SpiChannelContext =
        struct
            val mutable handle: nativeint
            val mutable config: SPI_CHANNEL_CONFIG
            val mutable next: nativeint
        end
    
    /// <summary><para>Gets the number of SPI channels connected to the host.</para>
    /// <para>This function gets the number of SPI channels that are connected to the host system.</para></summary>
    /// <remarks><para>This function doesn't return the number of FTDI chips connected to the host system.</para>
    /// <para>FT2232D has 1 MPSSE port.</para>
    /// <para>FT2232H has 2 MPSSE ports.</para>
    /// <para>FT4232H has 4 ports but only 2 of them have MPSSEs so call to this function will return 2 if a FT4232 is connected to it.</para></remarks>
    /// <param name="numChannels">Pointer to variable in which the no of channels will be returned.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS SPI_GetNumChannels (uint32 *numChannels)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_GetNumChannels(uint32& numChannels)

    /// <summary><para>Provides information about channel.</para>
    /// <para>This function takes a channel index (valid values are from 0 to the value returned by
    /// SPI_GetChannelInfo -1) and provides information about the channel in the form of a
    /// populated ChannelInfo structure.</para></summary>
    /// <remarks>The channel ID can be determined by the user from the last digit of the location ID.</remarks>
    /// <param name="index">Index of the channel.</param>
    /// <param name="chanInfo">Pointer to FT_DEVICE_LIST_INFO_NODE structure(see D2XX Programmer's Guide).</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS SPI_GetChannelInfo (uint32 index, FT_DEVICE_LIST_INFO_NODE *chanInfo)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_GetChannelInfo(uint32 index, FT_DEVICE_LIST_INFO_NODE& chanInfo)

    /// <summary><para>Opens a channel and returns a handle to it</para>
    /// <para>This function opens the indexed channel and returns a handle to it.</para></summary>
    /// <remarks>Trying to open an already open channel will return an error code.</remarks>
    /// <param name="index">Index of the channel.</param>
    /// <param name="handle">Pointer to the handle of the opened channel.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    /// 
    /// FT_STATUS SPI_OpenChannel (uint32 index, FT_HANDLE *handle)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_OpenChannel(uint32 index, nativeint& handle)

    /// <summary><para>Initializes a channel.</para>
    /// <para>This function initializes the channel and the communication parameters associated with it.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="config">Pointer to ChannelConfig structure(memory to be allocated by caller).</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS SPI_InitChannel (FT_HANDLE handle, ChannelConfig *config)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_InitChannel(nativeint handle, SPI_CHANNEL_CONFIG& config)

    /// <summary><para>Closes a channel.</para>
    /// <para>Closes a channel and frees all resources that were used by it.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    /// 
    /// FT_STATUS SPI_CloseChannel (FT_HANDLE handle)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_CloseChannel(nativeint handle)

    /// <summary><para>Reads data from a SPI slave device.</para>
    /// <para>This function reads the specified number of bits or bytes from the SPI device.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="buffer">Pointer to buffer to where data will be read to.</param>
    /// <param name="sizeToTransfer">Size of data to be transfered.</param>
    /// <param name="sizeTransfered">Pointer to variable containing the size of data that got transferred.</param>
    /// <param name="transferOptions"><para>This parameter specifies data transfer options.</para>
    /// <para>if BIT0 is 0 then size is in bytes, otherwise in bits.</para>
    /// <para>if BIT1 is 1 then CHIP_SELECT line will be enables at start of transfer.</para>
    /// <para>if BIT2 is 1 then CHIP_SELECT line will be disabled at end of transfer.</para></param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS SPI_Read(FT_HANDLE handle, uint8 *buffer, uint32 sizeToTransfer, uint32 *sizeTransferred, uint32 transferOptions)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_Read(
        nativeint handle,
        byte[] buffer,
        uint32 sizeToTransfer,
        uint32& sizeTransferred,
        uint32 transferOptions)

    /// <summary><para>Writes data to a SPI slave device.</para>
    /// <para>This function writes the specified number of bits or bytes to the SPI device.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="buffer">Pointer to buffer from containing the data.</param>
    /// <param name="sizeToTransfer">Size of data to be transfered.</param>
    /// <param name="sizeTransfered">Pointer to variable containing the size of data that got transferred.</param>
    /// <param name="transferOptions"><para>This parameter specifies data transfer options.</para>
    /// <para>if BIT0 is 0 then size is in bytes, otherwise in bits.</para>
    /// <para>if BIT1 is 1 then CHIP_SELECT line will be enables at start of transfer.</para>
    /// <para>if BIT2 is 1 then CHIP_SELECT line will be disabled at end of transfer.</para></param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FT_STATUS SPI_ Write(FT_HANDLE handle, uint8 *buffer, uint32 sizeToTransfer, uint32 *sizeTransferred, uint32 transferOptions)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_Write(
        nativeint handle,
        byte[] buffer,
        uint32 sizeToTransfer,
        uint32& sizeTransferred,
        uint32 transferOptions)

    /// <summary><para>Reads and writes data from/to a SPI slave device.</para>
    /// <para>This function transfers data in both directions between a SPI master and a slave. One bit is clocked out and one bit is clocked in during every clock.</para></summary>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="inBuffer">Pointer to buffer to which data read will be stored.</param>
    /// <param name="outBuffer">Pointer to buffer that contains data to be transferred to the slave.</param>
    /// <param name="sizeToTransfer">Size of data to be transferred.</param>
    /// <param name="sizeTransfered">Pointer to variable containing the size of data that got transferred.</param>
    /// <param name="transferOptions"><para>This parameter specifies data transfer options.</para>
    /// <para>if BIT0 is 0 then size is in bytes, otherwise in bits.</para>
    /// <para>if BIT1 is 1 then CHIP_SELECT line will be enables at start of transfer.</para>
    /// <para>if BIT2 is 1 then CHIP_SELECT line will be disabled at end of transfer.</para></param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
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

    /// <summary><para>Read the state of SPI MISO line.</para>
    /// <para>Reads the logic state of the SPI MISO line without clocking the bus.</para></summary>
    /// <remarks>This function may be used for applications that needs to poll the state of the MISO line to check if the device is busy</remarks>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="state">State of the line.</param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    ///
    /// FTDIMPSSE_API FT_STATUS SPI_IsBusy(FT_HANDLE handle, BOOL *state);
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_IsBusy(nativeint handle, bool& state)

    /// <summary><para>Changes the chip select line.</para>
    /// <para>This function changes the chip select line that is to be used to communicate to the SPI slave.</para></summary>
    /// <remarks>This function should only be called after SPI_Init has been called.</remarks>
    /// <param name="handle">Handle of the channel.</param>
    /// <param name="configOptions"><para>Provides a way to select the chip select line & SPI mode.</para>
    ///	<para>BIT1-0=CPOL-CPHA:</para>
    /// <para>00 - MODE0 - data captured on rising edge, propagated on falling.</para>
    ///	<para>01 - MODE1 - data captured on falling edge, propagated on rising.</para>
    ///	<para>10 - MODE2 - data captured on falling edge, propagated on rising.</para>
    ///	<para>11 - MODE3 - data captured on rising edge, propagated on falling.</para>
    ///	<para>BIT4-BIT2:</para>
    /// <para>000 - A/B/C/D_DBUS3=ChipSelect.</para>
    ///	<para>001 - A/B/C/D_DBUS4=ChipSelect.</para>
    /// <para>010 - A/B/C/D_DBUS5=ChipSelect.</para>
    ///	<para>011 - A/B/C/D_DBUS6=ChipSelect.</para>
    ///	<para>100 - A/B/C/D_DBUS7=ChipSelect.</para>
    ///	<para>BIT5:</para>
    /// <para>ChipSelect is active high if this bit is 0.</para>
    ///	<para>BIT6 -BIT31: Reserved.</para></param>
    /// <returns>Status code of type FT_STATUS(see D2XX Programmer's Guide).</returns>
    /// 
    /// FT_STATUS SPI_ChangeCS(FT_HANDLE handle, uint32 configOptions)
    [<DllImport(Libmpsse)>]
    extern FT_STATUS SPI_ChangeCS(nativeint handle, uint32 configOptions)
