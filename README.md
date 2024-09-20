# MinimalisticWPF
- [English](#English)
- [Chinese](#中文文档)

# English

## Target
- [StateMachine System](#StateMachine)
    - [State](#State) Describes the attribute value corresponding to the state
    - [StateVector](#StateVector) Describes the state that should be switched to when certain conditions are met
    - [StateMachine](#StateMachine) Make a linear change to the attribute value
    - [StateViewModelBase< T >](#StateViewModelBase) Types implementing this interface automatically switch State based on the StateVector
    - [TransferParams](#TransferParams) Describes the effect of a linear transition
    - Currently supported property types
      - double
      - Brush
      - Transform
      - Point
      - CornerRadius
      - Thickness
      - ILinearInterpolation
- [WebServices](#WebServices)
  - [ GaoDe ] WebApi
    - IPService
    - WeatherService
- [Minimalistic UserControls](#MinimalisticUserControls)
    - Uniform dark theme
    - All animations based on StateMachine
- [Extension Methods](#ExtensionMethods)
    - [string](#string)
      - Value conversion
      - Fuzzy matching
      - Crawler analysis
      - Password strength
	- FrameworkElement
      - Linear animation based on StateMachine
## Framework
- [.NET6.0-windows] 
- [.NET8.0-windows]
## Get
- [github][1]
- [nuget][2]

[1]: https://github.com/ChengduNeusoftUniversity-FengJunjie-Y22/MinimalisticWPF
[2]: https://www.nuget.org/packages/MinimalisticWPF/

## Change
- V1.5.0
  - StateMachineTransfer () changed to support any where T: class,new()
  - State is reduced to [public] and can be either a static field or a property
  - StateVector restricts changes to [public properties] instead of public static fields
  - Support for Transform transitions
    - Translate
    - Scale
    - Rotate
  - Support for Point transitions
  - CornerRadius transitions are supported
  - Supports Thickness transitions
  - The interface ILinearInterpolation allows custom types as transitionable properties
---

# StateMachine System
[![pAu2vOP.md.png](https://s21.ax1x.com/2024/09/15/pAu2vOP.md.png)](https://imgse.com/i/pAu2vOP)
## State
```csharp
   public static State MInsideState = State.FromObject(new Grid())
            .SetName("mouseinside")
            .SetProperty(x => x.Height, 300)
            .SetProperty(x => x.Width, 700)
            .SetProperty(x => x.Opacity, 0.2)
            .SetProperty(x => x.Background, Brushes.Lime)
            .ToState();
```
## StateVector ( ★ Recommended )
- Param1. Specifies the conditions that an object instance of a type should meet
- Param2. Automatically switches to the State of if the object meets the criteria
- Param3. When switching states, linearly animate related parameters
```csharp
   public static StateVector DefaultCondition = StateVector.FromType<Grid>()
            .AddCondition(x => x.Width>100, 
                 StateA, 
                 (x) => 
                 { 
                    x.Duration = 0.3; }
                 );
```
- Once the State and StateVector are defined, StateMachine is activated at initialization time
  - The State and StateVector must be Pulibc and Static
  - The ViewModel must implement the IConditionalTransfer< T > interface
  - The following example illustrates the technique of smoothly transitioning the opacity of the background panel when the mouse hovers over the control.
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

        public static StateVector<MButtonViewModel> ConditionA = StateVector<MButtonViewModel>.Create()
            .AddCondition(x => x.IsMouseInside, MouseIn, (x) => { x.Duration = 0.2; })
            .AddCondition(x => !x.IsMouseInside, Start, (x) => { x.Duration = 0.2; });

        public override bool IsMouseInside
        {
            get => base.IsMouseInside;
            set
            {
                base.IsMouseInside = value;
  
                OnConditionsChecked();
                //This is where you check to see if the current ViewModel satisfies the conditions contained in StateVector
            }
        }
    }
    ```

## StateMachine
- The FrameworkElement has an easier way to initiate transitions ( ★ Recommended )
```csharp
        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.StateMachineTransfer()
                .Add(x => x.Width, 700)
                .Add(x => x.Height, 300)
                .Add(x => x.Opacity, 0.2)
                .Add(x => x.Background, Brushes.Lime)
                .Set((x) =>
                {
                    x.Duration = 0.1;
                })
                .Start();
        }

        private void GD_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.StateMachineTransfer()
                .Add(x => x.Width, 70)
                .Add(x => x.Height, 30)
                .Add(x => x.Opacity, 1)
                .Add(x => x.Background, Brushes.Tomato)
                .Set((x) =>
                {
                    x.Duration = 0.1;
                })
                .Start();
        }
```
- Non-mvvm creation methods ( ⚠ Not Recommended )
```csharp
        public MainWindow()
        {
            InitializeComponent();

            State Start = State.FromObject(GD)
                .SetName("defualt")
                .ToState();

            GridMachine = StateMachine.Create(GD)
                .SetStates(Start, MInsideState)
                .SetConditions(DefaultCondition);
        }

        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GridMachine.Transfer("mouseinside",
                (x) =>
                {
                    x.Duration = 0.1;
                });
        }

        private void GD_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GridMachine.Transfer("defualt",
                (x) =>
                {
                    x.Duration = 0.1;
                });
        }
```

## TransferParams 
- Note that you will almost never define TransferParams separately; instead, they are passed in as Lambda expressions during State and StateVector setup,like this 
```csharp
(x) =>
{
    x.Duration = 0.1;
    x.Update = () =>
    {

    };
});
```

|Property|class|Default|Meaning|
|--------|-----|-------|-------|
|Duration|double|0|Animation duration ( unit: s )|
|Start|Action|null|Do something at the beginning|
|Update|Action|null|Do something at the beginning of each frame|
|LateUpdate|Action|null|Do something at the end of each frame|
|Completed|Action|null|Do something at the end|
|IsQueue|bool|false|When you apply for a transition operation, whether the state machine waits for the change currently in execution to complete|
|IsLast|bool|false|Whether to clear the queued transition operation when the currently requested transition operation is completed|
|IsUnique|bool|true|If the same operation as the transition operation applied for this time already exists in the queue, whether to continue to join the queue|
|FrameRate|int|400|The frame rate of linear transition determines the fluency and performance of the transition effect|
|WaitTime|double|0.008|You will rarely use it, and after a few versions it will be discarded as the system improves|

## StateViewModelBase
- The essence is an abstract base class that implements the INotifyPropertyChanged interface and the IConditionalTransfer< T > interface
- The [OnConditionsChecked()]() method is provided to determine if the condition to switch states has been met
```csharp
        public override bool IsMouseInside
        {
            get => base.IsMouseInside;
            set
            {
                base.IsMouseInside = value;
                OnConditionsChecked();
            }
        }
```
---
# WebServices
- ## GaoDe
  - Using
    ```csharp
    using MinimalisticWPF.GaoDeServices;
    ```
  - Get Key From https://console.amap.com/dev/key/app
    ```csharp
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GaoDeAPISet.Awake(Key: "Your_Api_Key", IsUpdateIP: true);
            //[ IsUpdateIP ] Describes whether to refresh local IP information upon activation
        }
    ```
  - IPService
    - Obtain IP information, mainly including province and city
      ```csharp
      var ip = await IPService.GetIP();
      MessageBox.Show(ip.GetCombined());
      ```
    - Gets the administrative district code by name
      ```csharp
      var adcode = await IPService.GetAdCode("都江堰");
      MessageBox.Show(adcode);
      ```
  - WeatherService
    - Automatically locate and get weather information
      ```csharp
      var weathers = await WeatherService.GetWeathers();
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - Query the weather by region name
      ```csharp
      var weathers = await WeatherService.GetWeathers("都江堰");
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - weather[0] represents today's weather
---
# MinimalisticUserControls
- ## ☆ Using
  - C#
    ```csharp
    using MinimalisticWPF;
    ```
  - XAML
    ```xml
    xmlns:mn="clr-namespace:MinimalisticWPF;assembly=MinimalisticWPF"
    ```
  - Change the Size by adjusting the Wise/Size property, and then add Margin to adjust the position
  - Using "#1e1e1e" as the background color might be more appropriate for these controls
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
- ## ☆ MPasswordBox
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLEoq.png)
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLQOJ.png)
  ## Property
  - WiseHeight
  - WiseWidth
  - FontSizeRatio
  - Password
  - Replace
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
- ### ☆ Notification
  [![pAKM40S.png](https://s21.ax1x.com/2024/09/18/pAKM40S.png)](https://imgse.com/i/pAKM40S)
  [![pAKMhm8.png](https://s21.ax1x.com/2024/09/18/pAKMhm8.png)](https://imgse.com/i/pAKMhm8)
  ## Method calls
  ```csharp
            if (Notification.Select("Are you sure you want to check the weather ?"))
            {
                Notification.Message("Weather");
            }
  ```
---
# ExtensionMethods
## string
- Value conversion
```csharp
   string valueA = "-123.7";
   string valueB = "TrUE";
   string valueC = "#1e1e1e";
   //Three values to be converted
   
   var result1 = valueA.ToInt();
   var result2 = valueA.ToDouble();
   var result3 = valueA.ToFloat();
   //Converting to numbers

   var result4 = valueB.ToBool();
   //Convert to bool

   var result5 = valueC.ToBrush();
   //Convert to Brush
```
- Fuzzy matching
```csharp
   string template = "abcdefg";
   //Assume that all other strings are similar to this string

   string sourceA = "abc";
   List<string> sourceB = new List<string>()
   {
       "abcdegf",
       "cbdgafe"
   };
   //These are assumed to be the sources to be matched

   var similarity1 = sourceA.LevenshteinDistance(template)
   //Calculate the shortest edit distance [int]

   var similarity2 = sourceA.JaroWinklerDistance(template)
   //Computing a similarity between 0 and 1 [double]

   var result3 = template.BestMatch(sourceB, 3);
   //Integer [3], meaning the shortest edit distance is less than [3]
   //Returns the index of sourceB with the smallest edit distance and less than [3] [int]

   var result4 = template.BestMatch(sourceB, 0.5);
   //Passing double precision [0.5] means that the approximation is higher than [0.5]]
   //Returns the index from sourceB with the highest approximation greater than [50%] [int]
```
- Folder generation operations
```csharp
   string folderNameA = "FF1";
   string folderNameB = "FF2";
   string folderNameC = "FF3";
   //The name of the folder

   var result1 = folderNameA.CreatFolder();
   //From the.exe location, create a folder named "FF1"

   var result2 = folderNameC.CreatFolder(folderNameA,folderNameB);
   //From the.exe location, create a nested folder "FF1/FF2/FF3"
```
- Xml and Json writes
```csharp
   string folderName = "Data";
   //Assume that the file ends up in a folder named folderName

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";
   //Assume the fileName is called filename

   var target = new Person();
   //Let's assume that target is an object that needs to be stored as a file

   var result1 = fileName1.CreatXmlFile(folderName.CreatFolder(), target);
   var result2 = fileName2.CreatJsonFile(folderName.CreatFolder(), target);
   //Objects are stored as.xml or.json files by providing [folder path + object] to the CreatFile() method
```
- Xml and Json deserialization
```csharp
   string folderName = "Data";
   //Assume that the file ends up in a folder named folderName

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";
   //Assume the fileName is called filename

   string AbsPathA = Path.Combine(folderName.CreatFolder(), $"{fileName1}.xml");
   string AbsPathB = Path.Combine(folderName.CreatFolder(), $"{fileName2}.json");
   var dataA = File.ReadAllText(AbsPathA);
   var dataB = File.ReadAllText(AbsPathB);
   //Assume that the serialized data dataA and dataB have been read

   var result1 = dataA.XmlParse<Person>();
   var result2 = dataB.JsonParse<Person>();
   //xml and json, respectively, to deserialize into a Person instance
```
- Regular operation
```csharp
   string sourceA = "[1]wkhdkjhk[a][F3]https:awijdioj.mp3fwafw";
   string sourceB = "awdhttps://aiowdjoajfo.comawd&*(&d)*dhttps://tn.comdawd";
   //Let's say we scrape two pieces of html
   
   var resultA = sourceA.CaptureBetween("https:", ".mp3");
   //Captures the string contained in the middle of the feature character
   //Output[ awijdioj ]

   var resultB = sourceB.CaptureLike("https://", "com");
   //Captures strings that conform to an ordered set of features
   //Output[ https://aiowdjoajfo.com ] [ https://tn.com ]
```
- Password strength
```csharp
   string password = "12345678";
   int Level = password.CheckPasswordStrength(MinLength=8);
   //An integer between 0 and 4 is returned to indicate the strength of the password
```

---
---
---
---
---
---

# 中文文档

## 目标
- [状态机系统](#StateMachine)
    - [State](#State) 描述一个对象在某一状态时的部分属性及其具体值
    - [StateVector](#StateVector) 描述一个对象在达成何种条件时自动切换到何种状态
    - [StateMachine](#StateMachine) 实现属性值的线性过渡
    - [StateViewModelBase< T >](#StateViewModelBase) 描述实现IConditionalTransfer接口以支持StateVector条件切换的、最小规格的ViewModel
    - [TransferParams](#TransferParams) 描述状态机如何加载此次线性过渡
    - 当前受支持的属性类型
      - double
      - Brush
      - Transform
      - Point
      - CornerRadius
      - Thickness
      - ILinearInterpolation
- [Web服务](#WebServices)
  - [ 高德 ] WebApi
    - IP服务
    - 天气服务
- [用户控件](#MinimalisticUserControls)
    - 统一的深色主题
    - 所有动画效果都基于状态机系统
- [扩展方法](#ExtensionMethods)
    - [string](#string)
      - 值转换
      - 模糊匹配
      - 分析工具（例如提取html中的资源地址）
      - 密码强度
	- FrameworkElement
      - 基于状态机的动画创建
## 支持框架
- [.NET6.0-windows] 
- [.NET8.0-windows]
## 获取
- [github][1]
- [nuget][2]

[1]: https://github.com/ChengduNeusoftUniversity-FengJunjie-Y22/MinimalisticWPF
[2]: https://www.nuget.org/packages/MinimalisticWPF/

## Change
- V1.5.0
  - StateMachineTransfer() 改为支持任何 where T : class ,new()
  - State限制降低为[公开]，可以是静态字段，也可以是属性
  - StateVector限制修改为[公开属性]，而不再是公开静态字段
  - 支持Transform过渡
    - Translate
    - Scale
    - Rotate
  - 支持Point过渡
  - 支持CornerRadius过渡
  - 支持Thickness过渡
  - 接口ILinearInterpolation允许自定义的类型作为可过渡属性
---

# 状态机系统
[![pAu2vOP.md.png](https://s21.ax1x.com/2024/09/15/pAu2vOP.md.png)](https://imgse.com/i/pAu2vOP)
## State
```csharp
   public static State MInsideState = State.FromObject(new Grid())
            .SetName("mouseinside")
            .SetProperty(x => x.Height, 300)
            .SetProperty(x => x.Width, 700)
            .SetProperty(x => x.Opacity, 0.2)
            .SetProperty(x => x.Background, Brushes.Lime)
            .ToState();
```
## StateVector ( ★ 推荐 )
- 参数1.描述这个对象应当满足的条件
- 参数2.描述当条件满足时,需要切换到哪个状态
- 参数3.设置一些过渡效果的细节
```csharp
   public static StateVector DefaultCondition = StateVector.FromType<Grid>()
            .AddCondition(x => x.Width>100, 
                 StateA, 
                 (x) => 
                 { 
                    x.Duration = 0.3; }
                 );
```
- 定义好State与StateVector后,在控件初始化时加载状态机即可
  - State 和 StateVector 应当是 public static 的
  - ViewModel 应当实现 IConditionalTransfer接口
  - 下述示例演示了如何在鼠标进入控件时,令其背景透明度过渡为0.2
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

        public static StateVector<MButtonViewModel> ConditionA = StateVector<MButtonViewModel>.Create()
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

## StateMachine
- FrameworkElement 有更简洁的方法加载状态机支持的动画化 ( ★ 推荐 )
```csharp
        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.StateMachineTransfer()
                .Add(x => x.Width, 700)
                .Add(x => x.Height, 300)
                .Add(x => x.Opacity, 0.2)
                .Add(x => x.Background, Brushes.Lime)
                .Set((x) =>
                {
                    x.Duration = 0.1;
                })
                .Start();
        }

        private void GD_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.StateMachineTransfer()
                .Add(x => x.Width, 70)
                .Add(x => x.Height, 30)
                .Add(x => x.Opacity, 1)
                .Add(x => x.Background, Brushes.Tomato)
                .Set((x) =>
                {
                    x.Duration = 0.1;
                })
                .Start();
        }
```
- 最原始的状态机用法 ( ⚠ 不推荐 )
```csharp
        public MainWindow()
        {
            InitializeComponent();

            State Start = State.FromObject(GD)
                .SetName("defualt")
                .ToState();

            GridMachine = StateMachine.Create(GD)
                .SetStates(Start, MInsideState)
                .SetConditions(DefaultCondition);
        }

        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GridMachine.Transfer("mouseinside",
                (x) =>
                {
                    x.Duration = 0.1;
                });
        }

        private void GD_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GridMachine.Transfer("defualt",
                (x) =>
                {
                    x.Duration = 0.1;
                });
        }
```

## TransferParams 
- 基本不会单独用到这个类型,通常只出现在Set（）内部,且以Lambda书写
```csharp
(x) =>
{
    x.Duration = 0.1;
    x.Update = () =>
    {

    };
});
```

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

## StateViewModelBase
- 一个抽象基类,它是MVVM模式下,使ViewModel支持StateVector条件切换的最小实现单元,算是一种模板
- 它提供[OnConditionsChecked()]()方法,使得您可在属性值变动时,检查当前ViewModel是否满足StateVector中设置的条件
```csharp
        public override bool IsMouseInside
        {
            get => base.IsMouseInside;
            set
            {
                base.IsMouseInside = value;
                OnConditionsChecked();
            }
        }
```
---
# WebServices
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
# MinimalisticUserControls
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
- ## ☆ MPasswordBox
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLEoq.png)
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLQOJ.png)
  ## Property
  - WiseHeight
  - WiseWidth
  - FontSizeRatio
  - Password
  - Replace
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
# ExtensionMethods
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
