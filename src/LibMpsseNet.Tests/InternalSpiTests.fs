module InternalSpiTests

open Xunit
open FsUnit.Xunit
open Ftdi.LibMpsse.Spi
open UnitsNet

[<Fact>]
let ``mapToSpiChannelConfigStruct returns expected result`` () =
    let channelConfig =
        { ClockRate = Frequency.FromMegahertz(30)
          LatencyTimer = 255uy
          SpiMode = SpiMode2
          ChipSelectLine = Dbus6Line
          ChipSelectActive = ActiveLow }
        
    let config = mapToSpiChannelConfigStruct channelConfig
    
    config.ClockRate |> should equal 30_000_000u
    config.configOptions |> should equal 0b0010_1110u
    
[<Fact>]
let ``toSpiTransferOptions returns expected result`` () =
    let transferOptions =
        { SizeInBits = false
          ChipSelectEnable = true
          ChipSelectDisable = true } 
        
    let options = toSpiTransferOptions transferOptions
    
    options |> should equal 0b0110u
    
