module InternalGpioTests

open Xunit
open FsUnit.Xunit
open Ftdi.LibMpsse.Gpio

[<Fact>]
let ``toGpioDirectionValue returns expected result`` () =
    let gpioPins =
        { Cbus0 = Input
          Cbus1 = OutputLow
          Cbus2 = OutputHigh
          Cbus3 = Input
          Cbus4 = OutputLow
          Cbus5 = OutputHigh
          Cbus6 = Input
          Cbus7 = OutputLow }
        
    let dir, value = toGpioDirectionValue gpioPins
    
    dir |> should equal 0b1011_0110uy
    
    value |> should equal 0b0010_0100uy
    
[<Fact>]
let ``toGpioInputReading returns expected result`` () =
    let gpioPins = 0b0010_0100uy
    
    let reading = toGpioInputReading gpioPins
    
    reading
    |> should equal { Cbus0Input = Low
                      Cbus1Input = Low
                      Cbus2Input = High
                      Cbus3Input = Low
                      Cbus4Input = Low
                      Cbus5Input = High
                      Cbus6Input = Low
                      Cbus7Input = Low }
