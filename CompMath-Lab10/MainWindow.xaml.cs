using System.Linq;
using System.Windows;
using System.Windows.Controls;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using static System.Math;

namespace CompMath_Lab10;

public partial class MainWindow : Window
{
	private double a;
	private double q;
	private double l;
	private double maxT;
	private double dx;
	private double dt;

	private readonly LineSeries series;

	public MainWindow()
	{
		InitializeComponent();

		DataContext = this;

		Model = new()
		{
			Title = "||y-u||(t)"
		};

		var xAxis = new LinearAxis()
		{
			Position = AxisPosition.Bottom,
			Title = "t",
			MajorGridlineStyle = LineStyle.Solid,
		};
		Model.Axes.Add(xAxis);

		var yAxis = new LinearAxis()
		{
			Position = AxisPosition.Left,
			MajorGridlineStyle = LineStyle.Solid,
		};
		Model.Axes.Add(yAxis);

		series = new();
		Model.Series.Add(series);

		UpdatePlot();
	}

	public PlotModel Model { get; set; }

	private static bool TryGetInput(TextBox? textBox, out double res)
	{
		if (textBox is null)
		{
			res = 0.0;
			return false;
		}
		return double.TryParse(textBox.Text, out res);
	}

	private void UpdatePlot()
	{
		if (!(TryGetInput(aInput, out a)
			&& TryGetInput(qInput, out q)
			&& TryGetInput(lInput, out l)
			&& TryGetInput(tInput, out maxT)
			&& TryGetInput(dxInput, out dx)
			&& TryGetInput(dtInput, out dt))
			|| plot is null
			|| series is null)
		{
			return;
		}

		int nx = (int)(l / dx) + 1;
		int nt = (int)(maxT / dt) + 1;
		double alpha = a * (PI / l) * (PI / l) + q;

		double U0(double x) => Sin(PI * x / l);
		double Y(double x, double t) => Exp(-alpha * t) * Sin(PI * x / l);

		try
		{
			var u = CrankNicolsonMethod.Solve(a, q, l, U0, maxT, dx, dt);

			series.Points.Clear();
			for (int j = 1; j < nt; j++)
			{
				double t = dt * j;
				double e = Enumerable.Range(0, nx).Select(i => Abs(Y(dx * i, t) - u[i, j])).Max();
				series.Points.Add(new(t, e));
			}
			plot.InvalidatePlot();
		}
		catch
		{
			return;
		}
	}

	private void UpdatePlot(object sender, TextChangedEventArgs ea)
	{
		UpdatePlot();
	}
}
