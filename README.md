﻿# MinimalisticWPF
- [中文文档](https://chengduneusoftuniversity-fengjunjie-y22.github.io/MinimalisticWPFDocumentation/)

---

## Change
<details>
<summary>V1.5.6</summary>

  - Repair it.
    - Gradient results may be distorted when the frame rate is between 57 and 61
    - When the frame rate is lower than 100, high-speed State switching may cause startup failure of the state machine
    - When the frame rate is 0, an error occurs
    - An error occurs when the duration is 0
  - Adjust
    - The default frame rate is set to 120Hz
    - Frame rate is limited from 1 to 240, out of range will be corrected automatically
  - New
    - class.IsSatisfy() allows you to decide whether to initiate a pre-described transition based on whether the instance object meets a specified condition
    ```csharp
            var board = GD.Transition()
                .SetProperty(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
                .SetParams((x) =>
                {
                    x.Duration = 3;
                    x.Acceleration = 1;
                });

            var result = GD.IsSatisfy(x => x.Width < 1000, board, true);
            // Parameter 1. Condition (required)
            // Parameter 2. Perform this transition if the conditions are met (optional)
            // Parameter 3. If the transition effect is object-based, whether to enable the whitelist mechanism (optional)
    ```
  - 2.0.0 version preview
    - ★ Greatly optimize the document
    - ★ Open up more functions that can only operate inside the state machine system
    - Fix more potential issues
    - Try to optimize performance further
    - Add more common extension methods
    - ⚠ Remove all non-core components
</details>

<details>
<summary>V1.8.0</summary>

  - UnSafe Mechanism - Allows you to separate and compose complex animations
  - Unified API style
  - Open up more content that was previously only available inside the state machine system
  - Better code use cases
</details>

# V1.8.x

## Ⅰ API Change
- AnyClass.StateMachineTransfer

```csharp
            GD.Transition()
                .SetProperty(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
                .SetProperty(x => x.Opacity, 0.2)
                .SetParams((x) =>
                {
                    x.Duration = 0.4;
                })
                .Start();
```

## Ⅱ New API
- New Methods to start a predefined animation
  - AnyClass.BeginTransition()
  - AnyClass.IsSatisfy()
```csharp
private void OnClick1(object sender, System.Windows.Input.MouseButtonEventArgs e)
{
    GD.BeginTransition(Compile);
    //[Compile] is a State or TransitionBoard

    GD.IsSatisfy(x => x.ActualWidth < 1000, Compile);
    //Add a condition before [Compile] start run
}
```

---

- Get the state machine instance of the control instance
  - AnyClass.FindStateMachine()
```csharp
var machine = source.FindStateMachine();
```

---

  - API provided by StateMachine

|Property|class|default|Meaning|
|-----------|------------|--------------|-----------|
|MaxFrameRate|int|120 Hz|Maximum frame rate limit|
|States|StateCollection||A collection of State|

|Method|Params|Meaning|
|-----------|--------------|---------|
|ReSet||Resetting the animation data of state machine|

---

## Ⅲ Practice
- First, you need to define the animation beforehand
```csharp
// Detail parameters, it is recommended to define them before all animations to avoid problems caused by static field positions
// defined here:
// Unsafe animation that lasts 2 seconds
// 2.Animation that is safe and lasts 2 seconds
private static Action<TransitionParams> _tp = (x) =>
{
    x.IsUnSafe = true;
    x.Duration = 2;
};
private static Action<TransitionParams> _tp2 = (x) =>
{
    x.IsUnSafe = false;
    x.Duration = 2;
};

// rotation, translation, and scale effects defined
private static RotateTransform rotateTransform = new RotateTransform(90, 50, 50);
private static TranslateTransform translateTransform = new TranslateTransform(-100, -50);
private static ScaleTransform scaleTransform = new ScaleTransform(2, 2, 50, 50);

// Static drawing boards, usually used to create [global, non-dynamic] animations
// The drawing board can be either State or TransitionBoard
// defined here:
//1. A rotation, translation, and scale animation UnSafe
//2. A background color change UnSafe
//3. A compound animation Safe
private static State TransformA = State.FromType<Grid>()
    .SetName("TransformChange1")
    .SetProperty(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
    .ToState();
private static TransitionBoard<Grid> ColorA = Transition.CreateBoardFromType<Grid>()
    .SetProperty(x => x.Background, Brushes.Tomato)
    .SetParams(_tp);
private static TransitionBoard<Grid> Compile = Transition.CreateBoardFromType<Grid>()
    .SetProperty(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
    .SetProperty(x => x.Background, Brushes.Tomato)
    .SetParams(_tp2);

// Instance drawing boards, which are used to create [local, dynamic] animations
// defined here:
// 1.An opacity change animation UnSafe
private TransitionBoard<Grid> OpacityA = Transition.CreateBoardFromType<Grid>()
    .SetProperty(x => x.Opacity, 0)
    .SetParams(_tp);
```
- Then you can start using those animations ( select one way from bottom )
```csharp
private void OnClick1(object sender, System.Windows.Input.MouseButtonEventArgs e)
{
    //☆ Each instance can render a different opacity transformation based on the result
    if (GD.ActualWidth < 500)
    {
        OpacityA.SetProperty(x => x.Opacity, 0.6);
    }

    //1.With Safe, an animation must be interrupted when it starts executing
    GD.BeginTransition(Compile);

    // 2.With UnSafe, each animation component runs independently and cannot be interrupted
    // You can combine multiple UnSafe components, but make sure they don't conflict with each other and that no other conflicting animations are added during execution
    GD.BeginTransition(TransformA, _tp);
    GD.BeginTransition(ColorA);
    GD.BeginTransition(OpacityA);

    //3. We can directly check if the object meets the specified conditions, and then use the result to decide whether to start the animation defined by the drawing board
    //☆ Note: OpacityA needs to start before Compile because it must interrupt the Safe animation
    GD.IsSatisfy(x => x.ActualWidth < 1000, OpacityA);
    GD.IsSatisfy(x => x.ActualHeight < 1000, Compile);
}
```


---
---
---
---
---
---

# V1.5.x

## Key Features
- [State Machine System - Create linear transitions to specified properties of specified instances](#StateMachineSystem)
  - [StateMachine]()
  - [TransitionParams]()
  - [MVVM]()
  - Property types that can participate in state machine transitions
    - double
    - Brush
    - Transform
    - CornerRadius
    - Thickness
    - Point
    - ★ ILinearInterpolation (This interface allows any custom type to support state machine transitions)
## Auxiliary Features
- [ExtensionMethods](#ExtensionMethods)
  - [string]
    - value conversion
    - Fuzzy matching
    - Profiling tools (e.g. extracting resource addresses from html)
    - Password strength
  - [UserControls](#UserControls)
    - A uniform dark theme
    - Font size ADAPTS to control height
    - All animation effects are based on state machine systems, which are both directly available and practices of the state machine system
      - Notification - Glass style notification/select box
      - MProgressBar - Bar/ring free switching progress bar
      - mtopbar - Top sidebar of the program
## Non-core Components
The MinimalisticWPF namespace does not include the following services, which will be referenced separately
  - [Web Services](#WebServices)
    - [ Autonavi ]() WebApi
      - IP Services
      - Weather services

## Supporting frameworks
- [.NET6.0-windows]
- [.NET8.0-windows]
## Getting
- [github][1]
- [nuget][2]

[1]: https://github.com/ChengduNeusoftUniversity-FengJunjie-Y22/MinimalisticWPF
[2]: https://www.nuget.org/packages/MinimalisticWPF/

# StateMachineSystem
- State & StateVecotr is built for MVVM and allows for automatic state switching with preset conditions
- StateMachine theory can make linear transitions for any type of property. Almost all types non-MVVM include an extension method for quickly creating linear transitions for object instances. This is probably the library's most common method
  - ★ Advantages
    - Create complex transitions with little code
    - Update() allows you to decide what you want to do for each frame of the transition, just like in game development engines
    - The functionality is not limited to animations, it modifies any possible type, any possible property, for example testing the program with simulated data is one of the uses the authors tried
  - ⚠ ️ Disadvantages
    - Unstable performance (relative to components like StoryBoard and VisualState)
    - The types of properties that support transitions are very limited, and although an interface is provided to solve this problem, it is not convenient to leave the calculation of linear interpolation to the implementation class of the interface
[![pAu2vOP.md.png](https://s21.ax1x.com/2024/09/15/pAu2vOP.md.png)](https://imgse.com/i/pAu2vOP)

---

## StateMachine
- For any type [where T: class, new()] you can create a linear transition using the following code
- For example perform the following transition on a 100×100 Grid
```csharp
        private RotateTransform rotateTransform = new RotateTransform(-280, 50, 50);
        private TranslateTransform translateTransform = new TranslateTransform(-100, -50);
        private ScaleTransform scaleTransform = new ScaleTransform(2, 2, 50, 50);

        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.Transition()
                .SetProperty(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
                .SetProperty(x => x.Opacity, 0.2)
                .SetProperty(x => x.CornerRadius,new CornerRadius(15))
                .SetParams((x) =>
                {
                    x.Duration = 0.4;
                    x.Completed = () =>
                    {
                        Notification.Message("Transition complete √");
                    };
                })
                .Start();
        }
```
- But the default properties available for state machine transitions are finite types, so how do you make custom types work with state machines?
  - Step 1. Implement a Class1, which is a custom type that supports state machine transitions
    - Here Class1 is the composition of Thickness and CornerRadius
    - Need to implement the interface method Interpolate (), steps is the number of interpolations, you need to customize how to split the two Class1 into steps to evenly interpolate
    ```csharp
    public class Class1 : ILinearInterpolation
    {
        public object Current { get; set; }
        public List<object?> Interpolate(object? current, object? target, int steps)
        {
            List<object?> result = new List<object?>();

            var v1 = current as Class1 ?? new Class1();
            var v2 = target as Class1 ?? new Class1();
            var itemsA = ILinearInterpolation.CornerRadiusComputing(v1.CornerRadius, v2.CornerRadius, steps);
            var itemsB = ILinearInterpolation.ThicknessComputing(v1.Thickness, v2.Thickness, steps);
            for (var i = 0; i < itemsA.Count; i++)
            {
                var temp = new Class1();
                temp.CornerRadius = itemsA[i] as CornerRadius? ?? new CornerRadius();
                temp.Thickness = itemsB[i] as Thickness? ?? new Thickness();
                result.Add(temp);
            }

            return result;
        }


        public Class1() { Current = this; }
        public Class1(CornerRadius cornerRadius, Thickness thickness) { Current = this; CornerRadius = cornerRadius; Thickness = thickness; }
        public CornerRadius CornerRadius { get; set; } = new CornerRadius();
        public Thickness Thickness { get; set; } = new Thickness();
    }
    ```
  - Step 2. Implement a Class2, which is the type that contains the Class1 property, is the type that actually needs to use the state machine
    ```csharp
    public class Class2
    {
        public Class2() { }
       
        public Class1 Class1 { get; set; } = new Class1();

        //…… other properties
    }
    ```
  - Step 3. At this point, you are ready to apply state machine transitions to Class2.Class1
    ```csharp
       Class1 T1 = new Class1();
       Class1 T2 = new Class1(new CornerRadius(10), new Thickness(2, 3, 1, 0));
  
       Class2 TargetClass2 = new Class2();
  
       TargetClass2.Transition()
           .SetProperty(x => x.Class1, T2)
           .SetParams((x) =>
           {
               x.Duration = 2;
               x.Start = () =>
               {
                    Notification.Message($"old Thickness {TargetClass2.Class1.Thickness}\n" +
                            $"old CornerRadius {TargetClass2.Class1.CornerRadius}");
               };
               x.Completed = () =>
               {
                    Notification.Message($"new Thickness {TargetClass2.Class1.Thickness}\n" +
                            $"new CornerRadius {TargetClass2.Class1.CornerRadius}");
               };
           })
           .Start();
    ```

---

## TransitionParams 
- Contains a number of parameters for the details of the transition
  - Transition parameters
  - Transition creation parameters
  - Lifecycle related parameters

| property | type | default | meaning |
|--------|-----|-------|-------|
|Duration|double|0| Animation duration (in s)|
|Start|Action|null| is executed once before the animation starts |
|Update|Action|null| is executed once before each frame of the animation starts |
|LateUpdate|Action|null| is executed once after each frame of the animation |
|Completed|Action|null| is executed once after the animation has finished |
|IsQueue|bool|false| Whether the newly enabled animation will be queued or not, otherwise the animation will be interrupted |
|IsLast|bool|false| Whether this is the last animation to be executed, if so it will clear the queued animation |
|IsUnique|bool|true| Should a transition animation that points to the same State continue if one exists |
|FrameRate|int|165| Animation frame rate |
|WaitTime|double|0.008| is rarely used, but if you find places where the probability doesn't animate or the probability is twitching, you can increase this value appropriately|
|Acceleration|double|0|The waiting time of each frame is shown as a straight line with slope [Acceleration] in the floor plan|

- Use cases
  - Set transition parameters (lambdas) for StateVector
  - Set a transition parameter (Lambda) for Transition()

```csharp
Set((x)=>
{
    x.Duration = 0.1;
    x.IsLast = true;
    x.Update = () =>
    {
        Notification.Message("Before the start of a frame");
    };
})
```

---

## State & StateVector & IConditionalTransition
- State describes the value of an object's property at a moment in time
- StateVector describes which transitions are created under which conditions
- IConditionalTransition allows you to automatically create a transition when a specified condition is met on an instance object
  - Examples
    - When the mouse is inside the control, make its background opacity transition to 0.2
    - Make the background opacity transition to 0 when the mouse leaves the control
    - Xaml - View
    ```xml
    <UserControl.DataContext>
        <local:MButtonViewModel x:Name="ViewModel"/>
    </UserControl.DataContext>
    ```
    - C# - View
    ```csharp
    public partial class MButton : UserControl
    {
        public MButton()
        {
            InitializeComponent();
            this.StateMachineLoading(ViewModel);
        }
    }
    ```
    - C# - ViewModel
    ```csharp
    public class MButtonViewModel : ViewModelBase<MButtonViewModel, MButtonModel>
    {
        public MButtonViewModel() { }

        public static State Start = State.FromObject(new MButtonViewModel())
            .SetName("defualt")
            .SetProperty(x => x.HoverBackgroundOpacity, 0)
            .ToState();
        public static State MouseIn = State.FromObject(new MButtonViewModel())
            .SetName("mouseInside")
            .SetProperty(x => x.HoverBackgroundOpacity, 0.2)
            .ToState();

        public StateVector<MButtonViewModel> Condition { get; set; } = StateVector<MButtonViewModel>.Create()
            .AddCondition(x => x.IsMouseInside, MouseIn, (x) => { x.Duration = 0.2; })
            .AddCondition(x => !x.IsMouseInside, Start, (x) => { x.Duration = 0.2; });

        public override bool IsMouseInside
        {
            get => base.IsMouseInside;
            set
            {
                base.IsMouseInside = value;
  
                OnConditionsChecked();
                // Change IsMouseInside when mouse in/out of control
                //IsMouseInside is modified to check if the condition is true, if so, switch State
            }
        }
    }
    ```

---
---

# ExtensionMethods
## string
- Value conversion
```csharp
   string valueA = "-123.7";
   string valueB = "TrUE";
   string valueC = "#1e1e1e";
   
   var result1 = valueA.ToInt();
   var result2 = valueA.ToDouble();
   var result3 = valueA.ToFloat();

   var result4 = valueB.ToBool();

   var result5 = valueC.ToBrush();
```
- Fuzzy matching
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
- Folder generation operations
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
- Xml and Json serialization
```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   var target = new Person();

   var result1 = fileName1.CreatXmlFile(folderName.CreatFolder(), target);
   var result2 = fileName2.CreatJsonFile(folderName.CreatFolder(), target);
```
- Xml and Json deserialization
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
- Regular operation
```csharp
   string sourceA = "[1]wkhdkjhk[a][F3]https:awijdioj.mp3fwafw";
   string sourceB = "awdhttps://aiowdjoajfo.comawd&*(&d)*dhttps://tn.comdawd";
   
   var resultA = sourceA.CaptureBetween("https:", ".mp3");

   var resultB = sourceB.CaptureLike("https://", "com");
```
- Password strength
```csharp
   string password = "12345678";
   int Level = password.CheckPasswordStrength(MinLength=8);
```

---
---

# UserControls
- ## ☆ Using
  - C#
    ```csharp
    using MinimalisticWPF;
    ```
  - XAML
    ```xml
    xmlns:mn="clr-namespace:MinimalisticWPF;assembly=MinimalisticWPF"
    ```
- ## ☆ MButton
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLPyQ.png)
  ## Property
  - Click
  - WiseHeight
  - WiseWidth
  - Text
  - TextBrush
  - FontSizeRatio
  - EdgeBrush
  - EdgeThickness
  - HoverBrush
  - CornerRadius
---
- ## ☆ MTopBar
  ![pAmMfv8.md.png](https://s21.ax1x.com/2024/09/09/pAmMfv8.md.png)
  ![pAmQnVH.md.png](https://s21.ax1x.com/2024/09/09/pAmQnVH.md.png)
  ## Property
  - WiseHeight
  - WiseWidth
  - Title
  - SizeRatio
  - EdgeBrush
  - EdgeThickness
  - HoverBrush
  - CornerRadius
  - Icon
---
- ## ☆ MPasswordBox
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLEoq.png)
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLQOJ.png)
  ## Property
  - WiseHeight
  - WiseWidth
  - FontSizeRatio
  - Password
  - Replace
---
- ### ☆ MProgressBar
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLkes.png)
  ## Property
  - Size
  - Value
  - Shape
  - Thickness
  - BottomBrush
  - FillBrush
  - TextBrush
  - FontSizeRatio
  - IsReverse
  - StartAngle
  - EndAngle
---
- ### ☆ Notification
  [![pAKM40S.png](https://s21.ax1x.com/2024/09/18/pAKM40S.png)](https://imgse.com/i/pAKM40S)
  [![pAKMhm8.png](https://s21.ax1x.com/2024/09/18/pAKMhm8.png)](https://imgse.com/i/pAKMhm8)
  ## 消息框调用示例
  ```csharp
            if (Notification.Select("Are you sure you want to check the weather ?"))
            {
                Notification.Message("Weather");
            }
  ```

---
---

# WebServices
- ##  Autonavi
  - using
    ```csharp
    using MinimalisticWPF.GaoDeServices;
    ```
  - Get ApiKey https://console.amap.com/dev/key/app
    ```csharp
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GaoDeAPISet.Awake(Key: "Your_Api_Key", IsUpdateIP: true);
        }
    ```
  - IPService
    - Get IP
      ```csharp
      var ip = await IPService.GetIP();
      MessageBox.Show(ip.GetCombined());
      ```
    - Get AdCode
      ```csharp
      var adcode = await IPService.GetAdCode("都江堰");
      MessageBox.Show(adcode);
      ```
  - WeatherService
    - Get the weather based on the current IP address
      ```csharp
      var weathers = await WeatherService.GetWeathers();
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - Get the weather by region name
      ```csharp
      var weathers = await WeatherService.GetWeathers("都江堰");
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - weather[0] Is for today's weather