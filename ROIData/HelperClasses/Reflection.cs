using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.HelperClasses {
    public static class Reflection {
		public static T GetField<T>(Type t, string field, object instance) {
			if (t == null)
				throw new ArgumentNullException(nameof(t));
			if (string.IsNullOrEmpty(field))
				throw new ArgumentNullException(nameof(field));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var fieldInfo = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);

			if (fieldInfo == null)
				throw new ArgumentException($"Could not find field {field} on instance {instance}.");

			var returnVal = (T)fieldInfo.GetValue(instance);

			if (returnVal == null)
				throw new ArgumentException($"Could not convert {fieldInfo.FieldType} to {typeof(T)}.");

			return returnVal;
		}

		public static void SetField<T>(Type t, string field, object instance, T value) {
			if (t == null)
				throw new ArgumentNullException(nameof(t));
			if (string.IsNullOrEmpty(field))
				throw new ArgumentNullException(nameof(field));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var fieldInfo = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);

			if (fieldInfo == null)
				throw new ArgumentException($"Could not find field {field} on instance {instance}.");

			fieldInfo.SetValue(instance, value);
		}

		public static void ExecuteMethod(Type t, string method, object instance) {
			if (t == null)
				throw new ArgumentNullException(nameof(t));
			if (string.IsNullOrEmpty(method))
				throw new ArgumentNullException(nameof(method));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var methodInfo = t.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);

			if (methodInfo == null)
				throw new ArgumentException($"Could not find method {method} on instance {instance}.");

			methodInfo.Invoke(instance, null);
		}
	}
}
