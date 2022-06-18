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
		public static T DisposeWith<T>(this T self, DisposableContainer hostingContainer) where T : IDisposable
		{
			hostingContainer.Add(self);
			return self;
		}
	}

	/// <summary>
	///     A reusable container for disposables.  Can be used over and over, and each time
	///     it is disposed it will dispose any items in it and reset itself to be used again.
	/// </summary>
	public class DisposableContainer : List<IDisposable>, IDisposable
	{
		public void Dispose()
		{
			foreach (var item in this)
				item?.Dispose();

			Clear();
		}
	}
}