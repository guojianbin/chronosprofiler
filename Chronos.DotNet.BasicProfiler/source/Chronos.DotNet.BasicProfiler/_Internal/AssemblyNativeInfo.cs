﻿using System;
using Chronos.Model;
using Chronos.Storage;

namespace Chronos.DotNet.BasicProfiler
{
    [Serializable]
    [DataTable(TableName = "BasicProfiler_Assemblies")]
    public sealed class AssemblyNativeInfo : NativeUnitBase
    {
        public AssemblyNativeInfo()
        {
            AppDomainId = 0;
        }

        [DataTableColumn]
        public ulong AppDomainId { get; set; }
    }
}
