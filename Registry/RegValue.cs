using System;
using Microsoft.Win32;
using System.Text;

namespace CrackSoft.RegExplore.Registry
{
    class RegValue : IComparable<RegValue>
    {
        string name;

        public string Name
        {
            get
            {
                if (IsDefault)
                    return "(Default)";
                else
                    return name;
            }
            set { name = value; }
        }

        public RegistryValueKind Kind { get; set; }

        public object Data { get; set; }

        public bool IsDefault
        {
            get { return name == String.Empty; }
        }

        public RegistryKey ParentKey { get; private set; }    

        public RegValue(string name, RegistryValueKind kind, object data)
        {
            this.name = name;
            Kind = kind;
            Data = data;
        }

        public RegValue(RegistryKey parentKey, string valueName) :
            this(valueName, parentKey.GetValueKind(valueName), parentKey.GetValue(valueName))
        {
            ParentKey = parentKey;
        }

        public override string ToString()
        {
            return ToString(Kind, Data);
        }

        public static string ToString(object valueData)
        {
            if (valueData is byte[])
                return Encoding.ASCII.GetString((byte[])valueData);
            else
                return valueData.ToString();
        }

        public static string ToString(RegistryValueKind valueKind, object valueData)
        {
            string data;
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    data = Encoding.ASCII.GetString((byte[])valueData);
                    break;
                case RegistryValueKind.MultiString:
                    data = String.Join(" ", (string[])valueData);
                    break;
                case RegistryValueKind.DWord:
                    data = ((UInt32)((Int32)valueData)).ToString();
                    break;
                case RegistryValueKind.QWord:
                    data = ((UInt64)((Int64)valueData)).ToString();
                    break;
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    data = valueData.ToString();
                    break;
                case RegistryValueKind.Unknown:
                default:
                    data = String.Empty;
                    break;
            }
            return data;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /* Implemented the IComparable interface
         * because Sort method of List<T> calls the CompareTo function
         * of the objects to compare them and I want the objects
         * of RegValue type to be sorted on basis of value name */
        #region IComparable<RegValue> Members

        public int CompareTo(RegValue other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion
    }
}
