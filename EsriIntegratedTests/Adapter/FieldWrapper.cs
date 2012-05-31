using System;
using ESRI.ArcGIS.Geodatabase;

namespace Tests 
{
    public class FieldWrapper
    {
        private IField field;
        private object value;

        public FieldWrapper(IField field, object value)
        {
            this.field = field;
            this.value = value;
        }

        public object Value
        {
            get
            {
                object returnValue = value;
                ICodedValueDomain codedValueDomain = field.Domain as ICodedValueDomain;

                if (codedValueDomain != null)
                {
                    for (int domainCount = 0; domainCount < codedValueDomain.CodeCount; domainCount++)
                    {
                        object domainValue = codedValueDomain.get_Value(domainCount);

                        if (domainValue != null && domainValue.ToString().Equals(value.ToString()))
                        {
                            returnValue = codedValueDomain.get_Name(domainCount);
                            break;
                        }
                    }
                }

                return returnValue;
            }
        }

        public string Name
        {
            get
            {
                return field.Name;
            }
        }

        public string Alias
        {
            get
            {
                return field.AliasName;
            }
        }

        public Type Type
        {
            get
            {
                esriFieldType esriType = field.Type;

                // Does this field have a domain?
                ICodedValueDomain codedValueDomain = field.Domain as ICodedValueDomain;

                if (codedValueDomain != null)
                {
                    return typeof(string);
                }

                switch (esriType)
                {
                    case esriFieldType.esriFieldTypeBlob:
                        //beyond scope of sample to deal with blob fields
                        return typeof(string);
                    case esriFieldType.esriFieldTypeDate:
                        return typeof(DateTime);
                    case esriFieldType.esriFieldTypeDouble:
                        return typeof(double);
                    case esriFieldType.esriFieldTypeGeometry:
                        return typeof(string);
                    case esriFieldType.esriFieldTypeGlobalID:
                        return typeof(string);
                    case esriFieldType.esriFieldTypeGUID:
                        return typeof(Guid);
                    case esriFieldType.esriFieldTypeInteger:
                        return typeof(Int32);
                    case esriFieldType.esriFieldTypeOID:
                        return typeof(Int32);
                    case esriFieldType.esriFieldTypeRaster:
                        //beyond scope of sample to correctly display rasters
                        return typeof(string);
                    case esriFieldType.esriFieldTypeSingle:
                        return typeof(Single);
                    case esriFieldType.esriFieldTypeSmallInteger:
                        return typeof(Int16);
                    case esriFieldType.esriFieldTypeString:
                        return typeof(string);
                    default:
                        return typeof(string);
                }
            }
        }
    }
}