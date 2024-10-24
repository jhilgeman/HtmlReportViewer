/* ObservableObject.cs
 * @version: 1.0
 * @author: Jonathan Hilgeman <jhilgeman@gmail.com>
 * @created: 2020-03-04
 * @updated: 2023-04-03
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace HtmlReportViewer
{
    [Serializable]
    public class ObservableObject : INotifyPropertyChanged
    {
        [XmlIgnore]
        public Dictionary<string, object> __BackingFields = new Dictionary<string, object>();
        [XmlIgnore]
        public Dictionary<string, PropertyInfo> __PropertyInfos = new Dictionary<string, PropertyInfo>();

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public T Get<T>([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            return (T)GetValue(propertyName);
        }

        public bool Set(object value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null, params string[] addlNotify)
        {
            if (SetValue(propertyName, value))
            {
                NotifyPropertyChanged(propertyName);
                foreach (string alsoNotify in addlNotify)
                {
                    NotifyPropertyChanged(alsoNotify);
                }
                return true;
            }
            return false;
        }

        private object GetValue(string propertyName)
        {
            if (!__BackingFields.ContainsKey(propertyName))
            {
                var propertyInfo = GetPropertyInfo(propertyName);
                object defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
                __BackingFields.Add(propertyName, defaultValue);
            }
            return __BackingFields[propertyName];
        }

        private bool SetValue(string propertyName, object value)
        {
            if (!__BackingFields.ContainsKey(propertyName))
            {
                __BackingFields.Add(propertyName, value);
                return true;
            }
            else
            {
                if (value != __BackingFields[propertyName])
                {
                    __BackingFields[propertyName] = value;
                    return true;
                }
            }
            return false;
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (!__PropertyInfos.ContainsKey(propertyName))
            {
                var pi = this.GetType().GetProperty(propertyName);
                if (pi == null) { throw new System.Exception("Property " + propertyName + " not found!"); }
                __PropertyInfos.Add(propertyName, pi);
            }
            return __PropertyInfos[propertyName];
        }

    }
}
