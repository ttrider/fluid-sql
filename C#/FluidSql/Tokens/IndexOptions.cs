// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    internal class IndexOptions : Token,
        ICreateOrAlterIndexOptions,
        IDropIndexOptions,
        ISetIndexOptions
    {
        public bool? Online { get; set; }
        public int? MaxDegreeOfParallelism { get; set; }
        public bool? PadIndex { get; set; }
        public int? Fillfactor { get; set; }
        public bool? SortInTempdb { get; set; }
        public bool? IgnoreDupKey { get; set; }
        public bool? StatisticsNorecompute { get; set; }
        public bool? DropExisting { get; set; }
        public bool? AllowRowLocks { get; set; }
        public bool? AllowPageLocks { get; set; }
        //TODO: DATA_COMPRESSION = { NONE | ROW | PAGE} [ ON PARTITIONS ( { <partition_number_expression> | <range> } [ , ...n ] ) ]
        public bool IsDefined => this.Online.HasValue ||
                                 this.MaxDegreeOfParallelism.HasValue ||
                                 this.PadIndex.HasValue ||
                                 this.Fillfactor.HasValue ||
                                 this.SortInTempdb.HasValue ||
                                 this.IgnoreDupKey.HasValue ||
                                 this.StatisticsNorecompute.HasValue ||
                                 this.DropExisting.HasValue ||
                                 this.AllowRowLocks.HasValue ||
                                 this.AllowPageLocks.HasValue;
    }
}