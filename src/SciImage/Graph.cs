using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace SciImage
{
    public partial class Graph : Form
    {
        public Graph()
        {
            InitializeComponent();
        }
        private delegate void SetDataDel(int[,] Data, string DataName);
        public void SetData(int[,] Data, string DataName)
        {
            if (this.InvokeRequired == true)
            {
                try
                {
                    object[] pars = { Data, DataName };
                    this.Invoke(new SetDataDel(SetData), pars);
                }
                catch { }
            }
            else 
                PlotData(zedGraphControl1, Data, DataName);
        }
        public void PlotData(ZedGraph.ZedGraphControl zGraph, int[,] Data, string DataName)
        {
            //zedGraphControl1.MasterPane = new MasterPane();
            GraphPane myPane = zGraph.GraphPane;
            myPane.CurveList.Clear();
            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Pixels";
            myPane.YAxis.Title.Text = DataName;
           // myPane.XAxis.Type = AxisType.Date;
            // Make up some data points based on the Sine function
            PointPairList list = new PointPairList();

            for (int i = 0;i< Data.GetLength(1); i++)
            {
                list.Add(new PointPair(Data[0,i],Data[1,i]));
                //    i += 35;
            }

            // Generate a red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve(DataName,
                list, Color.Red, SymbolType.Diamond);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            //myPane.XAxis.Scale.Type=AxisType.Date ;

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;

            // Fill the axis background with a gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            zGraph.AxisChange();
            // Make sure the Graph gets redrawn
            zGraph.Invalidate();
            //zGraph.Refresh();
            
            this.Refresh();
        }
    }
}
