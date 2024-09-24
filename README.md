# MinimalisticWPF
- [Documentation-English](#English)
- [文档-中文](#中文)

---

# English

## Change
<details>
<summary>V1.5.0 Soon to be deprecated</summary>

  - StateMachineTransfer() changed to support any [where T: class,new()] type
  - State restricts modification. It needs to be a public static field or a public property
  - StateVector restricts modification and requires writing public properties
  - Support for Transform transitions
    - Translate
    - Scale
    - Rotate
  - Support for Point transitions
  - CornerRadius transitions are supported
  - Supports Thickness transitions
  - The interface ILinearInterpolation allows custom types as transitionable properties
</details>

<details>
<summary>V1.5.1 Soon to be deprecated</summary>

  - Acceleration support
  - Remove some unnecessary object generation operations when the state machine is running
</details>

<details>
<summary>V1.5.2 Soon to be deprecated</summary>

  - Fixed the problem of abnormal animation effect in NET6.0 framework
</details>

<details>
<summary>V1.5.3 Soon to be deprecated</summary>

  - State performance optimization
    - Object-based
      - A lot of reflection/LINQ during State initialization
      - Allows automatic logging of all supported properties of an object
    - Type-based
      - Do not perform any reflection/LINQ operations during State initialization
      - You can only manually specify the properties that need to be modified
    - Use
      - State.FromType<>() / StateMachineTransfer( StateRecordModes.Type )
      - State.FromObject() / StateMachineTransfer( StateRecordModes.Object )
   - AnimationInterpreter logic optimization
     - Addressed twitching issues that could be caused by frequent State switching
</details>

<details>
<summary>V1.5.4</summary>

  - Fixed interpolation algorithm provided by ILinearInterpolation
    - Gradient results may be distorted when the frame rate is between 57 and 61
</details>

## Key Features
- [State Machine System - Create linear transitions to specified properties of specified instances](#StateMachineSystem)
  - [StateMachine]()
  - [TransferParams]()
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
            GD.StateMachineTransfer()
                .Add(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
                .Add(x => x.Opacity, 0.2)
                .Add(x => x.CornerRadius,new CornerRadius(15))
                .Set((x) =>
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
  
       TargetClass2.StateMachineTransfer()
           .Add(x => x.Class1, T2)
           .Set((x) =>
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

## TransferParams 
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
  - Set a transition parameter (Lambda) for StateMachineTransfer()

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

## State & StateVector & IConditionalTransfer
- State describes the value of an object's property at a moment in time
- StateVector describes which transitions are created under which conditions
- IConditionalTransfer allows you to automatically create a transition when a specified condition is met on an instance object
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

---
---
---
---
---
---

# 中文

## 修改 
<details>
<summary>V1.5.0 即将弃用</summary>

  - StateMachineTransfer() 改为支持任何 [ where T : class ,new() ] 类型
  - State限制修改，需要是公开静态字段或公开属性
  - StateVector限制修改，需要写作公开属性
  - 支持Transform过渡
    - Translate
    - Scale
    - Rotate
  - 支持Point过渡
  - 支持CornerRadius过渡
  - 支持Thickness过渡
  - 接口ILinearInterpolation允许自定义的类型作为可过渡属性
</details>

<details>
<summary>V1.5.1 即将弃用</summary>

  - Acceleration 加速度支持
  - 删除状态机运作时,部分不必要的对象生成操作 
</details>

<details>
<summary>V1.5.2即将弃用</summary>

  - 修复了NET6.0框架下动画效果异常的问题
</details>

<details>
<summary>V1.5.3即将弃用</summary>

  - State 性能优化
    - 基于 Object
      - 在State初始化时执行了大量反射、LINQ操作
      - 允许自动化地记录一个object所有受支持的属性
    - 基于 Type
      - 在State初始化时不执行任何反射、LINQ操作
      - 只能手动指定需要修改的属性
    - 使用
      - State.FromType<>() / StateMachineTransfer( StateRecordModes.Type )
      - State.FromObject() / StateMachineTransfer( StateRecordModes.Object )
  - AnimationInterpreter 逻辑优化
    - 解决了频繁切换State可能导致的抽搐问题
</details>

<details>
<summary>V1.5.4</summary>

  - 修正 ILinearInterpolation 提供的插值算法
    - 帧率处于 57~61 时，渐变结果可能失真
</details>

## 核心功能
- [状态机系统 - 对指定实例的指定属性创建线性过渡](#状态机系统)
    - [StateMachine]() 
    - [TransferParams]()
    - [MVVM]()
    - 可参与状态机过渡的属性类型
      - double
      - Brush
      - Transform      
      - CornerRadius
      - Thickness
      - Point
      - ★ ILinearInterpolation ( 该接口使得任意自定义类型支持状态机过渡 )
## 辅助功能
- [扩展方法](#扩展方法)
    - [string]
      - 值转换
      - 模糊匹配
      - 分析工具（例如提取html中的资源地址）
      - 密码强度
- [用户控件](#用户控件)
    - 统一的深色主题
    - 字体大小自适应控件高度
    - 所有动画效果都基于状态机系统,它们既是直接可用的,也是状态机系统的实践
      - Notification - 玻璃风格的通知/选择框
      - MProgressBar - 条状/环状自由切换的进度条
      - MTopBar - 程序顶侧栏
## 非核心组件
###### MinimalisticWPF命名空间不包括下述服务，另需引用
- [Web服务](#Web服务)
  - [ 高德 ] WebApi
    - IP服务
    - 天气服务

## 支持框架
- [.NET6.0-windows] 
- [.NET8.0-windows]
## 获取
- [github][1]
- [nuget][2]

[1]: https://github.com/ChengduNeusoftUniversity-FengJunjie-Y22/MinimalisticWPF
[2]: https://www.nuget.org/packages/MinimalisticWPF/

---

# 状态机系统
- State & StateVecotr 为MVVM而生 ,可通过预先设置条件实现状态的自动切换
- StateMachine 理论可对任何类型的属性做出线性过渡，非MVVM下，几乎所有类型都包含一个用于快速为对象实例创建线性过渡的扩展方法，这可能是该类库最常用的方法
- ★ 优势
  - 以少量代码创建复杂过渡效果
  - 像游戏开发引擎那样,可在Update()中决定过渡效果的每一帧你要做出的行为
  - 功能并不局限于动画,它修改任何可能的类型、任何可能的属性,例如使用模拟数据对程序进行测试正是作者在尝试中的用法之一
- ⚠ 劣势
  - 性能不稳定 ( 相对于StoryBoard与VisualState等组件而言 )
  - 支持过渡的属性类型受限较大，虽然提供了接口以解决这个问题，但将线性插值的计算交由接口的实现类仍然不是方便的做法
[![pAu2vOP.md.png](https://s21.ax1x.com/2024/09/15/pAu2vOP.md.png)](https://imgse.com/i/pAu2vOP)

## StateMachine
- 对于任何类型 [ where T : class , new() ] 可使用如下代码创建线性过渡
- 例如对一个100×100的Grid执行以下过渡
```csharp
        private RotateTransform rotateTransform = new RotateTransform(-280, 50, 50);
        private TranslateTransform translateTransform = new TranslateTransform(-100, -50);
        private ScaleTransform scaleTransform = new ScaleTransform(2, 2, 50, 50);

        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.StateMachineTransfer()
                .Add(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
                .Add(x => x.Opacity, 0.2)
                .Add(x => x.CornerRadius,new CornerRadius(15))
                .Set((x) =>
                {
                    x.Duration = 0.4;
                    x.Completed = () =>
                    {
                        Notification.Message("过渡完成 √");
                    };
                })
                .Start();
        }
```
- 但默认可用状态机过渡的属性终究是有限类型,如何让自定义的类型也能支持状态机呢?
  - 步骤1.实现一个Class1，它是支持状态机过渡的自定义类型
    - 这里Class1是Thickness和CornerRadius的复合
    - 需要实现接口方法Interpolate（）, steps是插值数量 , 你需要自定义如何将两个 Class1 拆成steps份均匀的插值
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
  - 步骤2.实现一个Class2，它是包含Class1属性的类型,是实际需要使用状态机的类型
    ```csharp
    public class Class2
    {
        public Class2() { }
       
        public Class1 Class1 { get; set; } = new Class1();

        //…… 其它属性
    }
    ```
  - 步骤3.到这一步，你已可以对Class2.Class1应用状态机的过渡功能
    ```csharp
       Class1 T1 = new Class1();
       Class1 T2 = new Class1(new CornerRadius(10), new Thickness(2, 3, 1, 0));
  
       Class2 TargetClass2 = new Class2();
  
       TargetClass2.StateMachineTransfer()
           .Add(x => x.Class1, T2)
           .Set((x) =>
           {
               x.Duration = 2;
               x.Start = () =>
               {
                    Notification.Message($"旧的 Thickness {TargetClass2.Class1.Thickness}\n" +
                            $"旧的 CornerRadius {TargetClass2.Class1.CornerRadius}");
               };
               x.Completed = () =>
               {
                    Notification.Message($"新的 Thickness {TargetClass2.Class1.Thickness}\n" +
                            $"新的 CornerRadius {TargetClass2.Class1.CornerRadius}");
               };
           })
           .Start();
    ```
---
## TransferParams 
- 包含系列参数用于修饰此次过渡效果的细节
  - 过渡效果相关的参数
  - 过渡创建相关的参数
  - 生命周期相关的参数

|属性|类型|默认|意义|
|--------|-----|-------|-------|
|Duration|double|0|动画持续时间 ( 单位: s )|
|Start|Action|null|在动画开始前执行一次|
|Update|Action|null|在动画的每一帧开始前执行一次|
|LateUpdate|Action|null|在动画的每一帧结束后执行一次|
|Completed|Action|null|在动画结束后执行一次|
|IsQueue|bool|false|新启用的动画是否排队,不排队就意味着会打断正在执行的动画|
|IsLast|bool|false|是否为最后一个被执行的动画,如果是则意味着会清空正在排队中的动画|
|IsUnique|bool|true|如果存在一个指向同一State的过渡动画,是否还要继续执行此动画|
|FrameRate|int|165|动画帧率|
|WaitTime|double|0.008|基本用不到,但如果发现有些地方概率无法触发动画或者概率抽搐,则可适当增加这个值|
|Acceleration|double|0|加速度,使得每一帧的等待时间在平面图中呈现出斜率为Acceleration的直线|

- 应用场景
  - 为StateVector设置过渡参数 ( Lambda )
  - 为StateMachineTransfer()设置过渡参数 ( Lambda )

```csharp
Set((x)=>
{
    x.Duration = 0.1;
    x.IsLast = true;
    x.Update = () =>
    {
        Notification.Message("一帧开始前");
    };
})
```
---
## State & StateVector & IConditionalTransfer
- State描述某一时刻对象的属性值
- StateVector描述在何种条件下创建何种过渡
- IConditionalTransfer接口允许在实例对象达成指定条件时自动创建过渡
  - 示例
    - 鼠标进入控件时,令其背景透明度过渡为0.2
    - 鼠标离开控件时,令其背景透明度过渡为0
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
                //鼠标进出控件时修改IsMouseInside
                //IsMouseInside被修改时检查是否满足条件,若满足,则切换State
            }
        }
    }
    ```

---
---

# 扩展方法
## string
- 值转换
```csharp
   string valueA = "-123.7";
   string valueB = "TrUE";
   string valueC = "#1e1e1e";
   //三个待被转换的值
   
   var result1 = valueA.ToInt();
   var result2 = valueA.ToDouble();
   var result3 = valueA.ToFloat();
   //转换成数字

   var result4 = valueB.ToBool();
   //转换成bool

   var result5 = valueC.ToBrush();
   //转换成Brush
```
- 模糊匹配
```csharp
   string template = "abcdefg";
   //这是待匹配的字符串

   string sourceA = "abc";
   List<string> sourceB = new List<string>()
   {
       "abcdegf",
       "cbdgafe"
   };
   //这是匹配源

   var similarity1 = sourceA.LevenshteinDistance(template)
   //返回最短编辑距离

   var similarity2 = sourceA.JaroWinklerDistance(template)
   //返回近似度

   var result3 = template.BestMatch(sourceB, 3);
   //编辑距离小于3且最小的结果

   var result4 = template.BestMatch(sourceB, 0.5);
   //近似度大于0.5且最大的结果
```
- 文件夹生成操作
```csharp
   string folderNameA = "FF1";
   string folderNameB = "FF2";
   string folderNameC = "FF3";
   //文件夹的名字

   var result1 = folderNameA.CreatFolder();
   //从.exe位置开始,创建名为"FF1"的文件夹

   var result2 = folderNameC.CreatFolder(folderNameA,folderNameB);
   //从.exe位置开始,创建名为"FF1/FF2/FF3"的文件夹
```
- Xml 和 Json 序列化
```csharp
   string folderName = "Data";
   //假设文件需要放在Data文件夹内

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";
   //假设这是两份文件的文件名

   var target = new Person();
   //假设需要将一个Person实例序列化存储

   var result1 = fileName1.CreatXmlFile(folderName.CreatFolder(), target);
   var result2 = fileName2.CreatJsonFile(folderName.CreatFolder(), target);
   //分别存储为.xml和.json文件
```
- Xml 和 Json 反序列化
```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   string AbsPathA = Path.Combine(folderName.CreatFolder(), $"{fileName1}.xml");
   string AbsPathB = Path.Combine(folderName.CreatFolder(), $"{fileName2}.json");
   var dataA = File.ReadAllText(AbsPathA);
   var dataB = File.ReadAllText(AbsPathB);
   //拿到原始数据

   var result1 = dataA.XmlParse<Person>();
   var result2 = dataB.JsonParse<Person>();
   //反序列化
```
- 正则操作
```csharp
   string sourceA = "[1]wkhdkjhk[a][F3]https:awijdioj.mp3fwafw";
   string sourceB = "awdhttps://aiowdjoajfo.comawd&*(&d)*dhttps://tn.comdawd";
   //原始数据
   
   var resultA = sourceA.CaptureBetween("https:", ".mp3");
   //捕获所有包含在"https:"和".mp3"中间的字符串

   var resultB = sourceB.CaptureLike("https://", "com");
   //捕获所有符合先出现"https://"再出现"com"特征的字符串,特征可以是多个
```
- 密码强度
```csharp
   string password = "12345678";
   int Level = password.CheckPasswordStrength(MinLength=8);
   //返回0~4的整数代表密码强度
```

---
---

# 用户控件
- ## ☆ Using
  - C#
    ```csharp
    using MinimalisticWPF;
    ```
  - XAML
    ```xml
    xmlns:mn="clr-namespace:MinimalisticWPF;assembly=MinimalisticWPF"
    ```
  - 不使用Width/Height调整大小,而是带有Wise、Size字样的属性,最后通过Margin调整位置
  - 需要使用例如"#1e1e1e"的深色背景板,很多控件都是白边透明底的,浅色背景板效果不佳
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

# Web服务
- ## 高德
  - using
    ```csharp
    using MinimalisticWPF.GaoDeServices;
    ```
  - 前往获取ApiKey https://console.amap.com/dev/key/app
    ```csharp
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GaoDeAPISet.Awake(Key: "Your_Api_Key", IsUpdateIP: true);
            //[ IsUpdateIP ] 描述本次加载是否要重新读取IP信息
        }
    ```
  - IPService
    - 获取当前IP
      ```csharp
      var ip = await IPService.GetIP();
      MessageBox.Show(ip.GetCombined());
      ```
    - 依据地区名称获取行政编码
      ```csharp
      var adcode = await IPService.GetAdCode("都江堰");
      MessageBox.Show(adcode);
      ```
  - WeatherService
    - 依据当前IP获取天气
      ```csharp
      var weathers = await WeatherService.GetWeathers();
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - 依据地区名称获取天气
      ```csharp
      var weathers = await WeatherService.GetWeathers("都江堰");
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - weather[0] 代表今天的天气
---
