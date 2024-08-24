# MinimalisticWPF
## Target
- [StateMachine System](#StateMachine)
    - Use a [State](##State) object to keep track of the control's property values at the current time
    - Use a [StateVector](##StateVector) object to describe the conditions under which the StateMachine transitions to which state
    - Use a [StateMachine](##StateMachine) object and give it State objects and StateVector objects to implement linear animations
- [Minimalistic UserControls](#MinimalisticUserControls)
    - Uniform dark theme
    - All animations based on StateMachine
- [Extension Method](#ExtensionMethod)
    - [string](##string)
      - Value conversion
      - Fuzzy matching
      - Crawler analysis
	- [FrameworkElement](##FrameworkElement)
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
If you are using the MVVM design pattern and want the control to automatically switch to a specific State when a certain condition is met, you need to create a StateVector object to record this relationship, as shown in the following code
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
## Tips
The [FrameworkElement](##FrameworkElement) has a more elegant way to quickly start the linear transitions StateMachine provides, and in fact, it's even better for non-MVVM design patterns, but note that StateMachine's linear transitions don't depend on storyboards at all. Therefore, when you mix the two, you need to avoid conflicts


# MinimalisticUserControls
## …… Under development

# ExtensionMethod
## string
## FrameworkElement
Use the extension method to quickly start the StateMachine linear transition animation
```cshapr
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

