using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kraken.Expressions.NetStandard
{
#if NETSTANDARD1_6
	using Microsoft.Extensions.DependencyModel;

	internal class AppDomain
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		public static AppDomain CurrentDomain { get; private set; }

		static AppDomain()
		{
			CurrentDomain = new AppDomain();
		}

		public Assembly[] GetAssemblies()
		{
			var assemblies = new List<Assembly>();
			var dependencies = DependencyContext.Default.RuntimeLibraries;
			foreach (var library in dependencies)
			{
				if (IsCandidateCompilationLibrary(library))
				{
					var assembly = Assembly.Load(new AssemblyName(library.Name));
					assemblies.Add(assembly);
				}
			}
			return assemblies.ToArray();
		}

		private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary)
		{
			return compilationLibrary.Name == ("Specify")
				|| compilationLibrary.Dependencies.Any(d => d.Name.StartsWith("Specify"));
		}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}
#endif
}
