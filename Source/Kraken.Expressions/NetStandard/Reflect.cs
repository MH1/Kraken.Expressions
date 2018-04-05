using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kraken.Expressions.NetStandard
{
    static class Reflect
    {
		public static Assembly[] GetAssemblies()
		{
#if NETSTANDARD1_6
			return NetStandard.AppDomain.CurrentDomain.GetAssemblies();
#else
			return AppDomain.CurrentDomain.GetAssemblies();
#endif
		}
	}
}
