module InternalSpiTests

open Xunit
open FsUnit.Xunit
open Ftdi.LibMpsse.Spi
open UnitsNet

[<Fact>]
let ``toSpiChannelConfigStruct returns expected result`` () =
    let channelConfig =
        { ClockRate = Frequency.FromMegahertz(30)
          LatencyTimer = 255uy
          SpiMode = SpiMode2
          ChipSelect = Dbus6Line
          ChipSelectActive = ActiveLow
          Pins = 0u }
        
    let config = toSpiChannelConfig channelConfig
    
    config.ClockRate |> should equal 30_000_000u
    config.LatencyTimer |> should equal 255uy
    config.configOptions |> should equal 0b0010_1110u
    
[<Fact>]
let ``toSpiTransferOptions returns expected result`` () =
    let transferOptions =
        { Size = SizeInBytes
          EnableChipSelect =  true }
        
    let options = toSpiTransferOptions transferOptions
    
    options |> should equal 0b0010u
    
