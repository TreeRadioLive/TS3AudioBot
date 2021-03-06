using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TS3AudioBot.Algorithm
{
	public class DependencyGraph<T> where T : IEquatable<T>
	{
		List<Node> DependencyObjects { get; }
		public List<T> DependencyList { get; }

		public DependencyGraph()
		{
			DependencyObjects = new List<Node>();
			DependencyList = new List<T>();
		}

		public void Map(T obj, T[] depentants)
		{
			var node = new Node(obj, depentants);
			DependencyObjects.Add(node);
		}

		public void BuildList()
		{
			DependencyList.Clear();

			for (int i = 0; i < DependencyObjects.Count; i++)
				DependencyObjects[i].Id = i;
			foreach (var node in DependencyObjects)
				node.DIdList = node.DList.Select(x => DependencyObjects.First(y => y.DObject.Equals(x)).Id).ToArray();

			var reachable = new BitArray(DependencyObjects.Count, false);

			var remainingObjects = DependencyObjects.ToList();
			while (remainingObjects.Count > 0)
			{
				bool hasChanged = false;
				for (int i = 0; i < remainingObjects.Count; i++)
				{
					if (remainingObjects[i].DIdList.All(x => reachable[x]))
					{
						var node = remainingObjects[i];
						remainingObjects.RemoveAt(i);
						DependencyList.Add(node.DObject);
						reachable[node.Id] = true;
						hasChanged = true;
						i--;
					}
				}

				if (!hasChanged)
					throw new Exception("Dependecy graph is cyclic");
			}
		}

		public class Node
		{
			public T DObject { get; }
			public IList<T> DList { get; }
			public int Id { get; set; }
			public IList<int> DIdList { get; set; }

			public Node(T dObject, IList<T> dList)
			{
				DObject = dObject;
				DList = dList;
			}
		}
	}
}
