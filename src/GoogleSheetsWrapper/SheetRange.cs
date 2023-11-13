using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleSheetsWrapper
{
    /// <summary>
    /// SheetRange object representing a Google Sheet range representation
    /// </summary>
    public class SheetRange : IEquatable<SheetRange>
    {
        /// <summary>
        /// Is this A1 notation?
        /// </summary>
        public string A1Notation { get; private set; }

        /// <summary>
        /// Is this R1C1 notation?
        /// </summary>
        public string R1C1Notation { get; private set; }

        /// <summary>
        /// Can this range support A1 notation?
        /// </summary>
        public bool CanSupportA1Notation { get; private set; }

        /// <summary>
        /// Is this a single cell?
        /// </summary>
        public bool IsSingleCellRange { get; private set; }

        /// <summary>
        /// StartColumn value (1 based index)
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// StartRow value (1 based index)
        /// </summary>
        public int StartRow { get; set; }

        /// <summary>
        /// EndColumn value (1 based index)
        /// </summary>
        public int? EndColumn { get; set; }

        /// <summary>
        /// EndRow value (1 based index)
        /// </summary>
        public int? EndRow { get; set; }

        /// <summary>
        /// Tab name in Google Sheets
        /// </summary>
        public string TabName { get; set; }

        private static readonly List<string> aToZ
            = Enumerable.Range('A', 26)
                .Select(x => (char)x + "")
                .ToList();

        /// <summary>
        /// Row and column numbers are 1 based indexes
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="startColumn"></param>
        /// <param name="endColumn"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        public SheetRange(string tabName, int startColumn, int startRow, int? endColumn = null, int? endRow = null)
        {
            if (endRow.HasValue && endColumn.HasValue)
            {
                this.R1C1Notation = $"R{startRow}C{startColumn}:R{endRow}C{endColumn}";
            }
            else
            {
                this.R1C1Notation = $"R{startRow}C{startColumn}";
                this.IsSingleCellRange = true;
            }

            if (!string.IsNullOrEmpty(tabName))
            {
                this.R1C1Notation = $"{tabName}!{this.R1C1Notation}";
            }

            if (endColumn.HasValue)
            {
                this.CanSupportA1Notation = true;
                this.IsSingleCellRange = !endRow.HasValue;

                var startLetters = GetLettersFromColumnID(startColumn);
                var endLetters = GetLettersFromColumnID(endColumn.Value);

                this.A1Notation = $"{startLetters}{startRow}:{endLetters}{endRow}";

                if (!string.IsNullOrEmpty(tabName))
                {
                    this.A1Notation = $"{tabName}!{this.A1Notation}";
                }
            }

            this.StartColumn = startColumn;
            this.StartRow = startRow;
            this.EndRow = endRow;
            this.EndColumn = endColumn;
            this.TabName = tabName ?? string.Empty;
        }

        /// <summary>
        /// Create a SheetRange from an A1 notation or an R1C1 notation
        /// </summary>
        /// <param name="rangeValue"></param>
        public SheetRange(string rangeValue)
        {
            var parser = new SheetRangeParser();
            SheetRange range;

#pragma warning disable IDE0045 // IF statement can be simplified

            if (parser.IsValidR1C1Notation(rangeValue))
            {
                range = parser.ConvertFromR1C1Notation(rangeValue);
            }
            else if (parser.IsValidA1Notation(rangeValue))
            {
                range = parser.ConvertFromA1Notation(rangeValue);
            }
            else
            {
                throw new ArgumentException($"rangeValue: {rangeValue} is not a valid range!");
            }

#pragma warning restore IDE0045 // IF statement can be simplified

            this.TabName = range.TabName;
            this.StartRow = range.StartRow;
            this.EndRow = range.EndRow;
            this.StartColumn = range.StartColumn;
            this.EndColumn = range.EndColumn;
            this.A1Notation = range.A1Notation;
            this.R1C1Notation = range.R1C1Notation;
            this.CanSupportA1Notation = range.CanSupportA1Notation;
            this.IsSingleCellRange = range.IsSingleCellRange;
        }

        /// <summary>
        /// columnId is a 1 based index
        /// </summary>
        /// <param name="columnID"></param>
        /// <returns></returns>
        public static string GetLettersFromColumnID(int columnID)
        {
            var block = columnID - 1;

            var columnLettersNotation = "";

            while (block >= 0)
            {
                columnLettersNotation += aToZ[block % 26];

                block = (block / 26) - 1;
            }

            columnLettersNotation = new string(columnLettersNotation.ToCharArray().Reverse().ToArray());

            return columnLettersNotation;
        }

        /// <summary>
        /// The resulting column id is on a 1 based index (i.e. A => 1)
        /// </summary>
        /// <param name="letters"></param>
        /// <returns></returns>
        public static int GetColumnIDFromLetters(string letters)
        {
            var result = 0;

            letters = letters.ToUpper();

            for (var i = 0; i < letters.Count(); i++)
            {
                var currentLetter = letters[i];
                var currentLetterNumber = (int)currentLetter;

                result *= 26;
                result += currentLetterNumber - 'A' + 1;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(SheetRange obj)
        {
            return HashCode.Combine(obj.StartRow, obj.StartColumn, obj.EndColumn, obj.EndRow, obj.TabName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SheetRange other)
        {
            return
                this.A1Notation == other.A1Notation &&
                this.CanSupportA1Notation == other.CanSupportA1Notation &&
                this.EndColumn == other.EndColumn &&
                this.EndRow == other.EndRow &&
                this.IsSingleCellRange == other.IsSingleCellRange &&
                this.R1C1Notation == other.R1C1Notation &&
                this.StartColumn == other.StartColumn &&
                this.StartRow == other.StartRow &&
                this.TabName == other.TabName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            return this.Equals((SheetRange)other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.GetHashCode(this);
        }
    }
}