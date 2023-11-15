﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Frogger.Extensions
{
    public static class ListExtensions
    {
        #region Methods

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        #endregion
    }
}