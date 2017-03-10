// Sharp Essentials
// Copyright 2016 Matthew Hamilton - matthamilton@live.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Globalization;
using System.Windows.Data;

namespace SharpEssentials.Controls.Converters
{
    /// <summary>
    /// Returns boolean based on whether a value is greater than a limit.
    /// </summary>
    public class GreaterThanConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <see cref="IValueConverter.Convert"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var comparable = value as IComparable;
            return comparable?.CompareTo(Limit) > 0;
        }

        /// <see cref="IValueConverter.ConvertBack"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion

        /// <summary>
        /// The value to compare to.
        /// </summary>
        public int Limit { get; set; }
    }
}