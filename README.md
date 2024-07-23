# MinimalisticWPF
### A toolkit designed to quickly build WPF projects. 
### Some functionality that may need to be reused a lot is encapsulated.
### Provides a few minimalistic user controls, and you can adjust their appearance and behavior to some extent by setting properties.
### ☆ You can view the documentation by selecting the following path （ Currently only Chinese documents are available ）
- [github][1]
- [gitee][2]

[1]: https://github.com/ChengduNeusoftUniversity-FengJunjie-Y22/MinimalisticWPF
[2]: https://gitee.com/CNU-FJj-Y22/MinimalisticWPF

## 更新进度
<details>
<summary>Version 1.0.5 (测试)</summary>

#### 一次中等程度的重构，删除不必要的接口、类型，优化个别属性的命名
#### 折线图初步完成
###### 注意这仍然是测试版本，距离正式版预计还有20天

</details>

## 更新预告
## ★ 基于 MVVM 模式，对所有控件相关部分进行重构
## ★ 完成 FloderManager
## ★ 完成 TempManager
## ★ 拓展可用控件、静态工具等

## 命名空间
### 1.[ MinimalisticWPF ] 后端支持
```csharp
using MinimalisticWPF;
```
### 2.[ MinimalisticWPF.XControls ] 前端支持 - 用户控件
```xaml
xmlns:xctr="clr-namespace:MinimalisticWPF.XControls;assembly=MinimalisticWPF"
```
### 3.[ MinimalisticWPF.XConvertors ] 前端支持 - 转换器
```xaml
xmlns:xcvt="clr-namespace:MinimalisticWPF.XConvertors;assembly=MinimalisticWPF"
```

## Ⅰ 用户控件 [ XControls ]
##### 需要 [ 重新生成解决方案 ] 才可以正确预览它们的效果
### 假定你已经用这句代码引入了库
```xaml
xmlns:xc="clr-namespace:MinimalisticWPF.XControls;assembly=MinimalisticWPF"
```
### 于是你可以像这样来获取一个用户控件
```xaml
<xc:XControls/>
```
### 具体提供以下控件
<details>
<summary>（1）XButton</summary>

### 简约风格 Button，有一套自己的动画系统，并且允许你通过设置属性值修改它的行为
-----------------------------------------------------------------------------------
#### Text - 文本内容
#### TextColor - 文本颜色
#### Height - ☆文本大小与控件高度之间会自动适配
#### XBackground - 按钮背景色（Background是用户控件的背景色，不是实际控件的背景色）
#### CornerRadius - 边框的圆滑度
#### Click - 点击事件
-----------------------------------------------------------------------------------
#### IsUseClickAnimator - 是否启用鼠标点击时的填充动画
#### ClickBackground - 鼠标点击填充动画颜色
#### ClickAnimationTime - 鼠标点击填充动画持续时间   
-----------------------------------------------------------------------------------
#### IsUseBorderAnimation - 是否启用鼠标进入时的边框动画
#### BorderAnimationSide - 鼠标进入按钮时，哪些边需要启用动画效果
#### BorderAnimationTime - 边框动画的持续时间
#### BorderAnimationColor - 边框动画颜色 
-----------------------------------------------------------------------------------
#### IsUseHoverTextColor - 是否启用鼠标悬停时的文本变色效果
#### HoverTextColor - 鼠标悬停时，文本变为指定的颜色
#### IsUseHoverBackground - 是否启用鼠标悬停时的背景变色效果
#### HoverBackground - 鼠标悬停时，背景色变为指定的值
#### HoverAnimationTime - 悬停动画的持续时间，本质是在控制背景色的透明度
-----------------------------------------------------------------------------------
#### IsUseFixedBorder - 是否使用固定边框
#### FixedBorderThickness - 固定边框的边框厚度
#### FixedBorderBrush - 固定边框的边框颜色
-----------------------------------------------------------------------------------
</details>
<details>
<summary>（2）XProgressBar</summary>

### 简约风格 ProgressBar，一种环形进度条，无动画系统

-----------------------------------------------------------------------------------
#### Value - 表示实际百分比，用于控制进度
#### RingBackground - 圆环背景色
#### RingForeground = 圆环进度填充色
-----------------------------------------------------------------------------------

</details>
<details>
<summary>（3）XTopBar</summary>

### 程序顶端栏，负责程序名称的显示、窗体隐藏、缩放、关闭，以及窗体的拖动功能
##### 建议在使用它时,先设置好Height属性
-----------------------------------------------------------------------------------
#### XBackground - 控件背景色
#### Height - ☆控件高度会决定此控件内所有文本的大小
#### Title - 程序标题
#### TitleColor - 标题颜色
#### CornerRadius - 边框圆滑度
-----------------------------------------------------------------------------------
#### ControlTextColor - 右侧窗体控制按钮的文本颜色
#### ControlHoverTextColor - 右侧控制按钮在鼠标悬浮时的文本颜色
-----------------------------------------------------------------------------------
#### FixedBorderThickness - 控件最外围的边框厚度
#### FixedBorderBrush - 控件最外围边框的颜色
-----------------------------------------------------------------------------------
#### IsUseLineAnimation - 鼠标进入控件时，是否启用下方的动态线条
#### LineAnimationColor - 动态线条的颜色
-----------------------------------------------------------------------------------
</details>

<details>
<summary>（4）XHistogram</summary>

### 直方图
-----------------------------------------------------------------------------------
#### Data - [ Histogram 实例 ] 表示实际数据,设置该属性将重新加载直方图
-----------------------------------------------------------------------------------
#### AxisColor - 轴线条的颜色
#### LineColor - 表内分割线的颜色
#### LineThickness - 表内分割线的厚度
-----------------------------------------------------------------------------------
#### DataOpacity - 数据涂层的透明度
#### DataColors - 集合,默认存放三个颜色Cyan，Red，Lime，表示X轴单个节点多个元素时，每个元素的数据涂层的颜色，若单个节点的数据对象超出集合元素数，则用默认的Cyan，你可以手动修改这个集合
-----------------------------------------------------------------------------------
#### 代码示例
##### 前端定义一个直方图
```XAML
<xc:XHistogram x:Name="MyHistogram" Width="500" Background="#1e1e1e"/>
```
##### 后端为直方图设置数据
```C#
        private void TestForHistogram()
        {
            //第一个X轴节点元素对象
            HistogramXElement histogramXElement1 = new HistogramXElement();

            //这是X轴节点的名字
            histogramXElement1.NodeName = "2024/07/01";
            
            //这是此节点存放的数据组，放几个数据，这个节点就生成几个数据条
            histogramXElement1.Value.Add(10);
            histogramXElement1.Value.Add(20);
            histogramXElement1.Value.Add(30);
            
            //其余三个X轴节点元素的定义
            HistogramXElement histogramXElement2 = new HistogramXElement();
            histogramXElement2.NodeName = "2024/07/02";
            histogramXElement2.Value.Add(40);
            histogramXElement2.Value.Add(50);
            histogramXElement2.Value.Add(60);
            HistogramXElement histogramXElement3 = new HistogramXElement();
            histogramXElement3.NodeName = "2024/07/03";
            histogramXElement3.Value.Add(70);
            histogramXElement3.Value.Add(80);
            HistogramXElement histogramXElement4 = new HistogramXElement();
            histogramXElement4.NodeName = "2024/07/04";
            histogramXElement4.Value.Add(90);
            histogramXElement4.Value.Add(100);

            //行列表单结构定义
            Form2D histogram = new Form2D();

            //Y轴最大值（最小值默认为0，且直方图中无需为X轴设置）
            histogram.EndValue_Y = 100;

            //Y轴节点数
            histogram.Nodes_Y = 11;

            //X轴节点数
            histogram.Nodes_X = 4;

            //数据单位
            histogram.Units = "kw·h";

            //为表单填入数据（不同类型的表填入对象的类型也不同，这里填入 HistogramXElement实例）
            histogram.HistogramData.Add(histogramXElement1);
            histogram.HistogramData.Add(histogramXElement2);
            histogram.HistogramData.Add(histogramXElement3);
            histogram.HistogramData.Add(histogramXElement4);

            //完成数据的传入
            MyHistogram.Data = histogram;
        }
```
----------------------------------------------------------------------------------
</details>

<details>
<summary>（5）XLineChart</summary>

### 折线图
----------------------------------------------------------------------------------
#### Data - [ LineChart 实例 ] 表示实际数据,设置该属性将重新加载折线图
----------------------------------------------------------------------------------
#### AxisColor - 轴线条的颜色
#### LineColor - 表内分割线的颜色
#### LineThickness - 表内分割线的厚度
#### DataLineThickness - 折线线条的颜色
----------------------------------------------------------------------------------
### 代码示例
#### 前端定义一个折线图
```xaml
<xc:XLineChart x:Name="MyLineChart" Width="300" Height="150" Margin="10,240,490,60"/>
```
#### 后端为折线图设置数据
```csharp
        private void TestForLineChart()
        {
            //四个数据元素
            LineChartXElement linechartXElement1 = new LineChartXElement();
            linechartXElement1.NodeName = "07/01";
            linechartXElement1.Value.Add(10);
            linechartXElement1.Value.Add(60);
            LineChartXElement linechartXElement2 = new LineChartXElement();
            linechartXElement2.NodeName = "07/02";
            linechartXElement2.Value.Add(90);
            linechartXElement2.Value.Add(40);
            LineChartXElement linechartXElement3 = new LineChartXElement();
            linechartXElement3.NodeName = "07/03";
            linechartXElement3.Value.Add(50);
            linechartXElement3.Value.Add(50);
            LineChartXElement linechartXElement4 = new LineChartXElement();
            linechartXElement4.NodeName = "07/04";
            linechartXElement4.Value.Add(50);
            linechartXElement4.Value.Add(20);
            LineChartXElement linechartXElement5 = new LineChartXElement();
            linechartXElement5.NodeName = "07/05";
            linechartXElement5.Value.Add(90);
            linechartXElement5.Value.Add(10);
            LineChartXElement linechartXElement6 = new LineChartXElement();
            linechartXElement6.NodeName = "07/06";
            linechartXElement6.Value.Add(20);
            linechartXElement6.Value.Add(80);

            //折线图表单结构
            LineChart linechart = new LineChart();

            linechart.Maximum_Y = 100;
            linechart.Maximum_X = 100;

            linechart.Minimum_Y = 0;
            linechart.Minimum_X = 0;

            linechart.AxisTicks_Y = 12;
            linechart.AxisTicks_X = 6;

            linechart.Title_Y = "用电量";
            linechart.Title_X = "日期";

            linechart.Unit_Y = "KW·H";
            linechart.Unit_X = "Time";

            linechart.DataGroup.Add(linechartXElement1);
            linechart.DataGroup.Add(linechartXElement2);
            linechart.DataGroup.Add(linechartXElement3);
            linechart.DataGroup.Add(linechartXElement4);
            linechart.DataGroup.Add(linechartXElement5);
            linechart.DataGroup.Add(linechartXElement6);

            MyLineChart.Data = linechart;
        }
```
----------------------------------------------------------------------------------

</details>


## Ⅱ 静态工具
<details>
<summary>（1）FileTool</summary>

### 有关文件操作的封装，例如Xml与Json的读写.
-----------------------------------------------------------------------------------
### 示例1. 对象的序列化与反序列化
```csharp
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Test(object sender, RoutedEventArgs e)
        {
            Data data = new Data();
            //假定 data 是你需要序列化的对象

            var xmldata = FileTool.SerializeObject(data);
            //调用 FileTool.SerializeObject方法，将实例对象 data 序列化
            //具体的序列化模式取决于 data.SerializeMode值，这里则是 Xml
            //将返回一个元组，Item1表示是否成功序列化（bool）；Item2表示序列化的结果（string）

            if (xmldata.Item1) MessageBox.Show(xmldata.Item2);
            //如果序列化成功 Item1==true ，则返回序列化的结果 Item2

            var result = FileTool.DeSerializeObject<Data>(xmldata.Item2, SerializeModes.Xml);
            //调用 FileTool.DeSerializeObject方法，将刚才序列化的结果 xmldata.Item2 反序列化
            //这里默认 xmldata.Item2不为空，实际使用时建议做出判断
            //确保你选择的反序列化模式与序列化模式是一致的

            if (result.Item1) MessageBox.Show(result.Item2.Items[0]);
            //如果反序列化成功，尝试获取集合 Items的首个元素，这里结果应该为 “A”
        }
    }
    public class Data : ISerializableObject
    {
        public Data() { }

        public SerializeModes SerializeMode { get; set; } = SerializeModes.Xml;
        public string AbsolutePath { get; set; } = "Default";
        //这是实现 ISerializableObject接口 的要求，序列化相关操作必须先实现此接口
        //SerializeMode（序列化格式，Xml或Json）
        //AbsolutePath（绝对路径，表示对象文件存储的路径）

        public List<string> Items { get; set; } = new List<string>()
        {
            "A",
            "B",
            "C"
        };
    }
```
-----------------------------------------------------------------------------------------------
### 示例2. 以文件的形式（.xml或.json）存储、读取对象
```csharp
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Test(object sender, RoutedEventArgs e)
        {
            Data data = new Data();

            data.AbsolutePath = FileTool.GenerateFilePath(null, "testdata", data.SerializeMode);
            //使用 FileTool.GenerateFilePath 方法，生成存储data的绝对路径
            //首个参数表示文件夹路径，若它为 null ，则会根据程序 .exe所在位置，加上第二个和第三个参数，生成一个绝对路径

            var result = FileTool.SaveObjectAsFile<Data>(data);
            //使用 FileTool.SaveObjectAsFile<T> 方法，存储data

            var result2 = FileTool.ReadObjectFromFile<Data>(data.AbsolutePath, data.SerializeMode);
            //使用  FileTool.ReadObjectFromFile<T> 方法，读取data

            MessageBox.Show(result2.Item2.Items[2]);
            //最终应显示 “C”
        }
    }
    public class Data : ISerializableObject
    {    
        public SerializeModes SerializeMode { get; set; } = SerializeModes.Xml;
        public string AbsolutePath { get; set; } = "Default";

        public Data() { }
        public List<string> Items { get; set; } = new List<string>()
        {
            "A",
            "B",
            "C"
        };
    }
```

</details>
<details>
<summary>（2）DataTool</summary>

### 有关数据处理操作的封装，例如捕获位于 [ ] 内的字符串.
|函数|参数|返回值|说明|
|---------------------|-----------------------------------------------|----------------------------------|---------------------------------------------------------------|
|StringToInt|string|int?|将string转为int|
|StringToBool|string|bool?|将string转为bool,注意是不区分大小写的|
|KeepDecimal|double,int|double|保留小数|
|CaptureByQuotationMarks|string|List< string > |捕获位于 《 》 内的字符串|
|CaptureByAngleBrackets|string|List< string > |捕获位于 < > 内的字符串|
|CaptureByCurlyBraces|string|List< string > |捕获位于 { } 内的字符串|
|CaptureBySquareBrackets|string|List< string > |捕获位于 [ ] 内的字符串|
|CaptureByParenthesis|string|List< string > |捕获位于 ( ) 内的字符串|

</details>
<details>
<summary>（3）AnimationTool</summary>

### 为[ FrameworkElement ]提供了一些简易的动画效果

</details>

## Ⅲ 表单相关
<details>
<summary>（1）FormBase</summary>

### [abstract] - [ISerializableObject接口] - 所有XY结构表格的抽象基类
|属性|说明|
|---------------------|-------------------------------------------------------|
|Unit_X|X单位|
|Unit_Y|Y单位|
|Title_X|X标题|
|Title_Y|Y标题|
|Minimum_X|X轴最小值|
|Minimum_Y|Y轴最小值|
|Maximum_X|X轴最大值|
|Maximum_Y|Y轴最大值|
|AxisTicks_X|X轴刻度数量|
|AxisTicks_Y|Y轴刻度数量|

</details>
<details>
<summary>（2）Histogram & HistogramXElement</summary>

### Histogram - 直方图结构
|属性|说明|
|---------------------|-------------------------------------------------------|
|DataGroup|HistogramXElement对象的集合，表示X轴每个节点的数据|
|IsEnable|检查当前数据是否可用|

### HistogramXElement - 节点数据
|属性|说明|
|---------------------|-------------------------------------------------------|
|NodeName|节点名称|
|Value|节点值,是个double集合|

</details>

<details>
<summary>（3）LineChart & LineChartXElement</summary>

### LineChart - 折线图结构
|属性|说明|
|---------------------|-------------------------------------------------------|
|DataGroup|LineChartXElement对象的集合，表示X轴每个节点的数据|
|PointGroups|PointCollection对象的集合，表示折线点的坐标|
|IsEnable|检查当前数据是否可用|

### LineChartXElement - 节点数据元素
|属性|说明|
|---------------------|-------------------------------------------------------|
|NodeName|节点名称|
|XIndex|在X轴的索引|
|Value|节点值,是个double集合|
|Point|每个节点值在图中的坐标点|
|Bottom|每个坐标点的最下方的坐标|

</details>

