# MinimalisticWPF
## Target
- [StateMachine System](#StateMachine)
    - Use a [State](#State) object to keep track of the control's property values at the current time
    - Use a [StateVector](#StateVector) object to describe the conditions under which the StateMachine transitions to which state
    - Use a [StateMachine](#StateMachine) object and give it State objects and StateVector objects to implement linear animations
    - Properties that currently support linear transitions
      - double
      - Brush
      - …… under development >>
- [Minimalistic UserControls](#MinimalisticUserControls)
    - Uniform dark theme
    - All animations based on StateMachine
- [Extension Method](#ExtensionMethod)
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
```csharp
        static StateVector DefaultCondition = StateVector.FromType<Grid>()
            .SetTarget(MInsideState)
            .SetName("LimitHeight")
            .SetCondition(x => x.Width > 260 || x.Height >= 500)
            .SetTransferParams(transitionTime: 0.3, frameRate: 244)
            .ToStateVector();
```
## StateMachine
Using the State and StateVector objects, you can create a StateMachine instance and load the linear animation in the MouseEnter and MouseLeave events of a Grid control called GD
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
## Example Under MVVM
…… under development >> The automatic switching function provided by StateVector has not passed the phase test
## Example Under FrameworkElement 
The [FrameworkElement](#FrameworkElement) has a more elegant way to quickly start the linear transitions StateMachine provides, and in fact, it's even better for non-MVVM design patterns, but note that StateMachine's linear transitions don't depend on storyboards at all. Therefore, when you mix the two, you need to avoid conflicts


# MinimalisticUserControls
…… under development >> The authors are working on using StateMachine entirely for linear animations in MVVM design pattern

# ExtensionMethod
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
## FrameworkElement
The same Grid widget called GD, now let's add a linear transition animation to it. When the mouse enters the Grid, the Grid expands and changes color, and when the mouse leaves, it returns to its initial state
```csharp
        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.MachineTransfer()
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
            GD.MachineTransfer()
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

