using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace SampleCode
{
	/// <summary>
	/// Defines an arrow that has multiple points.
	/// </summary>
	public class CurvedArrow : Shape
	{
		#region Dependency Property/Event Definitions

		public static readonly DependencyProperty ArrowHeadLengthProperty =
			DependencyProperty.Register("ArrowHeadLength", typeof(double), typeof(CurvedArrow),
				new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ArrowHeadWidthProperty =
			DependencyProperty.Register("ArrowHeadWidth", typeof(double), typeof(CurvedArrow),
				new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty PointsProperty =
			DependencyProperty.Register("Points", typeof(PointCollection), typeof(CurvedArrow),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		#endregion Dependency Property/Event Definitions

		/// <summary>
		/// The angle (in degrees) of the arrow head.
		/// </summary>
		public double ArrowHeadLength
		{
			get
			{
				return (double)GetValue(ArrowHeadLengthProperty);
			}
			set
			{
				SetValue(ArrowHeadLengthProperty, value);
			}
		}

		/// <summary>
		/// The width of the arrow head.
		/// </summary>
		public double ArrowHeadWidth
		{
			get
			{
				return (double)GetValue(ArrowHeadWidthProperty);
			}
			set
			{
				SetValue(ArrowHeadWidthProperty, value);
			}
		}

		/// <summary>
		/// The intermediate points that make up the line between the start and the end.
		/// </summary>
		public PointCollection Points
		{
			get
			{
				return (PointCollection)GetValue(PointsProperty);
			}
			set
			{
				SetValue(PointsProperty, value);
			}
		}

		#region Private Methods

		/// <summary>
		/// Return the shape's geometry.
		/// </summary>
		protected override Geometry DefiningGeometry
		{
			get
			{
				if (Points == null || Points.Count < 2)
				{
					return new GeometryGroup();
				}

				//
				// Geometry has not yet been generated.
				// Generate geometry and cache it.
				//
				Geometry geometry = GenerateGeometry();

				GeometryGroup group = new GeometryGroup();
				group.Children.Add(geometry);

				GenerateArrowHeadGeometry(group);

				//
				// Return cached geometry.
				//
				return group;
			}
		}

		/// <summary>
		/// Generate the geometry for the three optional arrow symbols at the start, middle and end of the arrow.
		/// </summary>
		private void GenerateArrowHeadGeometry(GeometryGroup geometryGroup)
		{
			Point startPoint = Points[0];

			Point penultimatePoint = Points[Points.Count - 2];
			Point arrowHeadTip = Points[Points.Count - 1];
			Vector startDir = arrowHeadTip - penultimatePoint;
			startDir.Normalize();
			Point basePoint = arrowHeadTip - (startDir * ArrowHeadLength);
			Vector crossDir = new Vector(-startDir.Y, startDir.X);

			Point[] arrowHeadPoints = new Point[3];
			arrowHeadPoints[0] = arrowHeadTip;
			arrowHeadPoints[1] = basePoint - (crossDir * (ArrowHeadWidth / 2));
			arrowHeadPoints[2] = basePoint + (crossDir * (ArrowHeadWidth / 2));

			//
			// Build geometry for the arrow head.
			//
			PathFigure arrowHeadFig = new PathFigure();
			arrowHeadFig.IsClosed = true;
			arrowHeadFig.IsFilled = true;
			arrowHeadFig.StartPoint = arrowHeadPoints[0];
			arrowHeadFig.Segments.Add(new LineSegment(arrowHeadPoints[1], true));
			arrowHeadFig.Segments.Add(new LineSegment(arrowHeadPoints[2], true));

			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.Figures.Add(arrowHeadFig);

			geometryGroup.Children.Add(pathGeometry);
		}

		/// <summary>
		/// Generate the shapes geometry.
		/// </summary>
		protected Geometry GenerateGeometry()
		{
			var startPoint = Points[0];
			var lastPoint = Points[Points.Count - 1];

			ArcSegment arc = new ArcSegment();
			arc.Point = lastPoint;
			arc.IsLargeArc = true;

			Size arcRadius = new Size();
			var distance = startPoint - lastPoint;
			if (distance.Length < 30)
			{
				arc.SweepDirection = SweepDirection.Clockwise;
				arc.IsLargeArc = true;
				arcRadius.Width = 25;
				arcRadius.Height = 25;
			}
			else
			{
				arc.SweepDirection = SweepDirection.Counterclockwise;
				arc.IsLargeArc = false;
				int arcRadiusMin = 100;
				arcRadius.Width = Math.Abs(startPoint.X - lastPoint.X);
				arcRadius.Height = Math.Abs(startPoint.Y - lastPoint.Y);
				if (arcRadius.Width < arcRadiusMin) arcRadius.Width = arcRadiusMin;
				if (arcRadius.Height < arcRadiusMin) arcRadius.Height = arcRadiusMin;
			}
			arc.Size = arcRadius;

			// Make an arc.
			PathFigure fig = new PathFigure();
			fig.IsClosed = false;
			fig.IsFilled = false;
			fig.StartPoint = startPoint;
			fig.Segments.Add(arc);

			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.Figures.Add(fig);

			return pathGeometry;
		}

		#endregion Private Methods
	}
}
