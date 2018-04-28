using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using NetworkModel;
using System.Windows;
using System.Diagnostics;
using FsmUtils;
using Newtonsoft.Json;
using TableModel;

namespace SampleCode
{
	/// <summary>
	/// The view-model for the main window.
	/// </summary>
	public class MainWindowViewModel : AbstractModelBase
	{
		#region Internal Data Members

		public event TableChangedEventHandler TableChanged;

		/// <summary>
		/// This is the network that is displayed in the window.
		/// It is the main part of the view-model.
		/// </summary>
		public NetworkViewModel network = null;

		private readonly FileHandler _fileHandler = new FileHandler();

		private string _filePath;

		/// <summary>
		/// Current task. Contains two tables, id and type.
		/// </summary>
		private TableViewModel _tableViewModel;

		///
		/// The current scale at which the content is being viewed.
		/// 
		private double contentScale = 1;

		///
		/// The X coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		private double contentOffsetX = 0;

		///
		/// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		private double contentOffsetY = 0;

		///
		/// The width of the content (in content coordinates).
		/// 
		private double contentWidth = 1000;

		///
		/// The heigth of the content (in content coordinates).
		/// 
		private double contentHeight = 1000;

		///
		/// The width of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		private double contentViewportWidth = 0;

		///
		/// The height of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		private double contentViewportHeight = 0;

		private IList<string> possibleStates;

		private IList<string> possibleInputs;

		private IList<string> possibleOutputs;

		JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
			PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			ObjectCreationHandling = ObjectCreationHandling.Auto
		};

		#endregion Internal Data Members

		public MainWindowViewModel()
		{
			// Add some test data to the view-model.
			//PopulateWithTestData();
		}

		/// <summary>
		/// This is the network that is displayed in the window.
		/// It is the main part of the view-model.
		/// </summary>
		public NetworkViewModel Network
		{
			get
			{
				return network;
			}
			set
			{
				network = value;

				OnPropertyChanged("Network");
				OnPropertyChanged("IsNetworkLoaded");
			}
		}

		/// <summary>
		/// Shows whether the network is loaded or not.
		/// </summary>
		public bool IsNetworkLoaded
		{
			get
			{
				return Network != null; 
				
			}
		}

		///
		/// The current scale at which the content is being viewed.
		/// 
		public double ContentScale
		{
			get
			{
				return contentScale;
			}
			set
			{
				contentScale = value;

				OnPropertyChanged("ContentScale");
			}
		}

		///
		/// The X coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		public double ContentOffsetX
		{
			get
			{
				return contentOffsetX;
			}
			set
			{
				contentOffsetX = value;

				OnPropertyChanged("ContentOffsetX");
			}
		}

		///
		/// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		public double ContentOffsetY
		{
			get
			{
				return contentOffsetY;
			}
			set
			{
				contentOffsetY = value;

				OnPropertyChanged("ContentOffsetY");
			}
		}

		///
		/// The width of the content (in content coordinates).
		/// 
		public double ContentWidth
		{
			get
			{
				return contentWidth;
			}
			set
			{
				contentWidth = value;

				OnPropertyChanged("ContentWidth");
			}
		}

		///
		/// The heigth of the content (in content coordinates).
		/// 
		public double ContentHeight
		{
			get
			{
				return contentHeight;
			}
			set
			{
				contentHeight = value;

				OnPropertyChanged("ContentHeight");
			}
		}

		///
		/// The width of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		public double ContentViewportWidth
		{
			get
			{
				return contentViewportWidth;
			}
			set
			{
				contentViewportWidth = value;

				OnPropertyChanged("ContentViewportWidth");
			}
		}

		///
		/// The heigth of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		public double ContentViewportHeight
		{
			get
			{
				return contentViewportHeight;
			}
			set
			{
				contentViewportHeight = value;

				OnPropertyChanged("ContentViewportHeight");
			}
		}

		public IList<string> PossibleStates
		{
			get { return possibleStates; }
			set
			{
				possibleStates = value;
				OnPropertyChanged("PossibleStates");
			}
		}

		public IList<string> PossibleInputs
		{
			get { return new List<string>(possibleInputs); }
			set
			{
				possibleInputs = value;
				OnPropertyChanged("PossibleInputs");
			}
		}

		public IList<string> PossibleOutputs
		{
			get { return possibleOutputs; }
			set
			{
				possibleOutputs = value;
				OnPropertyChanged("PossibleOutputs");
			}
		}

		public TableViewModel Table
		{
			get { return _tableViewModel; }
			set
			{
				_tableViewModel = value;
				OnPropertyChanged("Table");
			}
		}

		/// <summary>
		/// Called when the user has started to drag out a connector, thus creating a new connection.
		/// </summary>
		public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
		{
			//
			// Create a new connection to add to the view-model.
			//
			var connection = new ConnectionViewModel();

			if (draggedOutConnector.Type == ConnectorType.Output)
			{
				//
				// The user is dragging out a source connector (an output) and will connect it to a destination connector (an input).
				//
				connection.SourceConnector = draggedOutConnector;
				connection.DestConnectorHotspot = curDragPoint;
			}
			else
			{
				//
				// The user is dragging out a destination connector (an input) and will connect it to a source connector (an output).
				//
				connection.DestConnector = draggedOutConnector;
				connection.SourceConnectorHotspot = curDragPoint;
			}

			//
			// Add the new connection to the view-model.
			//
			this.Network.Connections.Add(connection);

			return connection;
		}

		/// <summary>
		/// Called to query the application for feedback while the user is dragging the connection.
		/// </summary>
		public void QueryConnnectionFeedback(ConnectorViewModel draggedOutConnector, ConnectorViewModel draggedOverConnector, out object feedbackIndicator, out bool connectionOk)
		{
			if (draggedOutConnector == draggedOverConnector)
			{
				//
				// Can't connect to self!
				// Provide feedback to indicate that this connection is not valid!
				//
				feedbackIndicator = new ConnectionBadIndicator();
				connectionOk = false;
			}
			else
			{
				var sourceConnector = draggedOutConnector;
				var destConnector = draggedOverConnector;

				//
				// Only allow connections from output connector to input connector (ie each
				// connector must have a different type).
				// Also only allocation from one node to another, never one node back to the same node.
				//
				connectionOk = sourceConnector.ParentNode != destConnector.ParentNode &&
								 sourceConnector.Type != destConnector.Type;

				if (connectionOk)
				{
					// 
					// Yay, this is a valid connection!
					// Provide feedback to indicate that this connection is ok!
					//
					feedbackIndicator = new ConnectionOkIndicator();
				}
				else
				{
					//
					// Connectors with the same connector type (eg input & input, or output & output)
					// can't be connected.
					// Only connectors with separate connector type (eg input & output).
					// Provide feedback to indicate that this connection is not valid!
					//
					feedbackIndicator = new ConnectionBadIndicator();
				}
			}
		}

		/// <summary>
		/// Called as the user continues to drag the connection.
		/// </summary>
		public void ConnectionDragging(Point curDragPoint, ConnectionViewModel connection)
		{
			if (connection.DestConnector == null)
			{
				connection.DestConnectorHotspot = curDragPoint;
			}
			else
			{
				connection.SourceConnectorHotspot = curDragPoint;
			}
		}

		/// <summary>
		/// Called when the user has finished dragging out the new connection.
		/// </summary>
		public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
		{
			if (connectorDraggedOver == null)
			{
				//
				// The connection was unsuccessful.
				// Maybe the user dragged it out and dropped it in empty space.
				//
				this.Network.Connections.Remove(newConnection);
				return;
			}

			//
			// Only allow connections from output connector to input connector (ie each
			// connector must have a different type).
			// Also only allocation from one node to another, never one node back to the same node.
			//
			bool connectionOk = connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode &&
								connectorDraggedOut.Type != connectorDraggedOver.Type;

			if (!connectionOk)
			{
				//
				// Connections between connectors that have the same type,
				// eg input -> input or output -> output, are not allowed,
				// Remove the connection.
				//
				this.Network.Connections.Remove(newConnection);
				return;
			}

			//
			// The user has dragged the connection on top of another valid connector.
			//

			//
			// Remove any existing connection between the same two connectors.
			//
			var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
			if (existingConnection != null)
			{
				this.Network.Connections.Remove(existingConnection);
			}

			//
			// Finalize the connection by attaching it to the connector
			// that the user dragged the mouse over.
			//
			if (newConnection.DestConnector == null)
			{
				newConnection.DestConnector = connectorDraggedOver;
			}
			else
			{
				newConnection.SourceConnector = connectorDraggedOver;
			}
		}

		/// <summary>
		/// Retrieve a connection between the two connectors.
		/// Returns null if there is no connection between the connectors.
		/// </summary>
		public ConnectionViewModel FindConnection(ConnectorViewModel connector1, ConnectorViewModel connector2)
		{
			Trace.Assert(connector1.Type != connector2.Type);

			//
			// Figure out which one is the source connector and which one is the
			// destination connector based on their connector types.
			//
			var sourceConnector = connector1.Type == ConnectorType.Output ? connector1 : connector2;
			var destConnector = connector1.Type == ConnectorType.Output ? connector2 : connector1;

			//
			// Now we can just iterate attached connections of the source
			// and see if it each one is attached to the destination connector.
			//

			foreach (var connection in sourceConnector.AttachedConnections)
			{
				if (connection.DestConnector == destConnector)
				{
					//
					// Found a connection that is outgoing from the source connector
					// and incoming to the destination connector.
					//
					return connection;
				}
			}

			return null;
		}

		/// <summary>
		/// Delete the currently selected nodes from the view-model.
		/// </summary>
		public void DeleteSelectedNodes()
		{
			if (Network == null) return;

			// Take a copy of the selected nodes list so we can delete nodes while iterating.
			var nodesCopy = this.Network.Nodes.ToArray();
			foreach (var node in nodesCopy)
			{
				if (node.IsSelected)
				{
					DeleteNode(node);
				}
			}
		}

		/// <summary>
		/// Delete the node from the view-model.
		/// Also deletes any connections to or from the node.
		/// </summary>
		public void DeleteNode(NodeViewModel node)
		{
			//
			// Remove all connections attached to the node.
			//
			this.Network.Connections.RemoveRange(node.AttachedConnections);

			//
			// Remove the node from the network.
			//
			this.Network.Nodes.Remove(node);
		}

		/// <summary>
		/// Create a node and add it to the view-model.
		/// </summary>
		public NodeViewModel CreateNode(string name, Point nodeLocation, bool centerNode)
		{
			if (Network == null) return null;

			var node = new NodeViewModel(name);
			node.X = nodeLocation.X;
			node.Y = nodeLocation.Y;

			if (centerNode)
			{
				// 
				// We want to center the node.
				//
				// For this to happen we need to wait until the UI has determined the 
				// size based on the node's data-template.
				//
				// So we define an anonymous method to handle the SizeChanged event for a node.
				//
				// Note: If you don't declare sizeChangedEventHandler before initializing it you will get
				//       an error when you try and unsubscribe the event from within the event handler.
				//
				EventHandler<EventArgs> sizeChangedEventHandler = null;
				sizeChangedEventHandler =
					delegate (object sender, EventArgs e)
					{
						//
						// This event handler will be called after the size of the node has been determined.
						// So we can now use the size of the node to modify its position.
						//
						node.X -= node.Size.Width / 2;
						node.Y -= node.Size.Height / 2;

						//
						// Don't forget to unhook the event, after the initial centering of the node
						// we don't need to be notified again of any size changes.
						//
						node.SizeChanged -= sizeChangedEventHandler;
					};

				//
				// Now we hook the SizeChanged event so the anonymous method is called later
				// when the size of the node has actually been determined.
				//
				node.SizeChanged += sizeChangedEventHandler;
			}

			//
			// Add the node to the view-model.
			//
			this.Network.Nodes.Add(node);

			// 
			// Remove selection and select created node.
			//
			RemoveSelection();
			node.IsSelected = true;

			return node;
		}

		public void RemoveSelection()
		{
			if (Network == null) return;

			foreach (var n in this.Network.Nodes)
			{
				n.IsSelected = false;
			}
		}

		/// <summary>
		/// Connect the currently selected nodes from the view-model.
		/// </summary>
		/// <param name="to"></param>
		public void ConnectSelectedNodes(NodeViewModel target)
		{
			if (Network == null) return;

			// Take a copy of the selected nodes list so we can delete nodes while iterating.
			var nodesCopy = this.Network.Nodes.ToArray();
			foreach (var node in nodesCopy)
			{
				if (node.IsSelected)
				{
					ConnectNodes(node, target);
				}
			}
		}

		/// <summary>
		/// Utility method to delete a connection from the view-model.
		/// </summary>
		public void DeleteConnection(ConnectionViewModel connection)
		{
			this.Network.Connections.Remove(connection);
		}

		/// <summary>
		/// Create a connection between two nodes.
		/// </summary>
		/// <param name="source">Source node</param>
		/// <param name="destination">Destination node</param>
		public void ConnectNodes(NodeViewModel source, NodeViewModel destination)
		{
			if (Network == null || source == null || destination == null) return;

			//
			// Check if the connection already exists.
			//
			if (source.AttachedConnections.Any(c => c.DestConnector.ParentNode == destination))
			{
				return;
			}

			//
			// Create connectors
			//
			ConnectorViewModel sourceConn = new ConnectorViewModel("Out1");
			ConnectorViewModel destinationConn = new ConnectorViewModel("In1");
			source.OutputConnectors.Add(sourceConn);
			destination.InputConnectors.Add(destinationConn);

			//
			// Create a connection between the nodes.
			//
			ConnectionViewModel connection = new ConnectionViewModel();
			connection.SourceConnector = sourceConn;
			connection.DestConnector = destinationConn;

			//
			// Add the connection to the view-model.
			//
			this.Network.Connections.Add(connection);
		}

		public void OpenTask(int taskId)
		{
			TableViewModel table = LoadTask(taskId);

			if (table == null)
			{
				return;
			}

			this.Table = table;

			try
			{
				Network = new NetworkViewModel(table.TaskId, table.Type, table.StatesCount, table.InputsCount, table.OutputsCount);
				TableChanged?.Invoke(this, new TableChangedEventArgs(Table));
				UpdatePossibleListsValues();
			}
			catch
			{
				Network = null;
				TableChanged?.Invoke(this, new TableChangedEventArgs(null));
			}
			finally
			{
				_filePath = "";
			}
		}

		public void LoadFromFile()
		{
			string newFilePath;
			NetworkViewModel newNetwork;
			TableViewModel newTable;

			try
			{
				var json = _fileHandler.Open(out newFilePath);

				if (json == null) return;

				newNetwork = JsonConvert.DeserializeObject<NetworkViewModel>(json, _jsonSettings);

				if (newNetwork == null)
				{
					MessageBox.Show("Не удалось загрузить файл.",
						"Ошибка при загрузке", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}
				
				newTable = LoadTask(newNetwork.TaskNumber);

				if (newTable == null)
				{
					MessageBox.Show("Не удалось загрузить файл.",
						"Ошибка при загрузке", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Не удалось загрузить файл.\n\n" + ex.Message,
					"Ошибка при загрузке", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			_filePath = newFilePath;
			Network = newNetwork;
			UpdatePossibleListsValues();
		}

		public void SaveToFile()
		{
			if (Network == null) return;

			try
			{


				var json = JsonConvert.SerializeObject(Network, Formatting.Indented, _jsonSettings);
				_fileHandler.Save(json, _filePath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Возникла ошибка при сохранении файла.\n\n" + ex.Message,
					"Ошибка при сохранении", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		public void SaveAsToFile()
		{
			if (Network == null) return;

			var oldFilePath = _filePath;

			try
			{
				var json = JsonConvert.SerializeObject(Network, Formatting.Indented, _jsonSettings);
				_fileHandler.SaveAs(json, out _filePath);
			}
			catch (Exception ex)
			{
				_filePath = oldFilePath;
				MessageBox.Show("Возникла ошибка при сохранении файла.\n\n" + ex.Message,
					"Ошибка при сохранении", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		public void Compare()
		{
			if (Table == null || Network == null) return;

			var ok = false;
			if (Table.Type == NetworkType.Mealy)
			{
				ok = CompareMealy();
			}
			else if (Table.Type == NetworkType.Moore)
			{
				ok = CompareMoore();
			}

			if (ok)
			{
				MessageBox.Show("Граф автомата соответствует таблице переходов.",
					"Результат проверки", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				MessageBox.Show("Были найдены ошибки. Пожалуйста, проверьте ваше решение.",
					"Результат проверки", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		#region Private Methods

		private TableViewModel LoadTask(int taskId)
		{
			string json;
			try
			{
				json = _fileHandler.OpenTable(taskId);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Не удалось выполнить загрузку файла задачи. " +
								$"Пожалуйста, проверьте наличие файла в папке \"data\\table_{taskId}\" программы. \n\n" + ex.Message,
					"Ошибка при загрузке задачи", MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}

			try
			{
				return JsonConvert.DeserializeObject<TableViewModel>(json);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Не удалось загрузить содержимое файла задачи. Возможно, файл поврежден.\n\n" + ex.Message,
					"Ошибка при загрузке задачи", MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}
		}

		private bool CompareMealy()
		{
			for (var i = 1; i <= Table.StatesCount; i++)
			{
				var state = "a" + i;
				for (var j = 1; j <= Table.InputsCount; j++)
				{
					var input = "x" + j;
					var nextState = Table.ContentFirst[j - 1][i] as string;
					var output = Table.ContentSecond[j - 1][i] as string;

					if (nextState == "-" || output == "-") continue;

					var connection = Network.GetConnection(state, nextState, input);

					if (connection == null || connection.Output != output) return false;
				}
			}

			return true;
		}

		private bool CompareMoore()
		{
			for (var i = 1; i <= Table.StatesCount; i++)
			{
				var stateName = "a" + i;
				var output = Table.ContentFirst[0][i-1] as string;

				if (output == "-") continue;

				var node = Network.GetNode(stateName);

				if (node == null || node.Output != output) return false;
			}

			for (var i = 1; i <= Table.StatesCount; i++)
			{
				var state = "a" + i;
				for (var j = 1; j <= Table.InputsCount; j++)
				{
					var input = "x" + j;
					var nextState = Table.ContentSecond[j - 1][i] as string;

					if (nextState == "-") continue;

					if (Network.GetConnection(state, nextState, input) == null) return false;
				}
			}

			return true;
		}

		/// <summary>
		/// A function to conveniently populate the view-model with test data.
		/// </summary>
		private void PopulateWithTestData()
		{
			int task = 1;
			NetworkType type = NetworkType.Moore;
			int stateCount = 6;
			int inputCount = 4;
			int outputCount = 3;

			//
			// Create a network, the root of the view-model.
			//
			this.Network = new NetworkViewModel(task, type, stateCount, inputCount, outputCount);

			UpdatePossibleListsValues();

			//
			// Create some nodes and add them to the view-model.
			//
			NodeViewModel node1 = CreateNode(possibleStates[0], new Point(100, 110), false);
			NodeViewModel node2 = CreateNode(possibleStates[1], new Point(350, 80), false);

			ConnectNodes(node1, node2);
		}

		private void UpdatePossibleListsValues()
		{
			var states = new List<string>();
			for (var i = 1; i <= Network.NumberOfStates; i++)
			{
				states.Add("a" + i);
			}
			PossibleStates = states;

			var inputs = new List<string>();
			for (var i = 1; i <= Network.NumberOfInputs; i++)
			{
				inputs.Add("x" + i);
			}
			PossibleInputs = inputs;

			var outputs = new List<string>();
			for (var i = 1; i <= Network.NumberOfOutputs; i++)
			{
				outputs.Add("y" + i);
			}
			PossibleOutputs = outputs;
		}

		#endregion Private Methods
	}

	public class TableChangedEventArgs : EventArgs
	{
		public TableViewModel Table;

		public TableChangedEventArgs(TableViewModel tablevm)
		{
			Table = tablevm;
		}
	}

	public delegate void TableChangedEventHandler(object sender, TableChangedEventArgs e);
}
