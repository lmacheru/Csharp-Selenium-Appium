using System;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace Selenium
{
    public class Reports
    {
        private string BrowserType;
        private string url;
        private DateTime date;
        private static string fileName;

        int rowNumber = 2;

        private static Application oXL;
        private static _Workbook oWB;
        private _Worksheet oSheet;
        private Range oRng;
        static DirectoryInfo dirInfo = null;

        public Reports(String BrowserType, string url, string className, string testName)
        {
            this.BrowserType = BrowserType;
            this.url = url;
            date = DateTime.Now;

            fileName = className;
            createCsvFile(testName);
        }

        public void createCsvFile(String testName)
        {
            try
            {
                oXL = new Application();
                oXL.Visible = true;

                oWB = oXL.Workbooks.Add("");
                oSheet = (_Worksheet)oWB.ActiveSheet;

                oSheet.Cells[1, 1] = "StepDescription";
                oSheet.Cells[1, 2] = "Pass/Fail";
                oSheet.Cells[1, 3] = "Exception";

                oSheet.get_Range("A1", "C1").Font.Bold = true;
                oSheet.get_Range("A1", "C1").VerticalAlignment = XlVAlign.xlVAlignCenter;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void closeReport()
        {
            //Creates the Results folder and sets the path to save the excel file
            string tmStamp1 = DateTime.Now.ToString("dd MMMM ");
            string tmStamp = DateTime.Now.ToString("hh-mm-ss tt");

            dirInfo = new DirectoryInfo(@".\" + "Results " + tmStamp1);
            string path = dirInfo.CreateSubdirectory(fileName).ToString() + @"\" + fileName+ tmStamp;

            oWB.SaveAs(path, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                        XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oXL.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oXL);
        }

        public void addLine(string description, string result, string exception)
        {
            oSheet.Cells[rowNumber, 1] = description;
            oSheet.Cells[rowNumber, 2] = result;
            oSheet.Cells[rowNumber, 3] = exception;

            rowNumber++;

            oRng = oSheet.get_Range("A1", "D1");
            oRng.EntireColumn.AutoFit();
        }

        //To enable saving and closing of file after exceution
        public static _Workbook GetWorkbook { get { return oWB; } }
    }
}
