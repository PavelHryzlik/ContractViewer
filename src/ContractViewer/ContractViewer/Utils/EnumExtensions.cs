using System;
using System.ComponentModel.DataAnnotations;

namespace ContractViewer.Utils
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get name form display attribute of the enum
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ToName(this Enum e)
        {
            var attributes = (DisplayAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes.Length > 0 ? attributes[0].Name : string.Empty;
        }
    }
}