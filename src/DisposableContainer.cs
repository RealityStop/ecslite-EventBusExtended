/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Leopotam.EcsLite
{

	public static class DisposableContainerExt
	{
		[DebuggerStepThrough]
		public static void DisposeWith(this IDisposable self, DisposableContainer parent)
		{
			parent.Add(self);
		}


		[DebuggerStepThrough]
		public static T DisposeWith<T>(this T self, DisposableContainer parent) where T : IDisposable
		{
			parent.Add(self);
			return self;
		}
	}

	/// <summary>
	///     A reusable container for disposables.  Can be used over and over, and each time it is disposed it will dispose any
	///     items in it and reset itself to be used again.
	/// </summary>
	public class DisposableContainer : IDisposable
	{
		//private bool _isDisposed = false;
		private readonly CompositeDisposable _bufferA = new();
		private readonly CompositeDisposable _bufferB = new();

		private readonly object lockObject = new();
		private bool isDisposing;


		public DisposableContainer()
		{
			ActiveCollection = _bufferA;
			BufferCollection = _bufferB;
		}


		private CompositeDisposable ActiveCollection { get; set; }
		private CompositeDisposable BufferCollection { get; set; }


		public int Count
		{
			get
			{
				lock (lockObject)
				{
					return ActiveCollection.Count + BufferCollection.Count;
				}
			}
		}


		public void Dispose()
		{
			//Swap the two collections under lock, so new items go to the empty and existing items go to the new active
			lock (lockObject)
			{
				//But we can't swap until the previous cleanup was done, so just... don't
				if (isDisposing)
					return;

				isDisposing = true;

				(ActiveCollection, BufferCollection) = (BufferCollection, ActiveCollection);
			}

			//We don't actually dispose under the lock.  That could deadlock.  But we should only dispose once.

			BufferCollection.Dispose();


			lock (lockObject)
			{
				isDisposing = false;
			}
		}


		public void Add(IDisposable item)
		{
			lock (lockObject)
			{
				ActiveCollection.Add(item);
			}
		}


		public void Add(Action act)
		{
			Add(Disposable.Create(act));
		}


		public void Clear()
		{
			lock (lockObject)
			{
				ActiveCollection.Clear();
				BufferCollection.Clear();
			}
		}


		public bool Contains(IDisposable item)
		{
			lock (lockObject)
			{
				return ActiveCollection.Contains(item) || BufferCollection.Contains(item);
			}
		}


		public void CopyTo(IDisposable[] array, int arrayIndex)
		{
			lock (lockObject)
			{
				ActiveCollection.Concat(BufferCollection).ToArray().CopyTo(array, arrayIndex);
			}
		}


		public IDisposable Get()
		{
			return Disposable.Create(() => { Dispose(); });
		}


		public IEnumerator<IDisposable> GetEnumerator()
		{
			throw new NotImplementedException();
		}


		public bool Remove(IDisposable item)
		{
			lock (lockObject)
			{
				return ActiveCollection.Remove(item);
			}
		}


		private class CompositeDisposable : List<IDisposable>, IDisposable
		{
			public void Dispose()
			{
				foreach (var item in this)
					if (item != null)
						item.Dispose();

				Clear();
			}
		}
	}
}