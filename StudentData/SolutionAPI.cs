using ExcelDataReader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudentData
{
    internal class SolutionAPI
    {
        public static DataSet OpenExcel()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Excel Files(*.xls)|*.xls|Excel Files(*.xlsx)|*.xlsx";

            if (openFile.ShowDialog() == true)
            {
                string sourceFile = openFile.FileName;
                return ReadExcel(sourceFile);
            }

            return null;
        }

        public static DataSet ReadExcel(string fileName)
        {
            try
            {
                using (var stream = File.OpenRead(fileName))
                {
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = false  }
                        });

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:");
            }

            return null;
        }

        //public bool FilterRow(IExcelDataReader reader)
        //{
        //    DataTable dt = reader.;
        //}


        public static void WriteExcel()
        {

        }

    }
}
