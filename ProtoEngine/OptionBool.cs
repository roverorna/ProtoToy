﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace ProtoEngine
{
    /// <summary>
    /// Klasa odpowiadająca zmiennej typu Bool
    /// </summary>
    public class OptionBool : Option
    {
        /// <summary>
        /// Callback wywoływany gdy zostanie zapisana wartość tej opcji (nawet jeśli będzie taka jak poprzednia)
        /// </summary>
        /// <param name="option">zmieniony obiekt OptionBool</param>
        /// <param name="prevValue">poprzednia wartość</param>
        public delegate void OptionBoolChangeHandler(OptionBool option, bool? prevValue);

        private bool? myValue;
        /// <summary>
        /// Aktualna wartość opcji.
        /// </summary>
        public bool Value
        {
            get { return myValue == null ? false : (bool)myValue; }
            set {
                bool? prev = myValue;
                myValue = value;
                if(OptionBoolChanged != null)
                    OptionBoolChanged(this, prev);
            }
        }
        public OptionBoolChangeHandler OptionBoolChanged;

        public override string TypeName
        {
            get { return "bool"; }
        }

        public OptionBool(String name, String type)
            : base(name)
        {
        }

        /// <summary>
        /// Konstruktor przyjmujący fragmet DOM opisujący opcję
        /// </summary>
        /// <param name="name">nazwa do nadania tej opcji</param>
        /// <param name="node">fragment DOM</param>
        public OptionBool(String name, XmlNode node)
            : base(name)
        {
            String strValue = "";
            try
            {
                XmlNode strValueNode = node.Attributes.GetNamedItem("value");
                if (strValueNode == null)
                     throw new ArgumentException("catch me");
                strValue = strValueNode.Value;
            }
            catch (Exception ex)
            {
                // ignoruję te wyjątki - oznaczają tylko że opcja jest pusta
                if (ex is System.ArgumentException ||
                    ex is System.InvalidOperationException)
                {
                }
                else
                {
                    throw;
                }
            }
            if(strValue.Trim() != "")
                setValueFromString(strValue);
        }

        /// <summary>
        /// Konstruktor przyjmujący wprost początkową wartość
        /// </summary>
        /// <param name="name">nazwa opcji</param>
        /// <param name="value">wartość</param>
        public OptionBool(String name, bool value)
            : base(name)
        {
            this.Value = value;
        }


        public OptionBool(String name)
            : base(name)
        {
        }

        override public byte[] toBytes()
        {
            return null;
        }

        override public bool match(TransactionalStreamReader s)
        {
            return false;
        }

        override public Option copy()
        {
            return new OptionBool(Name, Value);
        }

        public override void setValueFromString(string valueString)
        {
            string v = valueString.Trim().ToLower();
            if(v == "true" || v == "1") {
                Value = true;
            } else if(v == "false" || v == "0") {
                Value = false;
            } else {
                throw new ArgumentException("Bad value for type bool: \"" + valueString + "\"");
            }
        }

        public override string getValueAsString()
        {
            if (Value)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public override bool Equals(object obj)
        {
            if (this.GetType().IsInstanceOfType(obj))
                return ((OptionBool)obj).Value == this.Value;
            return false;
        }
    }
}
