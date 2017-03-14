﻿using NPOI.XSSF.UserModel;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Globalization;

namespace ProStudCreator
{
    public class ExcelCreator
    {
        private static string SHEET_NAME = "Projects";
        private static string MARKETING_SHEET_NAME = "Projekte";
        private static readonly string[] HEADERS = new string[] {
                "Abbreviation",
                "Name",
                "Display Name",
                "1P_Type",
                "2P_Type",
                "1P_Teamsize",
                "2P_Teamsize",
                "Major",
                "Wichtigkeit",
                "Betreuer",
                "Betreuer2",
                "Modules",
                "Fixe Zuteilung",
                "Fixe Zuteilung 2",
                "Kommentar",
                "ID",
                "SingleSemester",
                "Continuation",
                "German",
                "English"
            };

        // References
        // - http://poi.apache.org/spreadsheet/quick-guide.html#NewWorkbook

        public static void GenerateProjectList(Stream outStream, IEnumerable<Project> _projects)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(SHEET_NAME);

            // Header
            worksheet.CreateRow(0);
            for (int i = 0; i < HEADERS.Length; i++)
            {
                worksheet.GetRow(0).CreateCell(i).SetCellValue(HEADERS[i]);
            }

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                projectToExcelRow(projects[i], row);
            }

            for (int i = 0; i < HEADERS.Length; i++)
                worksheet.AutoSizeColumn(i);

            // Save
            workbook.Write(outStream);
        }

        private static void projectToExcelRow(Project p, IRow row)
        {
            ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
            p.Semester = p.Semester == null ? Semester.NextSemester(db) : p.Semester = p.Semester;

            string abbreviation = /*Semester.CurrentSemester.ToString() +*/ p.Semester.ToString() + "_" + p.Department.DepartmentName + p.ProjectNr.ToString("D2");
            string dispName = abbreviation + "_" + p.Name;

            row.CreateCell(0).SetCellValue(abbreviation);
            row.CreateCell(1).SetCellValue(p.Name);
            row.CreateCell(2).SetCellValue(dispName);
            row.CreateCell(3).SetCellValue(p.POneType.Export());
            row.CreateCell(4).SetCellValue(p.PTwoType != null ? p.PTwoType.Export() : p.POneType.Export());
            row.CreateCell(5).SetCellValue(p.POneTeamSize.Export());
            row.CreateCell(6).SetCellValue(p.PTwoTeamSize != null ? p.PTwoTeamSize.Export() : p.POneTeamSize.Export());
            row.CreateCell(7).SetCellValue("-");    // Major undefined
            row.CreateCell(8).SetCellValue(0);      // Importance undefined
            row.CreateCell(9).SetCellValue(p.Advisor1Mail);
            row.CreateCell(10).SetCellValue(p.Advisor2Mail);
            row.CreateCell(11).SetCellValue("");    // Modules undefined
            row.CreateCell(12).SetCellValue(p.Reservation1Mail);
            row.CreateCell(13).SetCellValue(p.Reservation2Mail);
            row.CreateCell(14).SetCellValue("");  // Comment undefined
            row.CreateCell(15).SetCellValue(p.Id);

            row.CreateCell(16).SetCellValue(p.DurationOneSemester ? 1 : 0);
            row.CreateCell(17).SetCellValue(p.IsContinuation ? 1 : 0);
            row.CreateCell(18).SetCellValue(p.LanguageGerman ? 1 : 0);
            row.CreateCell(19).SetCellValue(p.LanguageEnglish ? 1 : 0);
        }


        public static void GenerationMarketingList(Stream outStream, IEnumerable<Project> _projects, ProStudentCreatorDBDataContext db)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(MARKETING_SHEET_NAME);

            // Header
            worksheet.CreateRow(0);
            for (int i = 0; i < MARKETING_HEADERS.Length; i++)
            {
                worksheet.GetRow(0).CreateCell(i).SetCellValue(MARKETING_HEADERS[i]);
            }

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                ProjectToExcelMarketingRow(projects[i], row, db);
            }

            for (int i = 0; i < HEADERS.Length; i++)
                worksheet.AutoSizeColumn(i);

            // Save
            workbook.Write(outStream);
        }


        private static void ProjectToExcelMarketingRow(Project p, IRow row, ProStudentCreatorDBDataContext db)
        {

            var abbreviation = /*Semester.CurrentSemester.ToString() +*/ p.Semester.ToString() + "_" +
                                                                         p.Department.DepartmentName +
                                                                         p.ProjectNr.ToString("D2");

            var i = 0;
            row.CreateCell(i++).SetCellValue(abbreviation);
            row.CreateCell(i++).SetCellValue(p.Department.DepartmentName);
            row.CreateCell(i++).SetCellValue(p.Name);
            row.CreateCell(i++).SetCellValue(p.Semester?.StartDate.ToString(CultureInfo.InvariantCulture) ?? "");
            row.CreateCell(i++).SetCellValue(p.GetSubmissionDate());
            row.CreateCell(i++).SetCellValue(p.GetEndSemester(db).ExhibitionBachelorThesis ?? "");
            row.CreateCell(i++).SetCellValue(p.LogStudent1Name ?? "");
            row.CreateCell(i++).SetCellValue(p.LogStudent1Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.LogStudent2Name ?? "");
            row.CreateCell(i++).SetCellValue(p.LogStudent2Mail ?? "");
            row.CreateCell(i++).SetCellValue(string.IsNullOrEmpty(p.Reservation1Mail) ? "Nein" : "Ja");
            row.CreateCell(i++).SetCellValue(p.Advisor1Name ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor1Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor2Name ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor2Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.Project1?.Name ?? "");
            row.CreateCell(i++).SetCellValue(p.LogProjectType?.ExportValue ?? "-");
            row.CreateCell(i++).SetCellValue(p.LogProjectDuration?.ToString() ?? "");
            row.CreateCell(i++).SetCellValue(GetLanguage(p));
            row.CreateCell(i++).SetCellValue(p.Expert?.Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.LogDefenceDate?.ToString() ?? "-");
            row.CreateCell(i++).SetCellValue(p.LogDefenceRoom ?? "-");
            row.CreateCell(i++).SetCellValue(p.LogGradeStudent1?.ToString() ?? "");
            row.CreateCell(i++).SetCellValue(p.LogGradeStudent2?.ToString() ?? "");
            row.CreateCell(i++).SetCellValue(p.BillingStatus?.DisplayName ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientCompany ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressTitle ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientPerson ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientMail ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressDepartment ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressStreet ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressPostcode ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressCity ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientReferenceNumber ?? "");
        }

        private static readonly string[] MARKETING_HEADERS = new string[]
        {
            "Projektnummer",
            "Insitut",
            "Projekttitel",
            "Projektstart",
            "Projektabgabe",
            "Ausstellung Bachelorthesis",
            "Student/in 1",
            "Student/in 1 E-Mail",
            "Student/in 2",
            "Studnet/in 2 E-Mail",
            "Wurde reserviert",
            "Hauptbetreuende/r",
            "Hauptbetreuende/r E-Mail",
            "Nebenbetreuende/r",
            "Nebenbetreuende/r E-Mail",
            "Weiterführung von",
            "Projekttyp",
            "Anzahl Semester",
            "Durchführungssprache",
            "Experte",
            "Verteidigungsdatum",
            "Verteidigungsraum",
            "Note Student/in 1",
            "Note Student/in 2",
            "Verrechungsstatus",
            "Kunden-Unternehmen",
            "Kunden-Anrede",
            "Kunden-Name",
            "Kunden-E-Mail Adresse",
            "Kunden-Abteilung",
            "Kunden-Strasse und Nummer",
            "Kunden-PLZ",
            "Kunden-Ort",
            "Referenznummer des Kunden"
        };

        private static string GetLanguage(Project p)
        {
            if ((p.LogLanguageGerman ?? false) && !(p.LogLanguageEnglish ?? false))
            {
                return "Deutsch";
            }

            if (!(p.LogLanguageGerman ?? false) && (p.LogLanguageEnglish ?? false))
            {
                return "Englisch";
            }

            return "";

        }
    }


}