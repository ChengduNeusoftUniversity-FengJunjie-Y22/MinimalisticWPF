# MinimalisticWPF
## Target
- [StateMachine System](#StateMachine)
    - Use a [State](#State) object to keep track of the control's property values at the current time
    - Use a [StateVector](#StateVector) object to describe the conditions under which the StateMachine transitions to which state
    - Use a [StateMachine](#StateMachine) object and give it State objects and StateVector objects to implement linear animations
    - Provides the [StateViewModelBase< T >](#StateViewModelBase) base class, which helps you quickly build ViewModels powered by state machines
    - Use a [TransferParams](#TransferParams) object to describe the details of a linear transition animation.It usually appears as a Lambda in State, StateVector, or StateMachine.Transfer()
    - Properties that currently support linear transitions
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
	- [FrameworkElement](#FrameworkElement)
      - Linear animation based on StateMachine
      - Storyboard-based animations
## Framework
- [.NET6.0-windows] 
- [.NET8.0-windows]
## Get
- [github][1]
- [nuget][2]

[1]: https://github.com/ChengduNeusoftUniversity-FengJunjie-Y22/MinimalisticWPF
[2]: https://www.nuget.org/packages/MinimalisticWPF/

# StateMachine System
## State
Suppose the current state of the Grid control is A, when the mouse enters the control, I want its length, width, height and background color to have a linear gradient effect, and the final result of the gradient is recorded as state B, then the state at the time of B can be recorded as this code
```csharp
        static State MInsideState = State.FromObject(new Grid())
            .SetName("mouseinside")
            .SetProperty(x => x.Height, 300)
            .SetProperty(x => x.Width, 700)
            .SetProperty(x => x.Opacity, 0.2)
            .SetProperty(x => x.Background, Brushes.Lime)
            .ToState();
```
## StateVector
- If you are using the MVVM design pattern and want the control to automatically switch to a specific State when a certain condition is met, you need to create a StateVector object to record this relationship, as shown in the following code.
- Of course, here we're using Grid for the sake of demonstration, but you should actually fill in the specific ViewModel type
- [Example in MVVM design pattern](#MVVM)
```csharp
        static StateVector DefaultCondition = StateVector.FromType<Grid>()
            .SetTarget(MInsideState)
            .SetName("LimitHeight")
            .SetCondition(x => x.Width > 260 || x.Height >= 500)
            .SetTransferParams(transitionTime: 0.3, frameRate: 244)
            .ToStateVector();
```
## StateMachine
- Using the State and StateVector objects, you can create a StateMachine instance and load the linear animation in the MouseEnter and MouseLeave events of a Grid control called GD
- The FrameworkElement has a more elegant way to quickly start the linear transitions StateMachine provides, and in fact, it's even better for non-MVVM design patterns, but note that StateMachine's linear transitions don't depend on storyboards at all. Therefore, when you mix the two, you need to avoid conflicts
- [Example About FrameworkElement](#FrameworkElement)
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
            GridMachine.Transfer("mouseinside",
                (x) =>
                {
                    x.Duration = 0.1;
                });
        }
```
## TransferParams 
Note that you will almost never define TransferParams separately; instead, they are passed in as Lambda expressions during State and StateVector setup

|Property|class|Default|Meaning|
|--------|-----|-------|-------|
|Duration|double|0|Animation duration ( unit: s )|
|IsQueue|bool|false|When you apply for a transition operation, whether the state machine waits for the change currently in execution to complete|
|IsLast|bool|false|Whether to clear the queued transition operation when the currently requested transition operation is completed|
|IsUnique|bool|true|If the same operation as the transition operation applied for this time already exists in the queue, whether to continue to join the queue|
|FrameRate|int|400|The frame rate of linear transition determines the fluency and performance of the transition effect|
|ProtectNames|ICollection< string >?|null|The name of the property in this transition that is not affected by the state machine|
|WaitTime|double|0.008|You will rarely use it, and after a few versions it will be discarded as the system improves|

## StateViewModelBase
- The essence is an abstract base class that implements the INotifyPropertyChanged interface and the IConditionalTransfer< T > interface
- The [OnConditionsChecked()]() method is provided to determine if the condition to switch states has been met

## MVVM
Take the [MPasswordBox](#MPasswordBox) control provided by the class library as an example
- [PasswordStrengthColor]() is bound to [BorderBrush]() to indicate password strength with color
```xml
<UserControl x:Class="MinimalisticWPF.MPasswordBox"
    
    <UserControl.DataContext>
        <local:MPasswordBoxViewModel x:Name="ViewModel"/>
    </UserControl.DataContext>

    <Grid Background="{Binding FixedTransparent,Mode=TwoWay}"
          Width="{Binding ElementName=Total,Path=Width}"
          Height="{Binding ElementName=Total,Path=Height}">
        <Border x:Name="FixedBorder"
                Background="{Binding FixedTransparent}"
                CornerRadius="{Binding CornerRadius}"
                BorderThickness="2"
                BorderBrush="{Binding PasswordStrengthColor}"
                ClipToBounds="True"/>
        <Border ClipToBounds="True">
            <TextBox x:Name="TruePWD"
                 Width="{Binding ElementName=Total,Path=Width}"
                 Height="{Binding ElementName=Total,Path=Height}"
                 Foreground="{Binding FixedTransparent}"
                 Background="{Binding FixedTransparent}"
                 TextChanged="TruePWD_TextChanged"
                 Grid.ZIndex="2"
                 BorderThickness="0"
                 CaretBrush="Transparent"
                 FontSize="0.01"
                 ContextMenuOpening="TruePWD_ContextMenuOpening"/>
        </Border>
    </Grid>
</UserControl>
```
- The [StateMachine]() is loaded at initialization time
- [TextChanged]() event internally passes the latest password value
```csharp
    public partial class MPasswordBox : UserControl
    {
        public MPasswordBox()
        {
            InitializeComponent();
            this.StateMachineLoading(ViewModel);
        }

        private void TruePWD_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.TruePassword = TruePWD.Text;
        }

        private void TruePWD_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }
    }
```
- Now we can [automatically togple the border color using a preset State and StateVector]()
- Note that the State and StateVector must be [Public Static]()
- [CheckPasswordStrength(8)](#string) means that the minimum password length is 8, and anything less than that is considered a strength level of 0
```csharp
    public class MPasswordBoxViewModel : StateViewModelBase<MPasswordBoxViewModel>
    {
        public MPasswordBoxViewModel() { }

        public MPasswordBoxModel Model { get; set; } = new MPasswordBoxModel();

        public static State Default = State.FromObject(new MPasswordBoxViewModel())
            .SetName("default")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.White)
            .ToState();

        public static State Level1 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L1")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Tomato)
            .ToState();
        public static State Level2 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L2")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Yellow)
            .ToState();
        public static State Level3 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L3")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Cyan)
            .ToState();
        public static State Level4 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L4")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Lime)
            .ToState();

        public static StateVector<MPasswordBoxViewModel> ConditionA = StateVector<MPasswordBoxViewModel>.Create(new MPasswordBoxViewModel())
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 0, Default, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 1, Level1, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 2, Level2, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 3, Level3, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 4, Level4, (x) => { x.Duration = 0.1; });

        public string TruePassword
        {
            get => Model.TruePassword;
            set
            {
                Model.TruePassword = value;
                string result = string.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    result += ReplacingCharacters;
                }
                UIPassword = result;
                OnPropertyChanged(nameof(TruePassword));
                OnConditionsChecked();
            }
        }
    }
```
### Tips 
- Of course, this is just a simple exercise. Because the state machine is [type-specific](), it can in theory do more complex things than just animating. 
- The [IConditionalTransfer]() interface is the key component that allows the StateMachine to interact with the StateVector

# WebServices
- ## Using
  - C#
    ```csharp
    using MinimalisticWPF.GaoDeServices;
    ```
- ## GaoDe
  - Disposition
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
- ## ☆ MButton
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLPyQ.png)
  ## Property
  - Click
  - Text
  - TextBrush
  - FontSizeRatio
  - EdgeBrush
  - EdgeThickness
  - HoverBrush
  - CornerRadius
- ## ☆ MPasswordBox
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLEoq.png)
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLQOJ.png)
  ## Property
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
## FrameworkElement
The same Grid widget called GD, now let's add a linear transition animation to it. When the mouse enters the Grid, the Grid expands and changes color, and when the mouse leaves, it returns to its initial state
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

