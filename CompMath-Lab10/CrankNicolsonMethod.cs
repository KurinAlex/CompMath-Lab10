using System;
using System.Linq;

namespace CompMath_Lab10;

public static class CrankNicolsonMethod
{
	public static double[,] Solve(double a, double q, double l, Func<double, double> u0, double maxT, double dx, double dt)
	{
		int nx = (int)(l / dx);
		int nt = (int)(maxT / dt);
		var u = new double[nx + 1, nt + 1];

		for (int i = 0; i <= nx; i++)
		{
			u[i, 0] = u0(i * dx);
		}
		for (int j = 0; j <= nt; j++)
		{
			u[0, j] = 0.0;
			u[nx, j] = 0.0;
		}

		double idt = 1.0 / dt;
		double idx2 = 1.0 / (2.0 * dx * dx);
		double q2 = q / 2.0;

		double aa = idx2;
		double bb = idt - 2.0 * idx2 + q2;
		double w = aa / bb;
		bb -= w * aa;

		for (int j = 1; j <= nt; j++)
		{
			var dd = Enumerable.Range(1, nx - 1)
				.Select(i => (idt - q2) * u[i, j - 1] + a * idx2 * (u[i + 1, j - 1] - 2 * u[i, j - 1] + u[i - 1, j - 1]))
				.ToArray();

			for (int i = 1; i < nx - 1; i++)
			{
				dd[i] -= w * dd[i - 1];
			}

			u[nx - 1, j] = dd[nx - 2] / bb;
			for (int i = nx - 2; i > 0; i--)
			{
				u[i, j] = (dd[i - 1] - aa * u[i + 1, j]) / bb;
			}
		}
		return u;
	}
}
