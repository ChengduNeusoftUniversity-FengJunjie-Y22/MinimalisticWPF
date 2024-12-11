﻿# MinimalisticWPF

<h3 style="color:Violet">What can this project do for you ?</h3>
<p style="color:White">（1）Backend, simple, flexible animation implementation</p>
<p style="color:White">（2）Easy design pattern implementation</p>
<p style="color:White">（3）Other small components that speed up WPF development</p>

[github √](https://github.com/Axvser/MinimalisticWPF) 

[nuget √](https://www.nuget.org/packages/MinimalisticWPF/)

---

## Important Notice

2024 - 12 - 9 : 

Versions below 2.0.0 will be deprecated at the end of this month.

---

## Document 

Feature Directory
- Core
  - [Animation](#Animation)
  - [Mvvm](#Mvvm)
  - [Aspect-Oriented Programming](#AOP)
  - [Object Pool](#ObjectPool)
  - [Dynamic Theme](#Theme)
- Other
  - [Xml / Json Operation](#Xml&Json)
  - [Folder Creation](#folder)
  - [string Convertor](#stringConvertor)
  - [string Matcher](#stringMatcher)
  - [RGB Convertor](#RGB)

Details
- [TransitionParams](#TransitionParams)
- [Unsafe Transition](#UnsafeTransition)
- [Roslyn](#Generator)
---

## Animation

<h4 style="color:white">Take the following two controls as examples to demonstrate some animation operations</h4>

```xml
    <Grid>
        <TextBlock x:Name="box" Foreground="White" FontSize="30" TextAlignment="Center" VerticalAlignment="Top" Margin="0,40,0,0"/>
        <Grid x:Name="grid" Width="100" Height="200" Background="Gray"/>
    </Grid>
```

A description of the animation parameters is in the Details directory [TransitionParams](#TransitionParams)

<h4 style="color:White">[ 1 - 1 ] Warm Start</h4>

<p style="font-size:14px;color:wheat">Transition（ ）</p>
<p style="font-size:14px;color:wheat">SetProperty（ Property , Value ）</p>
<p style="font-size:14px;color:wheat">SetParams（ Action&lt;TransitionParams> ）</p>
<p style="font-size:14px;color:wheat">Start（ ）</p>

```csharp 
var animaiton = grid.Transition()
                 .SetProperty(x => x.Width, 200)
                 .SetProperty(x => x.Height, 100)
                 .SetParams((p) =>
                 {
                     p.Duration = 2;
                     p.Acceleration = 1;
                     p.StartAsync = async () =>
                     {
                         await Task.Delay(2000);
                     };
                     p.Update = () =>
                     {
                         box.Text = $"Width [{grid.Width} ] | Height [ {grid.Height} ]";
                     };
                     p.Completed = () =>
                     {
                         MessageBox.Show("Finished");
                     };
                 })
                 .Start();

// animaiton.Start(); It can also be executed like this
```
<h4 style="color:White">[ 1 - 2 ] Cold Start</h4>

<h5 style="color:white">Transition Definition</h5>

```csharp
        private static TransitionBoard<Grid> _animation = Transition.CreateBoardFromType<Grid>()
            .SetProperty(x => x.Height, 100)
            .SetProperty(x => x.Width, 200);

        private static Action<TransitionParams> _params = (p) =>
        {
            p.Duration = 2;
            p.Acceleration = 1;
            p.StartAsync = async () =>
            {
                await Task.Delay(2000);
            };
            p.Completed = () =>
            {
                MessageBox.Show("Finished");
            };
        };
```

<h5 style="color:white">Apply Transition</h5>

```csharp
grid.BeginTransition(_animation, _params);
```

<h4 style="color:White">[ 1 - 3 ] Make custom properties participate in the transition</h4>

<h5 style="color:white">An interface is provided to allow types to participate in transitions</h5>Please describe how this type computes interpolation

```csharp
    internal class ComplexValue : ILinearInterpolation
    {
        public double DoubleValue { get; set; } = 0;
        public Brush BrushValue { get; set; } = Brushes.Transparent;
        public ComplexValue() { Current = this; }

        public object Current { get; set; }

        public List<object?> Interpolate(object? current, object? target, int steps)
        {
            List<object?> result = new List<object?>(steps);

            ComplexValue old = current as ComplexValue ?? new ComplexValue();
            ComplexValue tar = target as ComplexValue ?? new ComplexValue();

            var doublelinear = ILinearInterpolation.DoubleComputing(old.DoubleValue, tar.DoubleValue, steps);
            var brushlinear = ILinearInterpolation.BrushComputing(old.BrushValue, tar.BrushValue, steps);

            for (int i = 0; i < steps; i++)
            {
                var dou = (double?)doublelinear[i];
                var bru = (Brush?)brushlinear[i];
                var newValue = new ComplexValue();
                newValue.DoubleValue = dou ?? 0;
                newValue.BrushValue = bru ?? Brushes.Transparent;
                result.Add(newValue);
            }

            return result;
        }
```

<h4 style="color:White">[ 1 - 4 ] Stop</h4>

<h5 style="color:white">static method</h5>

```csharp
Transition.Dispose();               // All transitions
Transition.Stop(grid,grid2);        // Only transitions of the selected object
Transition.StopSafe(grid,grid2);    // Only Safe transitions
Transition.StopUnSafe(grid,grid2);  // Only UnSafe transitions
```

<h5 style="color:white">extension method</h5>

```csharp
gd.StopTransition(IsStopSafe: true, IsStopUnSafe: false);
gd.StopTransition(true,false);
//The bool value indicates whether to terminate the Safe/UnSafe transition being performed by the object
```

---

## Mvvm

<h4 style="color:White">[ 2 - 1 ] Automatically generate ViewModel for a partial class</h4>
<p style="font-size:14px;color:wheat">[ VMProperty ] → Automatically generating properties</p>
<p style="font-size:14px;color:wheat">[ VMWatcher ] → Ability to listen for property values to change</p>
<p style="font-size:14px;color:wheat">[ VMInitialization ] → Adds extra logic to the no-argument constructor</p>

```csharp
    internal partial class Class1
    {
        [VMProperty]
        private int _id = -1;

        [VMWatcher]
        private void OnIdChanged(WatcherEventArgs e)
        {
            MessageBox.Show($"oldId{e.OldValue}\nnewId{e.NewValue}");
        }

        [VMInitialization]
        private void UseDefaultId()
        {
           _id = 2024;
        }
    }
```

---

## AOP

<h4 style="color:White">[ 3 - 1 ] Make class aspect-oriented</h4>Note properties/methods that must be public

```csharp
    [AspectOriented]
    internal partial class Class1
    {
        public string Property { get; set; }

        public void Action()
        {

        }
    }
```

<h4 style="color:White">[ 3 - 2 ] Make an interception/coverage</h4>
<p style="font-size:14px">You can execute custom logic before or after calling an operation, or you can override the original execution logic</p>
<p style="font-size:14px;color:wheat">Proxy.Set（ Name , Before , Original , After ）</p>

```csharp
            var c1 = new Class1();

            c1.Proxy.SetMethod(nameof(c1.Action),
                (para, last) => { MessageBox.Show("Intercept method"); return null; },
                null,
                null);
            c1.Proxy.SetPropertyGetter(nameof(c1.Property),
                (para, last) => { MessageBox.Show("Intercept getter"); return null; },
                null,
                null);
            c1.Proxy.SetPropertySetter(nameof(c1.Property),
                (para, last) => { MessageBox.Show("Intercept setter"); return null; },
                null,
                null);

            c1.Proxy.Action();
            var a = c1.Proxy.Property;
```

<h4 style="color:White">[ 3 - 3 ] Other details</h4>
<p style="font-size:14px">1. [ para ] refers to the value passed in for this method call</p>
<p style="font-size:14px">2. [ last ] represents the return value of the previous step</p>
<p style="font-size:14px">3. A null value is passed to indicate no interception/overwriting</p>
<p style="font-size:14px">4. If you don't need to return a value when defining an event, you can simply return null</p>

---

## ObjectPool

<h4 style="color:White">[ 4 - 1 ] Use attributes to configure a class</h4>

```csharp
  [Pool(5, 10)] //The initial number of units is 5. If resources are insufficient, the system automatically expands to a maximum of 10 units
  public class Unit
  {
      public static int count = 1;

      public Unit() { Value = count; count++; }

      public int Value { get; set; } = 1;

      [PoolFetch] //Triggered when the object is removed from the pool
      public void WhileFetch()
      {
          MessageBox.Show($"Fetch {Value}");
      }

      [PoolDispose] //Triggered when the object pool is automatically reclaimed
      public void WhileDispose()
      {
          MessageBox.Show($"Dispose {Value}");
      }

      [PoolDisposeCondition] //If the return value is true, the resource can be reclaimed automatically
      public bool CanDisposed()
      {
          return Value % 2 == 0 ? true : false;
      }
  }
```

<h4 style="color:White">[ 4 - 2 ] Use object pool to manage instances</h4>

(1) Get Instance

```csharp
var unit = Pool.Fetch(typeof(Unit)) as Unit;
if (Pool.TryFetch(typeof(Unit), out var result))
{
    var unit = result as Unit;
}
```

(2) Start Auto Dispose

```csharp
Pool.RunAutoDispose(typeof(Unit), 5000);
```

(3) End Auto Dispose

```csharp
Pool.StopAutoDispose(typeof(Unit));
```

(4) Force resource release

```csharp
var unit = Pool.Fetch(typeof(Unit)) as Unit;
unit.PoolDispose();
```

It is common to have one thread per class to intermittently reclaim objects, which can lead to too many threads, and you won't see this message if there are optimizations in future versions

---

## Theme

<h4 style="color:White">[ 5 - 1 ] Marking Theme Properties</h4>

<h5 style="color:white">Marking Class</h5>

```csharp
[Theme]
public partial class MyPage
{
    // The constructor is automatically generated by the source generator
}
```

<h5 style="color:white">Defining state values</h5>

```csharp
        [Dark(nameof(Brushes.Tomato))]
        [Light("#1e1e1e")]
        public Brush Color
        {
            get => txt.Foreground;
            set => txt.Foreground = value;
        }

        [Dark(6)]
        [Light(16,1,2,0)]
        public CornerRadius CornerRadius
        {
            get => bor.CornerRadius;
            set => bor.CornerRadius = value;
        }

        [Dark(0.0)]
        [Light(1.0)]
        public double ThemeOpacity
        {
            get => Opacity;
            set => Opacity = value;
        }

        [Dark(1,1,1,1)]
        [Light(5)]
        public Thickness ThemeThickness
        {
            get => bor.BorderThickness;
            set => bor.BorderThickness = value;
        }
```

<h5 style="color:white">Apply Theme</h5>

```csharp
   this.ApplyTheme(typeof(WhenLight)); // Started by the instance itself
   DynamicTheme.Apply(typeof(WhenLight), windowBack: Brushes.White); // Global usage
```

<h4 style="color:White">[ 5 - 2 ] BrushTags</h4>You can use tag when marking a theme value to the Brush

```csharp
        [Light(BrushTags.H1)]
        [Dark(BrushTags.H1)]
        public Brush BrushValue { get; set; } = Brushes.Transparent;
```

BrushTags makes uniform names for key parts under different topics

<h4 style="color:White">[ 5 - 3 ] Change the default theme color</h4>

By default, the library provides two color packages for light and dark themes and an RGB struct that you can use to apply RGBA transformations to Brush, such as making the opacity half of its original value

```csharp
     LightBrushes.H1 = LightBrushes.H1.ToRGB().Scale(1, 0.5).Brush;
     DarkBrushes.H1 = DarkBrushes.H1.ToRGB().Scale(1, 0.5).Brush;
```

<h4 style="color:White">[ 5 - 4 ] Theme Customization</h4>

<h5 style="color:white">To create your own theme, you need to include the following factors</h5>

<p style="color:wheat">1. Attribute & IThemeAttribute</p>

Declare an attribute that implements a given interface. This attribute can be used just like [Light] / [Dark]

<p style="color:wheat">2. IThemeBrushes</p>

Declare a class that implements a given interface to get a color based on a Tag

---

## Xml&Json

### (1)  deserialization

```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   string AbsPathA = Path.Combine(folderName.CreatFolder(), $"{fileName1}.xml");
   string AbsPathB = Path.Combine(folderName.CreatFolder(), $"{fileName2}.json");
   var dataA = File.ReadAllText(AbsPathA);
   var dataB = File.ReadAllText(AbsPathB);

   var result1 = dataA.XmlParse<Person>();
   var result2 = dataB.JsonParse<Person>();
```

### (2) serialization

```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   var target = new Person();

   var result1 = fileName1.CreatXmlFile(folderName.CreatFolder(), target);
   var result2 = fileName2.CreatJsonFile(folderName.CreatFolder(), target);
```

---

## folder

```csharp
   string folderNameA = "FF1";
   string folderNameB = "FF2";
   string folderNameC = "FF3";
   //The folder name

   var result1 = folderNameA.CreatFolder();
   //From the.exe location, create a folder named "FF1"

   var result2 = folderNameC.CreatFolder(folderNameA,folderNameB);
   //From the.exe location, create a folder named "FF1/FF2/FF3"
```

---

## stringConvertor

```csharp
   string valueA = "-123.7";
   string valueB = "TrUE";
   string valueC = "#1e1e1e";
   
   var result1 = valueA.ToInt();
   var result2 = valueA.ToDouble();
   var result3 = valueA.ToFloat();

   var result4 = valueB.ToBool();

   var result5 = valueC.ToBrush();
   var result6 = valueC.ToColor();
   var result7 = valueC.ToRGB();
```

---

## stringMatcher

### (1) Regular

```csharp
   string sourceA = "[1]wkhdkjhk[a][F3]https:awijdioj.mp3fwafw";
   string sourceB = "awdhttps://aiowdjoajfo.comawd&*(&d)*dhttps://tn.comdawd";
   
   var resultA = sourceA.CaptureBetween("https:", ".mp3");

   var resultB = sourceB.CaptureLike("https://", "com");
```

### (2) Fuzzy Match

```csharp
   string template = "abcdefg";

   string sourceA = "abc";
   List<string> sourceB = new List<string>()
   {
       "abcdegf",
       "cbdgafe"
   };

   var similarity1 = sourceA.LevenshteinDistance(template)
   //Returns the shortest edit distance

   var similarity2 = sourceA.JaroWinklerDistance(template)
   //Returns approximation

   var result3 = template.BestMatch(sourceB, 3);
   //Edit the result with a minimum distance of less than 3

   var result4 = template.BestMatch(sourceB, 0.5);
   //The result with the approximation degree greater than 0.5 and the largest
```

---

## RGB

### (1) Properties

| Property | Type   | Description                                             |
|----------|--------|---------------------------------------------------------|
| R        | int    | Gets or sets the red component of the color, ranging from 0 to 255. |
| G        | int    | Gets or sets the green component of the color, ranging from 0 to 255. |
| B        | int    | Gets or sets the blue component of the color, ranging from 0 to 255. |
| A        | int    | Gets or sets the alpha (transparency) component of the color, ranging from 0 to 255. |

### (2) Methods

| Method Name     | Return Type            | Parameters                                      | Description                                                         |
|-----------------|------------------------|-------------------------------------------------|---------------------------------------------------------------------|
| Color           | System.Windows.Media.Color | None                                        | Converts the current `RGB` structure to a WPF `Color` object.       |
| SolidColorBrush | System.Windows.Media.SolidColorBrush | None                                        | Creates a `SolidColorBrush` based on the current color.             |
| Brush           | System.Windows.Media.Brush | None                                        | Creates a `Brush` based on the current color; internally calls `SolidColorBrush`. |
| FromColor       | RGB                    | color: System.Windows.Media.Color              | Static method that creates a new `RGB` structure from a given WPF `Color` object. |
| FromBrush       | RGB                    | brush: System.Windows.Media.Brush             | Static method that attempts to create a new `RGB` structure from a given brush. |
| Scale           | RGB                    | rateR, rateG, rateB, rateA: double            | Scales each color component according to the given ratios.          |
| Scale           | RGB                    | rateRGB, rateA: double                        | Scales the RGB components and transparency of the color by the given ratios. |
| Scale           | RGB                    | rateRGBA: double                             | Uniformly scales all color components by the given ratio.           |
| Delta           | RGB                    | deltaR, deltaG, deltaB, deltaA: int           | Changes each color component by the specified deltas.               |
| Delta           | RGB                    | deltaRGB, deltaA: int                         | Changes the RGB components and transparency of the color by the specified deltas. |
| Delta           | RGB                    | deltaRGBA: int                               | Uniformly changes all color components by the specified delta.      |
| SubA            | RGB                    | newValue: int                                 | Sets the alpha (transparency) component of the color.               |
| SubR            | RGB                    | newValue: int                                 | Sets the red component of the color.                                |
| SubG            | RGB                    | newValue: int                                 | Sets the green component of the color.                              |
| SubB            | RGB                    | newValue: int                                 | Sets the blue component of the color.                               |
| ToString        | string                 | None                                        | Returns a string representation of the current color in the format "RGBA [R,G,B,A]". |

---

## TransitionParams

| Property Name     | Description                                                                                      | Default Value         |
|-------------------|--------------------------------------------------------------------------------------------------|-----------------------|
| Start             | Action to execute before transition starts                                                       | null                  |
| Update            | Action to execute at the start of each frame                                                     | null                  |
| LateUpdate        | Action to execute at the end of each frame                                                       | null                  |
| Completed         | Action to execute after animation completes                                                      | null                  |
| StartAsync        | Asynchronous action to execute before transition starts                                          | null                  |
| UpdateAsync       | Asynchronous action to execute at the start of each frame                                        | null                  |
| LateUpdateAsync   | Asynchronous action to execute at the end of each frame                                          | null                  |
| CompletedAsync    | Asynchronous action to execute after animation completes                                         | null                  |
| IsAutoReverse     | Whether to automatically reverse                                                                 | false                 |
| LoopTime          | Number of loops                                                                                  | 0                     |
| Duration          | Duration of the transition (unit: s)                                                             | 0                     |
| FrameRate         | Transition frame rate (default: 60)                                                              | DefaultFrameRate (60) |
| IsQueue           | Whether to queue execution (default: not queued)                                                 | false                 |
| IsLast            | Whether to clear other queued transitions after completion (default: do not clear)               | false                 |
| IsUnique          | Whether to add to the queue if a similar transition is already queued (default: do not add)      | true                  |
| Acceleration      | Acceleration (default: 0)                                                                        | 0                     |
| IsUnSafe          | Unsafe operation flag indicating unconditional and immediate execution (default: false)          | false                 |
| UIPriority        | UI update priority                                                                               | DefaultUIPriority     |
| IsBeginInvoke     | Whether to use BeginInvoke when updating properties                                              | DefaultIsBeginInvoke  |

---

## UnsafeTransition

If an animation is set to Unsafe, it will normally execute on its own without being interrupted by other animations

In some cases, you can use this feature to create animations like building blocks

Of course, the Transition class provides methods to terminate such transitions

---

## Generator

(1) Only for partial classes

(2) Contains at least one of the AOP, Theme, and VMProperty attributes for the source generator to work

(3) The source generator has built the no-argument constructor

(4) You'll need to regenerate after installing the project or if the content changes

(5) If regenerating does not make the source generator work, you need to restart the editor, and if this still does not work, you need to check for the [.NET Compliler Platform SDK] and [Visual Basic Roslyn] components.