# MinimalisticWPF
- [English](#English)
- [中文](#中文文档)
---

# 中文文档

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

## Change
- V1.5.0
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
---

# 状态机系统
- State & StateVecotr 为MVVM而生 ,可通过预先设置条件实现状态的自动切换
- StateMachine 理论可对任何类型的属性做出线性过渡，非MVVM下，几乎所有类型都包含一个用于快速为对象实例创建线性过渡的扩展方法，这可能是该类库最常用的方法
- ★ 优势
  - 以少量代码创建复杂过渡效果
  - 像游戏开发引擎那样,可在Update()中决定过渡效果的每一帧你要做出的行为
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

        public StateVector<MButtonViewModel> ConditionA = StateVector<MButtonViewModel>.Create()
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
