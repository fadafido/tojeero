using System;
using System.Collections.Generic;
using System.Linq;

namespace Tojeero.Core
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
	}
}

