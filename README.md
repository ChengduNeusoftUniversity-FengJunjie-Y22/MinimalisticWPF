# MinimalisticWPF
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
      - …… under development >>
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
- V1.4.3
  - The lifecycle of StateMachine already allows the consumer to enrich the behavior by setting the Start, Update, LateUpdate, Completed delegates in [TransferParams](#TransferParams) 
  - Added a type named [Notification](#Notification) that allows you to expand a Message box by calling its Message() or Select() methods
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

