using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.printerlib
{
    class printObjects
    {
    }
    public class GetPrinterPort
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public string ElementName { get; set; }
        public string InstanceID { get; set; }
        public string CommunicationStatus { get; set; }
        public string DetailedStatus { get; set; }
        public string HealthState { get; set; }
        public string InstallDate { get; set; }
        public string Name { get; set; }
        public string OperatingStatus { get; set; }
        public string OperationalStatus { get; set; }
        public string PrimaryStatus { get; set; }
        public string Status { get; set; }
        public string StatusDescriptions { get; set; }
        public string ComputerName { get; set; }
        public string PortMonitor { get; set; }
        public bool LprByteCounting { get; set; }
        public string LprQueueName { get; set; }
        public uint PortNumber { get; set; }
        public string PrinterHostAddress { get; set; }
        public string PrinterHostIP { get; set; }
        public uint Protocol { get; set; }
        public string SNMPCommunity { get; set; }
        public bool SNMPEnabled { get; set; }
        public uint SNMPIndex { get; set; }
        public string PSComputerName { get; set; }
    }

    public class GetPrinterDriver
    {
        public string Caption { get; set; }
        public string[] ColorProfiles { get; set; }
        public uint CommunicationStatus { get; set; }
        public string ComputerName { get; set; }
        public string ConfigFile { get; set; }
        public string[] CoreDriverDependencies { get; set; }
        public string DataFile { get; set; }
        public string Date { get; set; }
        public string DefaultDatatype { get; set; }
        public string[] DependentFiles { get; set; }
        public string Description { get; set; }
        public uint DetailedStatus { get; set; }
        public System.UInt64 DriverVersion { get; set; }
        public string ElementName { get; set; }
        public string HardwareID { get; set; }
        public uint HealthState { get; set; }
        public string HelpFile { get; set; }
        public string InfPath { get; set; }
        public string InstallDate { get; set; }
        public string InstanceID { get; set; }
        public bool IsPackageAware { get; set; }
        public uint MajorVersion { get; set; }
        public string Manufacturer { get; set; }
        public string Monitor { get; set; }
        public string Name { get; set; }
        public string OEMUrl { get; set; }
        public uint OperatingStatus { get; set; }
        public uint[] OperationalStatus { get; set; }
        //public string Path { get; set; }
        public string[] PreviousCompatibleNames { get; set; }
        public uint PrimaryStatus { get; set; }
        public string PrinterEnvironment { get; set; }
        public string PrintProcessor { get; set; }
        public string provider { get; set; }
        public string PSComputerName { get; set; }
        public string Status { get; set; }
        public string[] StatusDescriptions { get; set; }
        public string VendorSetup { get; set; }
    }
    public class GetPrinter
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public string ElementName { get; set; }
        public string InstanceID { get; set; }
        public string CommunicationStatus { get; set; }
        public string DetailedStatus { get; set; }
        public string HealthState { get; set; }
        public string InstallDate { get; set; }
        public string Name { get; set; }
        public string OperatingStatus { get; set; }
        public string OperationalStatus { get; set; }
        public string PrimaryStatus { get; set; }
        public string Status { get; set; }
        public string StatusDescriptions { get; set; }
        public uint BranchOfficeOfflineLogSizeMB { get; set; }
        public string Comment { get; set; }
        public string ComputerName { get; set; }
        public string Datatype { get; set; }
        public uint DefaultJobPriority { get; set; }
        public uint DeviceType { get; set; }
        public bool DisableBranchOfficeLogging { get; set; }
        public string DriverName { get; set; }
        public uint JobCount { get; set; }
        public bool KeepPrintedJobs { get; set; }
        public string Location { get; set; }
        public string PermissionSDDL { get; set; }
        public string PortName { get; set; }
        public uint PrinterStatus { get; set; }
        public string PrintProcessor { get; set; }
        public uint Priority { get; set; }
        public bool Published { get; set; }
        public string RenderingMode { get; set; }
        public string SeparatorPageFile { get; set; }
        public bool Shared { get; set; }
        public string ShareName { get; set; }
        public uint StartTime { get; set; }
        public uint Type { get; set; }
        public uint UntilTime { get; set; }
        public string WorkflowPolicy { get; set; }
        public string PSComputerName { get; set; }
        public string PrinterHostAddress { get; set; }
    }
}
