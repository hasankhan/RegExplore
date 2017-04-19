using System;
using System.Windows.Forms;
using System.Globalization;
using System.Text;

namespace CrackSoft.UI.Controls
{
    partial class NumericTextBox : TextBox
    {
        char decimalSeparator;
        char groupSeparator;
        char negativeSign;
        bool hexNumber;
        bool allowDecimal;
        bool allowGrouping;
        bool allowNegative;

        public override int MaxLength
        {
            get
            {
                return base.MaxLength;
            }
            set {}
        }

        public override bool ShortcutsEnabled
        {
            get
            {
                return base.ShortcutsEnabled;
            }
            set {}
        }

        public bool HexNumber
        {
            get { return hexNumber; }
            set
            {
                if (hexNumber ^ value)
                {
                    if (value)
                    {
                        if (AllowNegative)
                            Text = IntValue.ToString("x");
                        else
                            Text = UIntValue.ToString("x");
                        base.MaxLength = 8;
                    }
                    else
                    {
                        if (AllowNegative)
                            Text = IntValue.ToString();
                        else
                            Text = UIntValue.ToString();
                        base.MaxLength = 10;
                    }
                    hexNumber = value;
                }
            }
        }
        
        public bool AllowDecimal
        {
            get { return allowDecimal; }
            set
            {
                allowDecimal = value;
                FilterText();
            }
        }
        public bool AllowGrouping
        {
            get { return allowGrouping; }
            set
            {
                allowGrouping = value;
                FilterText();
            }
        }
        public bool AllowNegative 
        {
            get { return allowNegative; }
            set
            {
                allowNegative = value;
                FilterText();
            }
        }
        public int IntValue
        {
            get
            {
                try
                {
                    if (Text == String.Empty)
                        return 0;
                    else if (HexNumber)
                        return Int32.Parse(Text, NumberStyles.HexNumber);
                    else
                        return Int32.Parse(Text);
                }
                catch
                {
                    return Int32.MaxValue;
                }                
            }
        }
        public uint UIntValue
        {
            get
            {
                try
                {
                    if (Text == String.Empty)
                        return 0;
                    else if (HexNumber)
                        return UInt32.Parse(Text, NumberStyles.HexNumber);
                    else
                        return UInt32.Parse(Text);
                }
                catch (Exception)
                {
                    return UInt32.MaxValue;
                }
            }
        }

        public ulong ULongValue
        {
            get
            {
                try
                {
                    if (Text == String.Empty)
                        return 0;
                    else if (HexNumber)
                        return UInt64.Parse(Text, NumberStyles.HexNumber);
                    else
                        return UInt64.Parse(Text);
                }
                catch (Exception)
                {
                    return UInt64.MaxValue;
                }
            }
        }

        public float DecimalValue
        {
            get
            {
                try
                {
                    if (Text == String.Empty)
                        return 0;
                    else if (HexNumber)
                        return Single.Parse(Text, NumberStyles.HexNumber);
                    else
                        return Single.Parse(Text);
                }
                catch
                {
                    return Single.NaN;
                }
            }
        }

        public NumericTextBox()
        {
            InitializeComponent();
            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            decimalSeparator = numberFormatInfo.NumberDecimalSeparator[0];
            groupSeparator = numberFormatInfo.NumberGroupSeparator[0];
            negativeSign = numberFormatInfo.NegativeSign[0];
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            e.Handled = !IsDigit(e.KeyChar);
        }

        private bool IsDigit(char ch)
        {            
            bool result = (
                            Char.IsControl(ch) ||
                            Char.IsDigit(ch) ||
                            (HexNumber && Char.IsLetter(ch) && Char.ToLower(ch) <= 'f') ||
                            (AllowNegative && Text.Length == 0 && ch.Equals(negativeSign)) ||
                            (AllowGrouping && Text.Length > 0 && ch.Equals(groupSeparator)) ||
                            (AllowDecimal && ch.Equals(decimalSeparator) && Text.IndexOf(decimalSeparator) == -1)
                           );
            return result;
        }
        
        private void FilterText()
        {
            StringBuilder output = new StringBuilder(Text.Length);
            foreach (char ch in Text)
            {
                if (IsDigit(ch))
                    output.Append(ch);
            }
            if (output.Length != Text.Length)
                Text = output.ToString();
        }

    }
}
