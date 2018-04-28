using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetworkUI;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Utils;
using NetworkModel;
using System.Windows.Threading;
using ZoomAndPan;
using System.Collections;

namespace SampleCode
{
    /// <summary>
    /// This is a Window that uses NetworkView to display a flow-chart.
    /// </summary>
    public partial class MainWindow : Window
    {
		
		public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get
            {
                return (MainWindowViewModel)DataContext;
            }
        }

        /// <summary>
        /// Event raised when the Window has loaded.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//
			// Display help text for the sample app.
			//
			HelpTextWindow helpTextWindow = new HelpTextWindow();
			helpTextWindow.Left = this.Left + this.Width + 5;
			helpTextWindow.Top = this.Top;
			helpTextWindow.Owner = this;
			//helpTextWindow.Show();

			OverviewWindow overviewWindow = new OverviewWindow();
            overviewWindow.Left = this.Left;
            overviewWindow.Top = this.Top + this.Height + 5;
            overviewWindow.Owner = this;
            overviewWindow.DataContext = this.ViewModel; // Pass the view model onto the overview window.
			//overviewWindow.Show();

			OpenTableWindow();
		}

		private void NewNetwork_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			TaskSelectionDialog tsd = new TaskSelectionDialog(Constants.TasksCount);
			tsd.Owner = this;
			tsd.ShowDialog();
			if (tsd.DialogResult == true)
			{
				int taskId = tsd.TaskId;
				ViewModel.OpenTask(taskId);
			}
		}

		private void SaveNetwork_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.SaveToFile();
		}

		private void SaveNetwork_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.IsNetworkLoaded;
		}

		private void SaveAsNetwork_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.SaveAsToFile();
		}

		private void OpenNetwork_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.LoadFromFile();
		}

		private void CompareNetwork_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Compare();
		}

		private void ShowTable_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (!Helpers.IsWindowOpen<TableWindow>())
			{
				OpenTableWindow();
			}
		}

	    private void OpenTableWindow()
		{
			var tableWindow = new TableWindow(this.ViewModel);
			tableWindow.Left = this.Left;
			tableWindow.Top = this.Top + this.Height + 5;
			tableWindow.Owner = this;
			tableWindow.Show();
		}

		private void ShowAbout_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow();
			aboutWindow.Owner = this;
			aboutWindow.ShowDialog();
		}

		private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		/// <summary>
		/// Event raised when the user has started to drag out a connection.
		/// </summary>
		private void networkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(networkControl);

            //
            // Delegate the real work to the view model.
            //
            var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            //
            // Must return the view-model object that represents the connection via the event args.
            // This is so that NetworkView can keep track of the object while it is being dragged.
            //
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised, to query for feedback, while the user is dragging a connection.
        /// </summary>
        private void networkControl_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var draggedOverConnector= (ConnectorViewModel)e.DraggedOverConnector;
            object feedbackIndicator = null;
            bool connectionOk = true;

            this.ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);

            //
            // Return the feedback object to NetworkView.
            // The object combined with the data-template for it will be used to create a 'feedback icon' to
            // display (in an adorner) to the user.
            //
            e.FeedbackIndicator = feedbackIndicator;

            //
            // Let NetworkView know if the connection is ok or not ok.
            //
            e.ConnectionOk = connectionOk;
        }

        /// <summary>
        /// Event raised while the user is dragging a connection.
        /// </summary>
        private void networkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            Point curDragPoint = Mouse.GetPosition(networkControl);
            var connection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void networkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            var connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            var newConnection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        /// <summary>
        /// Event raised to delete the selected node.
        /// </summary>
        private void DeleteSelectedNodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.DeleteSelectedNodes();
        }

        /// <summary>
        /// Event raised to create a new node.
        /// </summary>
        private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNode();
        }

        /// <summary>
        /// Event raised to create a new node.
        /// </summary>
        private void CreateNodeAtCenter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
	        var center = new Point(zoomAndPanControl.ContentOffsetX + zoomAndPanControl.ContentViewportWidth/2,
				zoomAndPanControl.ContentOffsetY + zoomAndPanControl.ContentViewportHeight / 2);
            CreateNode(center);
        }

        /// <summary>
        /// Event raised to delete a node.
        /// </summary>
        private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var node = (NodeViewModel)e.Parameter;
            this.ViewModel.DeleteNode(node);
		}

		/// <summary>
		/// Event raised to delete a connection.
		/// </summary>
		private void DeleteConnection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var connection = (ConnectionViewModel)e.Parameter;
            this.ViewModel.DeleteConnection(connection);
        }

        /// <summary>
        /// Creates a new node in the network at the current mouse location.
        /// </summary>
        private void CreateNode()
        {
            var newNodePosition = Mouse.GetPosition(networkControl);
            this.ViewModel.CreateNode("a1", newNodePosition, true);
        }

        /// <summary>
        /// Creates a new node in the network at the current mouse location.
        /// </summary>
        private void CreateNode(Point nodePosition)
        {
            this.ViewModel.CreateNode("a1", nodePosition, true);
        }

        /// <summary>
        /// Event raised when the size of a node has changed.
        /// </summary>
        private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            //
            var element = (FrameworkElement)sender;
            var node = (NodeViewModel)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
        }
    }
}
