using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Frogger.Extensions
{
    /// <summary>
    ///     Method Extensions for Collections
    /// </summary>
    public static class ListExtensions
    {
        #region Methods

        /// <summary>Converts to observable collection.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>
        ///     A new observable collection
        /// </returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        #endregion
    }
}