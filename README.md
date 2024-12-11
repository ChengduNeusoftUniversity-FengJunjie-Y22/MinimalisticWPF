# MinimalisticWPF

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
  - [Dynamic Theme](#Theme)
- Other
  - [Xml / Json Operation](#Xml/Json)
  - [Folder Creation](#folder)
  - [string Convertor](#stringConvertor)
  - [string Matcher](#stringMatcher)
  - [RGB Convertor](#RGB)

Details
- [Compared to native animation](#Differences)
- [TransitionParams](#TransitionParams)
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

## Theme

<h4 style="color:White">[ 4 - 1 ] Marking Theme Properties</h4>

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

<h4 style="color:White">[ 4 - 2 ] BrushTags</h4>You can use tag when marking a theme value to the Brush

```csharp
        [Light(BrushTags.H1)]
        [Dark(BrushTags.H1)]
        public Brush BrushValue { get; set; } = Brushes.Transparent;
```

BrushTags makes uniform names for key parts under different topics

<h4 style="color:White">[ 4 - 3 ] Change the default theme color</h4>

By default, the library provides two color packages for light and dark themes and an RGB struct that you can use to apply RGBA transformations to Brush, such as making the opacity half of its original value

```csharp
     LightBrushes.H1 = LightBrushes.H1.ToRGB().Scale(1, 0.5).Brush;
     DarkBrushes.H1 = DarkBrushes.H1.ToRGB().Scale(1, 0.5).Brush;
```

<h4 style="color:White">[ 4 - 4 ] Theme Customization</h4>

<h5 style="color:white">To create your own theme, you need to include the following factors</h5>

<p style="color:wheat">1. Attribute & IThemeAttribute</p>

Declare an attribute that implements a given interface. This attribute can be used just like [Light] / [Dark]

<p style="color:wheat">2. IThemeBrushes</p>

Declare a class that implements a given interface to get a color based on a Tag

---

## Xml/Json

---

## folder

---

## stringConvertor

---

## stringMatcher

---

## RGB

---

## Differences

---

## TransitionParams

---

## Generator