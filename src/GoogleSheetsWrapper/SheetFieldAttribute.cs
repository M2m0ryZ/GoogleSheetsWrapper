using System;
using System.Collections.Generic;

namespace GoogleSheetsWrapper
{
    /// <summary>
    /// These fields MUST match header of sheet fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SheetFieldAttribute : Attribute
    {
        private static readonly Dictionary<SheetFieldType, string> dictionary = new Dictionary<SheetFieldType, string>()
        {
            { SheetFieldType.String, "" },
            { SheetFieldType.Number, "#,##0.00" },
            { SheetFieldType.PhoneNumber, "(###)\" \"###\"-\"####" },
            { SheetFieldType.DateTime, "M/d/yyyy H:mm:ss" },
            { SheetFieldType.Currency, "\"$\"#,##0.00" },
            { SheetFieldType.Boolean, "#" },
            { SheetFieldType.Integer, "0" },
        };

        /// <summary>
        /// Returns a Dictionary collection of the default format patterns
        /// </summary>
        protected static Dictionary<SheetFieldType, string> DefaultFormatPatterns { get; set; } = dictionary;

        /// <summary>
        /// The display name of the column header in Google Sheets
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The column id of the field.  This is a 1 based index.  Column 'A' => column id 1
        /// </summary>
        public int ColumnID { get; set; }

        /// <summary>
        /// The SheetFieldType (ie String, DateTime, Currency, PhoneNumber, Number, etc
        /// </summary>
        public SheetFieldType FieldType { get; set; }

        private string _numberFormatPattern;

        /// <summary>
        /// Optional property to override the default format pattern for any of the SheetFieldTypes
        /// 
        /// Google Docs details on pattern values: https://developers.google.com/sheets/api/guides/formats
        /// </summary>
        public string NumberFormatPattern
        {
            get => _numberFormatPattern ?? DefaultFormatPatterns[FieldType];

            set => _numberFormatPattern = value;
        }
    }
}