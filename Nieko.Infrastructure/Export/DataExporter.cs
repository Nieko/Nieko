using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nieko.Infrastructure.Export
{
    public class DataExporter : IDataExporter
    {
        private class NoDataExporter : IDataExporter
        {
            public bool IsEnabled { get { return false; } }

            public void Export(object data)
            {
                throw new NotImplementedException();
            }
        }

        public static IDataExporter None = new NoDataExporter();

        public virtual bool IsEnabled { get; set; }

        public virtual string ExportPath { get; set; }

        public Action PostExportAction { get; set; }

        public DataExporter()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            IsEnabled = true;
            ExportPath = string.Empty;
            PostExportAction = () => { };
        }

        public void Export(object data)
        {
            var stream = new MemoryStream();
            var exportWriter = new StreamWriter(stream);
            var exporter = new XmlExporter();

            exporter.Export(exportWriter, data);

            var exportReader = new StreamReader(stream);

            stream.Position = 0;
            var xmlData = exportReader.ReadToEnd();

            xmlData = xmlData.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");

            var fileStream = new StreamWriter(ExportPath, false);
            fileStream.Write(xmlData);

            fileStream.Close();

            PostExportAction(); 
        }
    }
}
