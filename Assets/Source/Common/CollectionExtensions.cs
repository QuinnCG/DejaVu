using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public static class CollectionExtensions
	{
		/// <summary>
		/// Do some action for a given node as well as its posterity.
		/// </summary>
		/// <remarks>This should only be used for tree-like nodal structures, to avoid infinite recursion.</remarks>
		/// <param name="root">The starting node.</param>
		/// <param name="action">The action to execute on every node.</param>
		/// <param name="getChildren">How does the method retrieve the child of a given node.</param>
		public static void RecursiveTree<T>(this T root, System.Action<T> action, System.Func<T, IEnumerable<T>> getChildren)
		{
			action(root);

			foreach (var child in getChildren(root))
			{
				child.RecursiveTree(action, getChildren);
			}
		}

		public static T GetRandom<T>(this IEnumerable<T> collection)
		{
			if (!collection.Any())
				return default;

			return collection.ElementAt(Random.Range(0, collection.Count()));
		}

		/// <remarks>
		/// The higher the weight, the more likely it is to be chosen.<br/>
		/// All weights should be positive (> 0).
		/// </remarks>
		/// / <param name="reverse">If true, the weights are reversed; i.e. higher weights are less likely to be chosen.</param>
		public static T GetWeightedRandom<T>(this IEnumerable<T> collection, System.Func<T, float> getWeightCallback, bool reverse = false)
		{
			if (!collection.Any())
			{
				return default;
			}

			if (collection.Count() == 1)
			{
				return collection.First();
			}

			float sum = collection.Sum(x => getWeightCallback(x));

			foreach (var item in collection)
			{
				if (reverse)
				{
					if (Random.value <= (1f - (getWeightCallback(item) / sum)))
					{
						return item;
					}
				}
				else
				{
					if (Random.value <= getWeightCallback(item) / sum)
					{
						return item;
					}
				}
			}

			return collection.GetRandom();
		}
		public static T GetWeightedRandom<T>(this List<T> collection, params float[] weights)
		{
			if (collection.Count != weights.Length)
			{
				throw new System.ArgumentException("The number of weights must match the number of items in the collection!");
			}

			return GetWeightedRandom(collection, x => weights[collection.IndexOf(x)]);
		}

		public static T GetClosestTo<T>(this IEnumerable<T> collection, Vector2 point) where T : Component
		{
			T t = default;
			float nearest = float.PositiveInfinity;

			foreach (var item in collection)
			{
				float dst = item.transform.position.DistanceTo(point);

				if (dst < nearest)
				{
					nearest = dst;
					t = item;
				}
			}

			return t;
		}
		public static T GetClosestTo<T>(this IEnumerable<T> collection, System.Func<T, float> dstCallback)
		{
			T t = default;
			float nearest = float.PositiveInfinity;

			foreach (var item in collection)
			{
				float dst = dstCallback(item);

				if (dst < nearest)
				{
					nearest = dst;
					t = item;
				}
			}

			return t;
		}

		public static void ForEach<T>(this IEnumerable<T> collection, System.Action<T> action)
		{
			foreach (var item in collection)
			{
				action(item);
			}
		}

		public static void AddRange<T>(this List<T> collection, IEnumerable<T> toRemove)
		{
			foreach (var item in toRemove)
			{
				collection.Add(item);
			}
		}
		public static void AddRange<T>(this HashSet<T> collection, IEnumerable<T> toRemove)
		{
			foreach (var item in toRemove)
			{
				collection.Add(item);
			}
		}

		public static void RemoveRange<T>(this List<T> collection, IEnumerable<T> toRemove)
		{
			foreach (var item in toRemove)
			{
				collection.Remove(item);
			}
		}
		public static void RemoveRange<T>(this HashSet<T> collection, IEnumerable<T> toRemove)
		{
			foreach (var item in toRemove)
			{
				collection.Remove(item);
			}
		}
		public static void RemoveRange<T, U>(this Dictionary<T, U> collection, IEnumerable<T> toRemove)
		{
			foreach (var item in toRemove)
			{
				collection.Remove(item);
			}
		}
	}
}
