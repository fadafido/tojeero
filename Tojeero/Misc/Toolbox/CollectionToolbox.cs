using System;
using System.Collections.Generic;
using System.Linq;
using Tojeero.Core.ViewModels;

namespace Tojeero.Core.Toolbox
{
	public static class CollectionToolbox
	{
		/// <summary>
		/// Inserts the items into the list in a sorted order defined by comparer. 
		/// It's assumed that the list is already sorted by the same sort order as produced by comparer.
		/// If the comparer is null all the items are appended at the end of the list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="items">Items.</param>
		/// <param name="comparer">Comparer.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void InsertSorted<T>(this IList<T> list, IEnumerable<T> items, Comparison<T> comparer = null)
		{
			if (items == null)
				return;
			list = list ?? new List<T>();
			if (comparer == null)
			{
				foreach (var obj in items)
					list.Add(obj);
			}
			else
			{
				var sorted = items.ToList<T>();
				sorted.Sort(comparer);
				int j = 0;
				foreach (var item in sorted)
				{					
					int comparison = 0;
					//Find the place where the item should be inserted
					while (j < list.Count && (comparison = comparer.Invoke(list[j], item)) < 0)
						j++;
					//If they are equal, simply update the item
					if (j < list.Count && comparison == 0)
						list[j] = item;
					//Otherwise insert the new item in new place
					else
						list.Insert(j, item);
					j++;
				}
			}
		}

		/// <summary>
		/// Adds each element from items to collection.
		/// </summary>
		/// <param name="collection">Collection.</param>
		/// <param name="items">Items.</param>
		/// <typeparam name="T">The type of collection.</typeparam>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			if (items == null)
				return;
			foreach (var item in items)
				collection.Add(item);
		}

		public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> items)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (items == null)
				throw new ArgumentNullException("items");
			if (index < 0 || index > list.Count)
				throw new IndexOutOfRangeException();
			int j = index;
			foreach(var item in items)
			{
				list.Insert(j++, item);
			}
		}

		public static IList<T> SubCollection<T>(this IList<T> items, int index, int count)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			if (index < 0 || index > items.Count)
				throw new IndexOutOfRangeException();
			var list = new List<T>();
			for (int i = index; i < Math.Min(items.Count, index + count); i++)
			{
				list.Add(items[i]);
			}
			return list;
		}

		public static string PrintCollection<T>(this IEnumerable<T> collection, string joinString = "\n", Func<T, string> format = null)
		{
			var items = collection.Select(item => format != null ? format(item) : item.ToString());
			var result = string.Join(joinString, items);
			Tools.Logger.Log(result);
			return result;
		}

		public static IEnumerable<FacetViewModel<T>> ApplyFacets<T>(this IEnumerable<T> objects, Dictionary<string, int> facets, bool countVisible = true) where T : IUniqueEntity
		{
			if (objects == null || facets == null)
				return null;
			return objects.Join(facets, i => i.ID, f => f.Key, (i, f) => new FacetViewModel<T>(i, f.Value, countVisible)).ToList();
		}

		public static T[] Concatenate<T>(this T[] arr1, T[] arr2)
		{
			if (arr1 == null)
				return arr2;
			if (arr2 == null)
				return arr1;
			var z = new T[arr1.Length + arr2.Length];
			arr1.CopyTo(z, 0);
			arr2.CopyTo(z, arr1.Length);
			return z;
		}
			
	}
}

