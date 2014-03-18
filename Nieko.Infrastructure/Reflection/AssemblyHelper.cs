using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.Reflection
{
    /// <summary>
    /// Facade helper class for finding assemblies and types in the 
    /// current AppDomain
    /// </summary>
    public static class AssemblyHelper
    {
        private static object _LockObject = new object();
        private static HashSet<string> SpecificExclusions { get; set; }

        private static List<Assembly> _ApplicationAssemblies;

        /// <summary>
        /// All non-Microsoft assemblies referenced or loaded via Modules
        /// by this application 
        /// </summary>
        public static List<Assembly> ApplicationAssemblies
        {
            get
            {
                lock (_LockObject)
                {
                    if (_ApplicationAssemblies == null)
                    {
                        var entryAssembly = Assembly.GetEntryAssembly();

                        var foundAssemblies = new List<Assembly>
                        {
                            entryAssembly
                        };
                        var processedAssemblies = new HashSet<Assembly>(foundAssemblies);

                        AddReferencedAssemblies(entryAssembly, foundAssemblies, processedAssemblies);
                        ProcessAssembliesForAddition(AppDomain.CurrentDomain.GetAssemblies(), foundAssemblies, processedAssemblies);

                        _ApplicationAssemblies = foundAssemblies;
                    }
                }

                return _ApplicationAssemblies;
            }
        }

        static AssemblyHelper()
        {
            SpecificExclusions = new HashSet<string>
            {
                "CommonLanguageRuntimeLibrary",
                "WindowsBase.dll",
                "PresentationCore.dll",
                "PresentationFramework.dll",
                "PresentationFramework.Classic.dll",
                "UIAutomationProvider.dll",
                "UIAutomationTypes.dll",
                "PresentationCFFRasterizer.dll",
                "PresentationUI.dll",
                "ReachFramework.dll"
            };

            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainAssemblyLoad;
        }

        private static bool IsApplicationAssembly(Assembly assembly)
        {
            var result = assembly.ManifestModule.Name != "<In Memory Module>"
                    && !assembly.FullName.StartsWith("System")
                    && !assembly.FullName.StartsWith("Microsoft")
                    && assembly.Location.IndexOf("App_Web") == -1
                    && assembly.Location.IndexOf("App_global") == -1
                    && assembly.FullName.IndexOf("CppCodeProvider") == -1
                    && assembly.FullName.IndexOf("WebMatrix") == -1
                    && assembly.FullName.IndexOf("SMDiagnostics") == -1
                    && !SpecificExclusions.Contains(assembly.ManifestModule.ScopeName);

            return result;
        }

        /// <summary>
        /// Searches application assemblies for types that match the filter
        /// </summary>
        /// <param name="filter">Predicate to include types by</param>
        /// <returns>Types matching filter</returns>
        public static IEnumerable<Type> FindTypes(Func<Type, bool> filter)
        {
            return ApplicationAssemblies.SelectMany(a => a.GetTypes().Where(filter));
        }

        private static void AddReferencedAssemblies(Assembly assembly, List<Assembly> foundAssemblies, HashSet<Assembly> processedAssemblies)
        {
            ProcessAssembliesForAddition(assembly.GetReferencedAssemblies()
                .Select(name => Assembly.Load(name)), foundAssemblies, processedAssemblies);
        }

        private static void ProcessAssembliesForAddition(IEnumerable<Assembly> assemblies, List<Assembly> foundAssemblies, HashSet<Assembly> processedAssemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (processedAssemblies.Contains(assembly))
                {
                    continue;
                }

                processedAssemblies.Add(assembly);
                if (!IsApplicationAssembly(assembly))
                {
                    continue;
                }

                foundAssemblies.Add(assembly);

                AddReferencedAssemblies(assembly, foundAssemblies, processedAssemblies);
            }
        }

        /// <summary>
        /// Returns the string values of all public static string properties on a type
        /// </summary>
        /// <param name="type">Type with static string members</param>
        /// <returns>Static string property values</returns>
        public static IEnumerable<string> GetStaticStrings(Type type)
        {
            return type.GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => p.GetValue(null, null) as string);
        }

        public static void ResetCache()
        {
            lock (_LockObject)
            {
                _ApplicationAssemblies = null;
            }
        }

        private static void CurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            ResetCache();
        }
    }
}
