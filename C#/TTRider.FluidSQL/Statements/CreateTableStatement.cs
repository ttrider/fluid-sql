// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class CreateTableStatement : IStatement
    {
        public CreateTableStatement()
        {
            this.Columns = new List<TableColumn>();
            this.UniqueConstrains = new List<ConstrainDefinition>();
            this.Indicies = new List<CreateIndexStatement>();
        }

        public Name Name { get; set; }

        public bool AsFiletable { get; set; }

        public List<TableColumn> Columns { get; private set; }

        public ConstrainDefinition PrimaryKey { get; set; }

        public List<ConstrainDefinition> UniqueConstrains { get; private set; }

        public List<CreateIndexStatement> Indicies { get; private set; }

        public bool CheckIfNotExists { get; set; }

        public bool IsTemporary { get; set; }

        public bool IsTableVariable { get; set; }

        public ISelectStatement AsSelectStatement { get; set; }

        //TODO:  [ ON { partition_scheme_name ( partition_column_name ) | filegroup | "default" } ] 
        //TODO:  [ { TEXTIMAGE_ON { filegroup | "default" } ] 
        //TODO:  [ FILESTREAM_ON { partition_scheme_name | filegroup | "default" } ]
        //TODO:  [ WITH ( <table_option> [ ,...n ] ) ]
    }
}

/*

CREATE TABLE 
    [ database_name . [ schema_name ] . | schema_name . ] table_name 
    [ AS FileTable ]
    ( { <column_definition> | <computed_column_definition> 
        | <column_set_definition> | [ <table_constraint> ] [ ,...n ] } )
    [ ON { partition_scheme_name ( partition_column_name ) | filegroup 
        | "default" } ] 
    [ { TEXTIMAGE_ON { filegroup | "default" } ] 
    [ FILESTREAM_ON { partition_scheme_name | filegroup 
        | "default" } ]
    [ WITH ( <table_option> [ ,...n ] ) ]
[ ; ]

<column_definition> ::= 
column_name <data_type>
    [ FILESTREAM ]
    [ COLLATE collation_name ] 
    [ SPARSE ]
    [ NULL | NOT NULL ]
    [ 
        [ CONSTRAINT constraint_name ] DEFAULT constant_expression ] 
      | [ IDENTITY [ ( seed ,increment ) ] [ NOT FOR REPLICATION ] 
    ]
    [ ROWGUIDCOL ] 
    [ <column_constraint> [ ...n ] ] 

<data type> ::= 
[ type_schema_name . ] type_name 
    [ ( precision [ , scale ] | max | 
        [ { CONTENT | DOCUMENT } ] xml_schema_collection ) ] 

<column_constraint> ::= 
[ CONSTRAINT constraint_name ] 
{     { PRIMARY KEY | UNIQUE } 
        [ CLUSTERED | NONCLUSTERED ] 
        [ 
            WITH FILLFACTOR = fillfactor  
          | WITH ( < index_option > [ , ...n ] ) 
        ] 
        [ ON { partition_scheme_name ( partition_column_name ) 
            | filegroup | "default" } ]

  | [ FOREIGN KEY ] 
        REFERENCES [ schema_name . ] referenced_table_name [ ( ref_column ) ] 
        [ ON DELETE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
        [ ON UPDATE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
        [ NOT FOR REPLICATION ] 

  | CHECK [ NOT FOR REPLICATION ] ( logical_expression ) 
} 

<computed_column_definition> ::= 
column_name AS computed_column_expression 
[ PERSISTED [ NOT NULL ] ]
[ 
    [ CONSTRAINT constraint_name ]
    { PRIMARY KEY | UNIQUE }
        [ CLUSTERED | NONCLUSTERED ]
        [ 
            WITH FILLFACTOR = fillfactor 
          | WITH ( <index_option> [ , ...n ] )
        ]
        [ ON { partition_scheme_name ( partition_column_name ) 
        | filegroup | "default" } ]

    | [ FOREIGN KEY ] 
        REFERENCES referenced_table_name [ ( ref_column ) ] 
        [ ON DELETE { NO ACTION | CASCADE } ] 
        [ ON UPDATE { NO ACTION } ] 
        [ NOT FOR REPLICATION ] 

    | CHECK [ NOT FOR REPLICATION ] ( logical_expression ) 
] 

<column_set_definition> ::= 
column_set_name XML COLUMN_SET FOR ALL_SPARSE_COLUMNS

< table_constraint > ::=
[ CONSTRAINT constraint_name ] 
{ 
    { PRIMARY KEY | UNIQUE } 
        [ CLUSTERED | NONCLUSTERED ] 
        (column [ ASC | DESC ] [ ,...n ] ) 
        [ 
            WITH FILLFACTOR = fillfactor 
           |WITH ( <index_option> [ , ...n ] ) 
        ]
        [ ON { partition_scheme_name (partition_column_name)
            | filegroup | "default" } ] 
    | FOREIGN KEY 
        ( column [ ,...n ] ) 
        REFERENCES referenced_table_name [ ( ref_column [ ,...n ] ) ] 
        [ ON DELETE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
        [ ON UPDATE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
        [ NOT FOR REPLICATION ] 
    | CHECK [ NOT FOR REPLICATION ] ( logical_expression ) 
} 
<table_option> ::=
{
    [DATA_COMPRESSION = { NONE | ROW | PAGE }
      [ ON PARTITIONS ( { <partition_number_expression> | <range> } 
      [ , ...n ] ) ]]
    [ FILETABLE_DIRECTORY = <directory_name> ] 
    [ FILETABLE_COLLATE_FILENAME = { <collation_name> | database_default } ]
    [ FILETABLE_PRIMARY_KEY_CONSTRAINT_NAME = <constraint_name> ]
    [ FILETABLE_STREAMID_UNIQUE_CONSTRAINT_NAME = <constraint_name> ]
    [ FILETABLE_FULLPATH_UNIQUE_CONSTRAINT_NAME = <constraint_name> ]
}

<index_option> ::=
{ 
    PAD_INDEX = { ON | OFF } 
  | FILLFACTOR = fillfactor 
  | IGNORE_DUP_KEY = { ON | OFF } 
  | STATISTICS_NORECOMPUTE = { ON | OFF } 
  | ALLOW_ROW_LOCKS = { ON | OFF} 
  | ALLOW_PAGE_LOCKS ={ ON | OFF} 
  | DATA_COMPRESSION = { NONE | ROW | PAGE }
       [ ON PARTITIONS ( { <partition_number_expression> | <range> } 
       [ , ...n ] ) ]
}
<range> ::= 
<partition_number_expression> TO <partition_number_expression>*/