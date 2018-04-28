using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utils;

namespace NetworkModel
{
	/// <summary>
	/// Defines a network of nodes and connections between the nodes.
	/// </summary>
	public sealed class NetworkViewModel : AbstractModelBase
	{
		#region Internal Data Members

		/// <summary>
		/// The collection of nodes in the network.
		/// </summary>
		private ImpObservableCollection<NodeViewModel> nodes = null;

		/// <summary>
		/// The collection of connections in the network.
		/// </summary>
		private ImpObservableCollection<ConnectionViewModel> connections = null;

		private int taskNumber;

		private NetworkType type;

		private int numberOfStates;

		private int numberOfInputs;

		private int numberOfOutputs;

		#endregion Internal Data Members

		/// <summary>
		/// The collection of nodes in the network.
		/// </summary>
		public ImpObservableCollection<NodeViewModel> Nodes
		{
			get
			{
				if (nodes == null)
				{
					nodes = new ImpObservableCollection<NodeViewModel>();
				}

				return nodes;
			}
		}

		/// <summary>
		/// The collection of connections in the network.
		/// </summary>
		public ImpObservableCollection<ConnectionViewModel> Connections
		{
			get
			{
				if (connections == null)
				{
					connections = new ImpObservableCollection<ConnectionViewModel>();
					connections.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(connections_ItemsRemoved);
				}

				return connections;
			}
		}

		public int NumberOfStates
		{
			get
			{
				return numberOfStates;
			}
			set
			{
				numberOfStates = value;
				OnPropertyChanged("NumberOfStates");
			}
		}

		public int NumberOfInputs
		{
			get
			{
				return numberOfInputs;
			}
			set
			{
				numberOfInputs = value;
				OnPropertyChanged("NumberOfInputs");
			}
		}

		public int NumberOfOutputs
		{
			get
			{
				return numberOfOutputs;
			}
			set
			{
				numberOfOutputs = value;
				OnPropertyChanged("NumberOfOutputs");
			}
		}

		public int TaskNumber
		{
			get
			{
				return taskNumber;
			}

			set
			{
				taskNumber = value;
				OnPropertyChanged("TaskNumber");
			}
		}

		public NetworkType Type
		{
			get
			{
				return type;

			}
			set
			{
				type = value;
				OnPropertyChanged("Type");
			}
		}

		public NetworkViewModel()
		{

		}

		public NetworkViewModel(int taskNumber, NetworkType type,
			int numberOfStates, int numberOfInputs, int numberOfOutputs)
		{
			TaskNumber = taskNumber;
			Type = type;
			NumberOfStates = numberOfStates;
			NumberOfInputs = numberOfInputs;
			NumberOfOutputs = numberOfOutputs;
		}

		public NetworkViewModel(int taskNumber, NetworkType type,
			int numberOfStates, int numberOfInputs, int numberOfOutputs,
			ICollection<NodeViewModel> nodes, ICollection<ConnectionViewModel> connections)
			: this(taskNumber, type, numberOfStates, numberOfInputs, numberOfOutputs)
		{
			Nodes.AddRange(nodes);
			Connections.AddRange(connections);
		}

		public NodeViewModel GetNode(string nodeName)
		{
			return Nodes.FirstOrDefault(node => node.Name == nodeName);
		}

		public ConnectionViewModel GetConnection(string node1Name, string node2Name)
		{
			return Connections.FirstOrDefault(cn =>
							cn.SourceConnector.ParentNode.Name == node1Name && cn.DestConnector.ParentNode.Name == node2Name);
		}

		public ConnectionViewModel GetConnection(string node1Name, string node2Name, string input)
		{
			return Connections.FirstOrDefault(cn => cn.Input == input
													&& cn.SourceConnector.ParentNode.Name == node1Name
													&& cn.DestConnector.ParentNode.Name == node2Name);
		}

		#region Private Methods

		/// <summary>
		/// Event raised then Connections have been removed.
		/// </summary>
		private void connections_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
		{
			foreach (ConnectionViewModel connection in e.Items)
			{
				connection.SourceConnector = null;
				connection.DestConnector = null;
			}
		}

		#endregion Private Methods
	}
}
