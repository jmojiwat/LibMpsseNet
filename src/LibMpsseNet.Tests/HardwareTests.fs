module Tests.Hardware

open Xunit
open FsUnit.Xunit
open Ftdi.LibMpsse.UsbSerialBridge
open Ftdi.LibMpsse.I2c
open Ftdi.LibMpsse.Gpio
open System.Threading

[<Fact>]
let ``i2cGetNumChannels returns expected result`` () =
    initLibMpsse ()
    
    i2cGetNumChannels ()
    |> function
        | Ok numChannels -> numChannels
        | Error _ -> 0u
    |> should not' (equal 0u)
    
    cleanupLibMpsse ()

[<Fact>]
let ``i2cGetChannelInfo returns expected result`` () =
    use libMpsse = useLibMpsse ()
    
    let info =
        i2cGetChannelInfo 0u
        |> function
            | Ok ci -> ci
            | Error _ -> failwith "failed getting channel info."
            
    info.Description |> should not' (be Empty)            
    
    
[<Fact>]
let ``i2cOpenChannel returns expected result`` () =
    initLibMpsse ()

    let status = i2cOpenChannel 0u

    status |> Result.isOk |> should equal true
    
    status
    |> Result.map i2cCloseChannel
    |> ignore
    
    cleanupLibMpsse ()

[<Fact>]
let ``i2cInitChannel returns expected result`` () =
    initLibMpsse ()
    
    let config =
        { ClockRate = I2cClockRate.FastMode
          LatencyTimer = 255uy
          Enable3PhaseClocking = true
          EnableLoopback = false
          EnableClockStretching = false
          PinStateConfig = Disable }
        
    let channel =
        i2cOpenChannel 0u
        |> Result.bind (i2cInitChannel config)

    channel |> Result.isOk |> should equal true
    
    channel
    |> Result.map i2cCloseChannel
    |> ignore
    
    cleanupLibMpsse ()
    
    
[<Fact>]
let ``ftWriteGpio on off returns expected result`` () =
    initLibMpsse ()
    
    let config =
        { ClockRate = I2cClockRate.FastMode
          LatencyTimer = 255uy
          Enable3PhaseClocking = true
          EnableLoopback = false
          EnableClockStretching = false
          PinStateConfig = Disable }
    
    let device =
        i2cOpenChannel 0u
        |> Result.bind (i2cInitChannel config)
        |> function
            | Ok device -> device
            | Error _ -> invalidArg "index" "No device found here"
    
    ftWriteGpio device { Cbus0 = OutputHigh; Cbus1 = Input; Cbus2 = Input; Cbus3 = Input; Cbus4 = Input; Cbus5 = Input; Cbus6 = Input; Cbus7 = Input } |> ignore
    
    Thread.Sleep(2_000)
        
    ftWriteGpio device { Cbus0 = OutputLow; Cbus1 = Input; Cbus2 = Input; Cbus3 = Input; Cbus4 = Input; Cbus5 = Input; Cbus6 = Input; Cbus7 = Input } |> ignore
    
    Thread.Sleep(2_000)
        
    i2cCloseChannel device |> ignore        
    
    cleanupLibMpsse ()