using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;

namespace NetworkModel
{
	/// <summary>
	/// Defines a connection between two connectors (aka connection points) of two nodes.
	/// </summary>
	public sealed class ConnectionViewModel : AbstractModelBase
	{
		#region Internal Data Members

		/// <summary>
		/// The source connector the connection is attached to.
		/// </summary>
		private ConnectorViewModel sourceConnector = null;

		/// <summary>
		/// The destination connector the connection is attached to.
		/// </summary>
		private ConnectorViewModel destConnector = null;

		/// <summary>
		/// The source and dest hotspots used for generating connection points.
		/// </summary>
		private Point sourceConnectorHotspot;
		private Point destConnectorHotspot;

		/// <summary>
		/// Points that make up the connection.
		/// </summary>
		private PointCollection points = null;

		/// <summary>
		/// Point where the information about connection is placed.
		/// </summary>
		private Thickness labelPoint;

		private string input;

		private string output;

		#endregion Internal Data Members

		public ConnectionViewModel()
		{
			Input = "x1";
			Output = "y1";
		}

		/// <summary>
		/// The source connector the connection is attached to.
		/// </summary>
		public ConnectorViewModel SourceConnector
		{
			get
			{
				return sourceConnector;
			}
			set
			{
				if (sourceConnector == value)
				{
					return;
				}

				if (sourceConnector != null)
				{
					sourceConnector.AttachedConnections.Remove(this);
					sourceConnector.HotspotUpdated -= new EventHandler<EventArgs>(sourceConnector_HotspotUpdated);
				}

				sourceConnector = value;

				if (sourceConnector != null)
				{
					sourceConnector.AttachedConnections.Add(this);
					sourceConnector.HotspotUpdated += new EventHandler<EventArgs>(sourceConnector_HotspotUpdated);
					this.SourceConnectorHotspot = sourceConnector.Hotspot;
				}

				OnPropertyChanged("SourceConnector");
				OnConnectionChanged();
			}
		}

		/// <summary>
		/// The destination connector the connection is attached to.
		/// </summary>
		public ConnectorViewModel DestConnector
		{
			get
			{
				return destConnector;
			}
			set
			{
				if (destConnector == value)
				{
					return;
				}

				if (destConnector != null)
				{
					destConnector.AttachedConnections.Remove(this);
					destConnector.HotspotUpdated -= new EventHandler<EventArgs>(destConnector_HotspotUpdated);
				}

				destConnector = value;

				if (destConnector != null)
				{
					destConnector.AttachedConnections.Add(this);
					destConnector.HotspotUpdated += new EventHandler<EventArgs>(destConnector_HotspotUpdated);
					this.DestConnectorHotspot = destConnector.Hotspot;
				}

				OnPropertyChanged("DestConnector");
				OnConnectionChanged();
			}
		}

		/// <summary>
		/// The source and dest hotspots used for generating connection points.
		/// </summary>
		public Point SourceConnectorHotspot
		{
			get
			{
				return sourceConnectorHotspot;
			}
			set
			{
				sourceConnectorHotspot = value;

				ComputeConnectionPoints();
				ComputeAngle();

				OnPropertyChanged("SourceConnectorHotspot");
			}
		}

		public Point DestConnectorHotspot
		{
			get
			{
				return destConnectorHotspot;
			}
			set
			{
				destConnectorHotspot = value;

				ComputeConnectionPoints();
				ComputeAngle();

				OnPropertyChanged("DestConnectorHotspot");
			}
		}

		/// <summary>
		/// Points that make up the connection.
		/// </summary>
		public PointCollection Points
		{
			get
			{
				return points;
			}
			set
			{
				points = value;

				OnPropertyChanged("Points");
			}
		}

		public Thickness LabelPoint
		{
			get
			{
				return labelPoint;
			}
			set
			{
				labelPoint = value;
				OnPropertyChanged("LabelPoint");
			}
		}

		public string Input
		{
			get
			{
				return input;

			}
			set
			{
				input = value;
				OnPropertyChanged("Input");
			}
		}

		public string Output
		{
			get
			{
				return output;

			}
			set
			{
				output = value;
				OnPropertyChanged("Output");
			}
		}


		/// <summary>
		/// Event fired when the connection has changed.
		/// </summary>
		public event EventHandler<EventArgs> ConnectionChanged;

		#region Private Methods

		/// <summary>
		/// Raises the 'ConnectionChanged' event.
		/// </summary>
		private void OnConnectionChanged()
		{
			if (ConnectionChanged != null)
			{
				ConnectionChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Event raised when the hotspot of the source connector has been updated.
		/// </summary>
		private void sourceConnector_HotspotUpdated(object sender, EventArgs e)
		{
			this.SourceConnectorHotspot = this.SourceConnector.Hotspot;
		}

		/// <summary>
		/// Event raised when the hotspot of the dest connector has been updated.
		/// </summary>
		private void destConnector_HotspotUpdated(object sender, EventArgs e)
		{
			this.DestConnectorHotspot = this.DestConnector.Hotspot;
		}

		/// <summary>
		/// Rebuild connection points.
		/// </summary>
		private void ComputeConnectionPoints()
		{
			PointCollection computedPoints = new PointCollection();
			computedPoints.Add(this.SourceConnectorHotspot);

			// get the middle of the arc
			if ((this.SourceConnectorHotspot.X > 0 || this.SourceConnectorHotspot.Y > 0) &&
				(this.DestConnectorHotspot.X > 0 || this.DestConnectorHotspot.Y > 0))
			{
				var startPoint = Points[0];
				var lastPoint = Points[Points.Count - 1];

				// Make an arc.
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

				double fractionHalf = 0.5; //the relative point of the curve
				Point ptHalf; //the absolute point of the curve
				Point tgHalf; //the tangent point of the curve
				pathGeometry.GetPointAtFractionLength(
					fractionHalf,
					out ptHalf,
					out tgHalf);

				// TODO
				if (distance.Length < 30)
				{
					ptHalf.X -= 15;
					ptHalf.Y -= 58;
				}
				else
				{
					ptHalf.X -= 5;
					ptHalf.Y += 3;
				}

				computedPoints.Add(ptHalf);

				double fractionArrow = 0.9; //the relative point of the curve
				Point ptArrow; //the absolute point of the curve
				Point tgArrow; //the tangent point of the curve
				pathGeometry.GetPointAtFractionLength(
					fractionArrow,
					out ptArrow,
					out tgArrow);

				computedPoints.Add(ptArrow);
			}
			else
			{
				computedPoints.Add(this.SourceConnectorHotspot);
			}

			computedPoints.Add(this.DestConnectorHotspot);
			computedPoints.Freeze();

			this.Points = computedPoints;
			LabelPoint = new Thickness(computedPoints[1].X, computedPoints[1].Y, 0, 0);
		}

		private void ComputeAngle()
		{
			if (destConnector == null || sourceConnector == null) return;

			if (sourceConnector.ParentNode == destConnector.ParentNode)
			{
				const double SourceAngle = 150;
				const double DestAngle = 210;

				sourceConnector.Angle = SourceAngle;
				destConnector.Angle = DestAngle;

				return;
			}

			double angle = Math.Atan2(destConnector.ParentNode.Y - sourceConnector.ParentNode.Y,
				destConnector.ParentNode.X - sourceConnector.ParentNode.X);
			angle *= 180 / Math.PI;

			if (angle < 0)
			{
				angle += 360;
			}

			var destAngle = angle + 180;

			if (destAngle > 360)
			{
				destAngle -= 360;
			}

			//
			// Offset the angle of the source and destination nodes to make a difference
			// between source and destination connectors of two-way connection
			//
			const double AngleOffset = 30;

			sourceConnector.Angle = angle + AngleOffset;
			destConnector.Angle = destAngle - AngleOffset;
		}

		#endregion Private Methods
	}
}
