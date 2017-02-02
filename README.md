# Tangible Touch
Recognize physical object identities by decoding touch patterns on capacitive screens.

# Prerequisites
The code is written to work with .NET Framework >= 4.5 and references the Math.NET Spatial and Math.NET Numerics libraries for calculation.

# Quickstart
Given a list of TouchPoint instances from WPF, call this API function:
```
public Touchcode Check(IList<TouchPoint> touchPoints, bool xMirror = true, int maxY = 1080)
``` 

The `xMirror` flag should be set to `true` if y-coordinates of the screen start counting at the top of the screen. Additionally, `maxY` tells the system how many pixels the screen width is.

A more general way to check for Touchcodes that does not rely on WPF can be used like this:

```
public Touchcode Check(IList<Point2D> touchpoints, bool xMirror = true, int maxY = 1080)
```

In both cases, if no Touchcode could be read, `Touchcode.None` will be returned. If a valid Touchcode is returned, the origin and the angle between the Touchcode and the positive y-axis can be retrieved.


# How it works
Touchcodes are encoded using a physical 4x4 sized point grid in which points can be either `on` or `off`, meaning whether some capacity can be measured by the touch screen at that point (e.g. by inserting a grounded metal pin at that point.)

In order to find a Touchcode, three corner pins must always be `on`, so that an `origin` and two virtual axes `vx` and `vy` can be found by the system. The 16 points are numbered with an index 0 - 15, going left to right and top to bottom. 

Four pins must always be the encoded the same way:
  - pin  0: `on` (`vy`)
  - pin  3: `off`
  - pin 12: `on` (`origin`)
  - pin 15: `on` (`vx`)
  
That leaves us with 12 pins that can be used to encode a Touchcode, they are here called the "Touchcode pins". Each Touchcode pin is mapped to a single bit in a 12-bit number, so 4096 Touchcodes can be encoded, numbered 0 - 4095 or `0x000` - `0xFFF`  in hex.

The following example is the minimum configuration necessary to read a Touchcode (`X` meaning `on` and `o` meaning `off`):
  
```
X----o----o----o  
|    |    |    |  
o----o----o----o  
|    |    |    |      Touchcode 0x00 (0)
o----o----o----o      (angle = 0°)
|    |    |    |  
X----o----o----X  
```

None of the 12 Touchcode pins is `on`, so the recognized Touchcode will be `0x00`.

To encode other Touchcodes, the 12 Touchcode pins can be set to `on`, with pin 1 encoding the least significant bit and pin 14 the most significant bit. 

Some examples:

```
X----X----o----o  
|    |    |    |  
o----o----o----o  
|    |    |    |      Touchcode 0x01 (1)
o----o----o----o      (angle = 0°)
|    |    |    |  
X----o----o----X  


X----o----o----o  
|    |    |    |  
o----o----o----o  
|    |    |    |      Touchcode 0x800 (2048)
o----o----o----o      (angle = 0°)
|    |    |    |  
X----o----X----X  


X----X----o----o  
|    |    |    |  
o----o----o----o  
|    |    |    |      Touchcode 0x801 (2049)
o----o----o----o      (angle = 0°)
|    |    |    |  
X----o----X----X  

X----X----X----o  
|    |    |    |  
X----X----X----X  
|    |    |    |      Touchcode 0xFFF (4095)
X----X----X----X      (angle = 0°)
|    |    |    |  
X----X----X----X  
```

It is important that all pins have the same vertical and horizontal distance so that the system can recognize the grid.