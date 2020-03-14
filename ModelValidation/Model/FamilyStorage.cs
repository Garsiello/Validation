using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using SchemaWrapperTools;

namespace FamilyManager.Actions
{
    public class FamilyStorage
    {
        /// <summary>
        /// Значение версии семейства, получаемое/посылаемое и/в БД
        /// </summary>
        public int Versia { get; set; }
        /// <summary>
        /// Значение регистрационного номера семейства, получаемое/посылаемое из/в БД
        /// </summary>
        public Guid RegNumber { get; set; }
        public Document RevitDocument { get; set; }


        private Family OwnerFamily => RevitDocument.OwnerFamily;
        private string NameFieldRegNumber = "FamilyRegNumber";
        private string NameFieldVersion = "FamilyVersion";
        
        /// <summary>
        /// Проеврка, создание или апгрейд схемы
        /// </summary>
        public void FamilyStorageProccess()
        {
            Schema schema = CheckSchema();
            if (null == schema)
            {
                schema = CreateSchema();
            }
            else
            {
                UpgradeSchema(schema);
            }
        }

        /// <summary>
        /// Создать схему с полями и записать в них значения, полученные из БД.
        /// </summary>
        /// <returns></returns>
        public Schema CreateSchema()
        {
            SchemaBuilder schemaBuilder = new SchemaBuilder(RegNumber);

            //Доступ схемы на чтение и запись.
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
            //Создаем поля.
            FieldBuilder versionFieldBuilder = schemaBuilder.AddSimpleField(NameFieldVersion, typeof(int));
            FieldBuilder regNumberFieldBuilder = schemaBuilder.AddSimpleField(NameFieldRegNumber, typeof(Guid));
            //Описание полей.
            versionFieldBuilder.SetDocumentation("Отображает версию семейства, которое используется в проекте");
            regNumberFieldBuilder.SetDocumentation("Регистрационный номер семейства, которое было скачено из базы семейств");
            //Запекаем схему.
            schemaBuilder.SetSchemaName("BI_FamilyDataStorage");
            Schema schema = schemaBuilder.Finish();
            //Создаем сущность
            Entity entity = new Entity(schema);
            //Ищем филды
            Field versionField = schema.GetField(NameFieldVersion);
            Field regNumberField = schema.GetField(NameFieldRegNumber);

            using (Transaction createStorage = new Transaction(RevitDocument))
            {
                createStorage.Start("CreateStorage");
                //Загоняем значения
                entity.Set(versionField, Versia);
                entity.Set(regNumberField, RegNumber, DisplayUnitType.DUT_CUSTOM);
                //Присвоить сущность
                OwnerFamily.SetEntity(entity);
                createStorage.Commit();
            }

            return schema;
        }

        /// <summary>
        /// Получить значения полей из схемы "Регистрационный номер" и "Версия семейства"
        /// </summary>
        public void GetData(out Guid regNumber, out int version)
        {
            regNumber = Guid.Empty;
            version = 0;
            var listGuids = OwnerFamily.GetEntitySchemaGuids();
            foreach (Guid Guid in listGuids)
            {
                Schema bi_Schema = Schema.Lookup(Guid);
                var entity = OwnerFamily.GetEntity(bi_Schema);
                if (entity.Schema.SchemaName == "BI_FamilyDataStorage")
                {
                    regNumber = entity.Get<Guid>(NameFieldRegNumber);
                    version = entity.Get<int>(NameFieldVersion);
                }
            }
        }

        /// <summary>
        /// Найти схему по Guid, полученному из БД или удалить похожую.
        /// </summary>
        /// <returns></returns>
        public Schema CheckSchema()
        {
            Schema schema = Schema.Lookup(RegNumber);
            if (schema == null)
            {
                var listGuids = OwnerFamily.GetEntitySchemaGuids();
                foreach (Guid Guid in listGuids)
                {
                    Schema BISchema = Schema.Lookup(Guid);
                    var entity = OwnerFamily.GetEntity(BISchema);
                    if (entity.Schema.SchemaName == "BI_FamilyDataStorage")
                    {
                        schema = entity.Schema;
                        using (Transaction tr = new Transaction(RevitDocument))
                        {
                            tr.Start("DeleteOldSchema");
                            OwnerFamily.DeleteEntity(schema);
                            tr.Commit();
                        }

                    }
                }
            }

            return schema;
        }

        /// <summary>
        /// Обновить значения полей в схеме.
        /// </summary>
        /// <param name="schema"></param>
        public void UpgradeSchema(Schema schema)
        {
            Guid oldRegNumber;
            int oldVersia;
            
            if (schema != null)
            {
                var oldEntity = OwnerFamily.GetEntity(schema);
                Entity newEntity = new Entity(schema);

                oldRegNumber = oldEntity.Get<Guid>(NameFieldRegNumber);
                //versia = oldEntity.Get<int>(NameFieldVersion) + 10;
                using (Transaction Upgrade = new Transaction(RevitDocument))
                {
                    Upgrade.Start("UpgradeSchema");
                    OwnerFamily.DeleteEntity(schema);

                    if (RegNumber == oldRegNumber)
                    {
                        newEntity.Set<Guid>(NameFieldRegNumber, oldRegNumber);
                        newEntity.Set<int>(NameFieldVersion, Versia);
                        OwnerFamily.SetEntity(newEntity);
                    }

                    else
                    {
                        TaskDialog.Show("Ошибка!", "Регистрационный номер семейства не совпадает с номером в базе");
                    }
                    Upgrade.Commit();
                }
            }
            else
            {
                TaskDialog.Show("Ошибка!", "К сожалению, произошел сбой во время публикации");
            }
        }
    }
}
