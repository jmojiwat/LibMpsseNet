module InternalI2cTests

open Xunit
open FsUnit.Xunit
open Ftdi.LibMpsse.I2c

[<Fact>]
let ``toI2cChannelConfig returns expected result`` () =
    let channelConfig =
        { ClockRate = I2cClockRate.FastMode
          LatencyTimer = 255uy
          Enable3PhaseClocking = false
          EnableLoopback = false
          EnableClockStretching = true
          PinStateConfig = Disable }
        
    // 3PhaseClocking = 0x01
    // ClockStretching = 0x04
    // 0000_0001 ||| 0000_0100 = 0b0000_0101
    
    let config = toI2cChannelConfig channelConfig
    
    config.Options |> should equal 0b0000_0101u

[<Fact>]
let ``toI2cTransferOptions returns expected result`` () =
    let transferOptions =
        { StartBit = true
          StopBit = false
          TransferSpeed = NormalTransfer (true, false) 
          NoAddress = true } 

    // StartBit = 0x01
    // NormalTransfer = 0x00, BreakOnNack = 0x04
    // NoAddress = 0x40
    // 0000_0001 ||| 0000_0100 ||| 0100_000 = 0b0100_0101
            
    let options = toI2cTransferOptions transferOptions
    
    options |> should equal 0b0100_0101u
    