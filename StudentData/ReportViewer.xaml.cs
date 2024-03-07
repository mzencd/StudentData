using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudentData
{
    /// <summary>
    /// Interaction logic for Reporter.xaml
    /// </summary>
    public partial class Reporter : Window
    {
        public DataView reportView { get; set; }
        private bool _isReportViewerLoaded;

        public Reporter()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            reportViewer.Load += ReportViewer_Load;
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            ReportViewer _reportViewer = (ReportViewer)sender;

            if (!_isReportViewerLoaded)
            {
                ReportDataSource reportDataSource1 = new ReportDataSource();
                DatabaseDataSet dataset = new DatabaseDataSet();

                dataset.BeginInit();

                reportDataSource1.Name = "StudentDataSet"; //Name of the report dataset in our .RDLC file
                reportDataSource1.Value = dataset.StudentList;
                _reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                _reportViewer.LocalReport.ReportEmbeddedResource = "StudentData.Report1.rdlc";

                dataset.EndInit();

                //fill data into adventureWorksDataSet
                DatabaseDataSetTableAdapters.StudentListTableAdapter salesOrderDetailTableAdapter = new DatabaseDataSetTableAdapters.StudentListTableAdapter();
                salesOrderDetailTableAdapter.ClearBeforeFill = true;
                salesOrderDetailTableAdapter.Fill(dataset.StudentList);

                DatabaseDataSet.StudentListDataTable studentList = dataset.StudentList;
                if(reportView != null)
                {
                    foreach (DataRowView rowView in reportView)
                    {
                        if (rowView.Row.RowState ==  DataRowState.Deleted || rowView.Row.RowState == DataRowState.Detached) { continue; }

                        studentList.AddStudentListRow(rowView["Level"].ToString(), rowView["Classroom"].ToString(), rowView["Rate"].ToString(), rowView["Code"].ToString(),
                            rowView["Percentage"].ToString(), rowView["StudentName"].ToString(), rowView["Gender"].ToString(), Convert.ToDateTime(rowView["BirthDate"]), rowView["BirthPlace"].ToString());
                    }
                }

                _reportViewer.RefreshReport();

                _isReportViewerLoaded = true;
            }
        }
    }
}
