/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */
using System;

namespace Leopotam.EcsLite
{
	public class Disposable : IDisposable
	{
		public static IDisposable Empty = new Disposable(null);

		private Action _action;


		private Disposable(Action action)
		{
			_action = action;
		}


		void IDisposable.Dispose()
		{
			if (_action != null)
			{
				_action();
				_action = null;
			}
		}


		public static IDisposable Create(Action action)
		{
			return new Disposable(action);
		}
	}
}