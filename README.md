# LibMpsseNet

LibMpsseNet is a thin F# wrapper for FTDI Chip Multi Protocol Synchronous Serial Engine (MPSSE) library 1.0.5.

MPSSE provides a flexible means of interfacing synchronous serial devices to a USB port. The FT-serices device supported includes the FT2232D, FT2232H, FT4232H and FT232H.

## Requirements

LibMpsseNet requires libmpsse.lib C library to be present from the [LibMPSSE library](https://ftdichip.com/wp-content/uploads/2024/01/LibMPSSE_1.0.5.zip).

## API

API closely follows the API in the MPSSE documentation that can be found in the [FTDI Chip Application Notes](https://ftdichip.com/document/application-notes/).

[AN 177 User Guide for LibMPSSE-I2C](https://ftdichip.com/wp-content/uploads/2020/08/AN_177_User_Guide_For_LibMPSSE-I2C.pdf)

[AN 178 User Guide for LibMPSSE-SPI](https://ftdichip.com/wp-content/uploads/2023/08/AN_178_User-Guide-for-LibMPSSE-SPI.pdf)

## Example

```fsharp

use lib = useLibMpsse ()
    
let config =
    { ClockRate = I2cClockRate.FastMode
      LatencyTimer = 255uy
      Enable3PhaseClocking = true
      EnableLoopback = false
      EnableClockStretching = false
      PinStateConfig = Disable }
        
let status =
    i2cOpenChannel 0u
    |> Result.bind (i2cInitChannel config)

// some code
    
status
|> Result.map i2cCloseChannel
|> ignore

```
