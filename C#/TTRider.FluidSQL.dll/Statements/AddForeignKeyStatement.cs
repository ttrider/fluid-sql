using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class AddForeignKeyStatement : ForeignDefinitionBase, IStatement
    {
        public Name TableName { get; set; }
        public List<ForeignKeyColumn> Columns { get; } = new List<ForeignKeyColumn>();
        public bool NotSetForReplication { get; set; } = true;

        public bool CheckIfNotExists { get; set; }
    }

    public class ForeignKeyColumn
    {
        public Name Name { get; set; }
        public Name ReferencedName { get; set; }
    }

    public class DropForeignKeyStatement : IStatement
    {
        public Name Name { get; set; }
        public Name TableName { get; set; }

        public bool CheckIfExists { get; set; }
    }

    /* 
     [ FOREIGN KEY ]   
        REFERENCES [ schema_name . ] referenced_table_name [ ( ref_column ) ]   
        [ ON DELETE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ]   
        [ ON UPDATE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ]   
        [ NOT FOR REPLICATION ]   */
}