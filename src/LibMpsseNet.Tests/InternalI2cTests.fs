module InternalI2cTests

open Xunit
open FsUnit.Xunit
open Ftdi.LibMpsse
open Ftdi.LibMpsse.I2c

[<Fact>]
let ``mapToI2cChannelConfigStruct returns expected result`` () =
    let channelConfig =
        { ClockRate = I2cClockRate.FastMode
          LatencyTimer = 255uy
          DisableI2c3PhaseClocking = false
          EnableI2cDriveOnlyZero = true }
        
    let config = I2c.mapToI2cChannelConfigStruct channelConfig
    
    config.Options |> should equal 2u

[<Fact>]
let ``toI2cTransferOptions returns expected result`` () =
    let transferOptions =
        { StartBit = true
          StopBit = false
          BreakOnNak = true
          FastTransferBytes = true
          FastTransferBits = false
          NoAddress = true } 
        
    let options = I2c.toI2cTransferOptions transferOptions
    
    options |> should equal 0b0101_0101u
    