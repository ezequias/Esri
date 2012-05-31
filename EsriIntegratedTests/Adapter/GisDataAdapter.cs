using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace Tests 
{
    public class GisDataAdapter
    {
        public readonly IObject DataObject;

        public GisDataAdapter(IObject gisObject)
        {
            this.DataObject = gisObject;
        }

        public virtual IObjectClass DataObjectClass
        {
            get
            {
                return DataObject.Class;
            }
        }

        public virtual string TableName
        {
            get
            {
                IDataset dataset = (IDataset)DataObject.Class;
                return dataset.Name;
            }
        }

        public virtual int GetFieldIndex(string fieldName)
        {
            int fieldIndex = DataObject.Fields.FindField(fieldName);

            if (fieldIndex != -1)
            {
                return fieldIndex;
            }
            else
            {
                throw new Exception("Coluna não existe:" + fieldName);
            }
        }

        public virtual object GetFieldValue(string fieldName)
        {
            int fieldIndex = GetFieldIndex(fieldName);
            return DataObject.get_Value(fieldIndex);
        }

        public virtual void SetFieldValue(string fieldName, object value)
        {
            int fieldIndex = GetFieldIndex(fieldName);

            try
            {
                DataObject.set_Value(fieldIndex, value);
            }
            catch (System.Exception)
            {
                throw new ApplicationException(string.Format("Não foi possível definir o valor {0} para o campo {1} da {2}", value, fieldName, TableName));
            }
        }

        public string[] GetListOfFieldNames()
        {
            string[] fieldNames = new string[DataObject.Fields.FieldCount];

            for (int i = 0; i < DataObject.Fields.FieldCount; i++)
            {
                fieldNames[i] = DataObject.Fields.get_Field(i).Name;
            }

            return fieldNames;
        }

        public string[] GetListOfFieldValues()
        {
            string[] fieldValues = new string[DataObject.Fields.FieldCount];

            for (int i = 0; i < DataObject.Fields.FieldCount; i++)
            {
                object value = DataObject.get_Value(i);
                fieldValues[i] = value != null ? value.ToString() : string.Empty;
            }

            return fieldValues;
        }

        public void SaveChanges()
        {
            try
            {
                DataObject.Store();
            }
            catch (System.Exception)
            {
                throw new ApplicationException(string.Format("Ocorreu um erro ao salvar as alterações neste objeto da {0}.", TableName));
            }
        }

        public List<FieldWrapper> GetListOfFieldNamesAndValues()
        {
            List<FieldWrapper> fields = new List<FieldWrapper>();

            for (int i = 0; i < DataObject.Fields.FieldCount; i++) fields.Add(new FieldWrapper(DataObject.Fields.get_Field(i), DataObject.get_Value(i)));

            return fields;
        }

        public virtual IField GetFieldFromRelationedObject(esriRelRole relationshipType, string tableName, string fieldName)
        {
            IObject relatoned = GetRelationedObject(relationshipType, tableName, fieldName);
            if (relatoned != null)
                return relatoned.Fields.get_Field(relatoned.Fields.FindField(fieldName));
            else
                return null;
        }

        public virtual List<IRelationshipClass> GetRelationshipClasses(esriRelRole relationshipType)
        {
            List<IRelationshipClass> relationshipClasses = new List<IRelationshipClass>();
            IEnumRelationshipClass enumRelationshipClass = this.DataObject.Class.get_RelationshipClasses(relationshipType);
            IRelationshipClass relationshipClass;

            try
            {
                while ((relationshipClass = enumRelationshipClass.Next()) != null)
                {
                    relationshipClasses.Add(relationshipClass);
                }
            }
            catch (System.Exception exception)
            {
                throw new System.ApplicationException("Ocorreu um erro ao obter os Relacionamentos desta Feature. \n Detalhes: " + exception.Message);
            }

            return relationshipClasses;
        }

        public virtual IObject GetRelationedObject(esriRelRole relationshipType, string tableName, string fieldName)
        {
            IObject obj = this.DataObject;
            IEnumRelationshipClass enumRelatClass = obj.Class.get_RelationshipClasses(relationshipType);
            IRelationshipClass relatClass = enumRelatClass.Next();
            while (relatClass != null)
            {
                if (((IDataset)relatClass.OriginClass).Name.ToLower() == tableName.ToLower())
                {
                    IEnumRelationship enumRelat = relatClass.GetRelationshipsForObject(obj);
                    IRelationship relat = enumRelat.Next();
                    if (relat != null)
                    {
                        IObject relationed = relat.OriginObject;
                        return relationed;
                    }
                }
                else if (((IDataset)relatClass.DestinationClass).Name.ToLower() == tableName.ToLower())
                {
                    IEnumRelationship enumRelat = relatClass.GetRelationshipsForObject(obj);
                    IRelationship relat = enumRelat.Next();
                    if (relat != null)
                    {
                        IObject relationed = relat.DestinationObject;
                        return relationed;
                    }
                }
                relatClass = enumRelatClass.Next();
            }
            return null;
        }

        public virtual object GetFieldValueFromRelationedObject(esriRelRole relationshipType, string tableName, string fieldName)
        {
            IObject relationed = GetRelationedObject(relationshipType, tableName, fieldName);
            if (relationed == null)
                throw new System.NullReferenceException("Não existe relacionamento entre as tabelas ('" + TableName + "' e '" + tableName + "')");

            int fieldIndex = relationed.Fields.FindField(fieldName);
            if (fieldIndex == -1)
                throw new System.NullReferenceException("A tabela '" + tableName + "' não contém o campo " + fieldName);

            return relationed.get_Value(fieldIndex);
        }

        public virtual IWorkspace Workspace
        {
            get
            {
                return ((IDataset)DataObject.Class).Workspace;
            }
        }


    }
}